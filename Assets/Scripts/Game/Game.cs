using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine.Events;


[ExecuteAlways]
public class Game : MonoBehaviour {
//{ necessities
//{ Game needs to be instantiated in order to work with the unity inspector,
//  but its functionality is otherwise static.
//  the solution is to store a static pointer to the instance, to allow
//  static methods to access the instance's properties.
	public static Game S;
//}
//{ MonoBehaviour functions
	void Awake() {
		ResetStaticVariables();
		S = this;
	}
	void Start() {
#if UNITY_EDITOR
		// game code:
		if (EditorApplication.isPlaying) {
#endif
			BuildResources();
			Add(Resource.Time,startingTime);
			if (cards.Length==0) cards = deck;
			CheckDuplicateCards();
			if (shuffle) {
				ShuffleDeck();
			}
			BuildZoneDict();
			PackZone(Zone.Deck);
			foreach (Card card in zoneCards[Zone.Deck]) {
				card.AnimateInstant();
				card.SetColour(Define.Colour(Status.Unplayable));
			}
			DrawStartOfGame();
#if UNITY_EDITOR
		}
#endif
	}
	public bool addCard = false;
	public bool addCloneOfLast;
	public bool packCounters = false;
	public Renderer counterZone;
	public GameObject cardPrefab;
	void Update() {
#if UNITY_EDITOR
		// game code:
		if (EditorApplication.isPlaying) {
#endif
			TriggerNextAnimation();
			if (requests.Count != 0&&!selecting) {
				ProcessRequest();
			}
#if UNITY_EDITOR
		// editor code:
		} else {
			S = this;
			// make the addCard checkbox add a new card to the deck (in the editor)
			if (addCard) {
				addCard=false;
				Card newCard=(PrefabUtility.InstantiatePrefab(cardPrefab) as GameObject).GetComponent<Card>();
				if (addCloneOfLast) {
					newCard.CopyDesign(deck.Last());
				}
				Card[] newDeck = new Card[deck.Length+1];
				for (int i=0;i<deck.Length;i++) {
					newDeck[i]=S.deck[i];
				}
				newDeck[deck.Length]=newCard;
				deck=newDeck;
			}
			if (packCounters) {
				Counter[] counters = Define.GetCounterList();
				Bounds[] bounds = Packing.PackBounds(counters[0].GetComponent<Renderer>().bounds.size,counterZone.bounds,11,false,false);
				for (int i=0;i<11;i++) {
					counters[i].transform.localScale = Packing.GetLocalScale(bounds[i],counters[i].gameObject);
					counters[i].transform.position = bounds[i].center;
				}
				// packCounters=false;
			}
			cards = S.deck;
			if (S.packDeckEditor && Define.S != null) {
				cards = deck;
				PackDeckEditor();
			}
			foreach (Card card in cards) {
				if (card != null) {
					card.name=card.cardName;
				}
			}
		}
#endif
	}
//}
//}
//{ design elements
	public Card[] deck;
	public static Card[] cards; // static copy of deck
	public bool shuffle;
//}
//{ interface
//{ Animation queue so that cards (mostly) animate 1 at a time
	public static Queue<Card> AnimationQueue = new Queue<Card>();
	public static void TriggerNextAnimation() {
		while (AnimationQueue.Count!=0&&!cards.Any(x=>x.animatingStart())) {
			AnimationQueue.Dequeue().Animate();
		}
	}
	public static void QueueAnimation(Card card) {
		AnimationQueue.Enqueue(card);
	}
//}
//{ sort and arrange cards in a zone
// called whenever a card animates into that zone
	public static Dictionary<Card,Bounds> cardBounds = new Dictionary<Card,Bounds>();
	public static void PackZone(Zone zone) {
		if (!zonePacked[zone]) {
			zonePacked[zone] = true;
			bool rightToLeft = zone==Zone.Junk;
			Bounds[] boundsList = Packing.PackBounds(cards[0].GetComponent<Renderer>().bounds.size,Define.Bounds(zone),zoneCards[zone].Count,true,rightToLeft);
			if (zone==Zone.Junk) {
			}
			for (int i=0;i<boundsList.Length;i++) {
				cardBounds[zoneCards[zone][i]] = boundsList[i];
			}
		}
	}
	public static void SortZone(Zone zone) {
		if (zone != Zone.Hand) return;
		var query = zoneCards[zone].OrderBy(card => card.Priority());
		zoneCards[zone] = query.ToList();
	}
	// tests each card in hand to see if it can be installed, or whether it can be installed in conjunction with another card
	// costly, so only called when gamestate changes (ie. successful install/uninstall, return, or start of game)
	public static void TestCardsStatus() {
		List<Card> list = new List<Card>(zoneCards[Zone.Hand]);
		foreach(Card card in list) {
			if (card.CanInstall()) {
				card.status = Status.Playable;
			} else {
				card.status = Status.Unplayable;
			}
		}
		foreach(Card card in list) {
			if (card.status==Status.Unplayable&&zoneCards[Zone.Hand].Count<22) {
				foreach(Card friend in list) {
					if (card.CanInstallWith(friend)) {
						card.status = Status.Almost;
					}
				}
			}
			card.SetColour(Define.Colour(card.status));
		}
	}
//}
//{ ask the user to select a card in a zone (on behalf of another card)
// sequence of events:
// 1. request is added
// 2. if there's a request and no current selector, process the next request at next frame update
// 3. tell the cards in the relevant zone to wait to be selected
// 4a. if the user uninstalls the requesting card, cancel the selection (card will clear pending requests)
// 4b. when the user selects a card, tell the selector
// 5. process the next request
// 6. if there are no more requests, finish and tell the selector to output
	public static Card selector = null;
	public static bool selecting = false;
	public static Stack<Tuple<Card,Zone>> requests = new Stack<Tuple<Card,Zone>>(); // requests are a stack to make requesting reversible
	public static void RequestSelection(Zone zone,Card card) {
		requests.Push(new Tuple<Card,Zone>(card,zone));
	}
	public static void UndoLastRequest() {
		if (requests.Count!=0) {
			requests.Pop();
		} else { 			// requests are always made/cancelled in the same quantity, so this means a request has been processed
			EndSelection(); // undoing a processed request means cancelling the selection interface
		}
	}
	public static void ProcessRequest() {
		if (requests.Count==0) {			// if all the requests have been processed, we've selected enough cards and it's time for the part to use them
			selector.ProcessSelections();
			EndSelection();
		} else {
			Tuple<Card,Zone> request = requests.Pop();
			selector=request.Item1;
			StartSelection(request.Item2);
		}
	}
	public static void StartSelection(Zone zone) {
			foreach (Card card in zoneCards[zone]) {
				card.MakeSelectable();
			}
			selecting=true;
	}
	public static void EndSelection() {
		foreach (Card card in cards) {
			card.EndSelection();
		}
		selecting=false;
	}
	public static void Select(Card card) {
		selector.GiveSelection(card);
		ProcessRequest();
	}
	public static void UnSelect(Card card) {
		RequestSelection(card.zone,selector);
		selector.LoseSelection(card);
	}
//}
//}
//{ game logic
//{ deck shuffler
	private static System.Random rng = new System.Random();
	public static void ShuffleDeck() {
		int n = cards.Length;
		int k;
		while (n > 1) {  
			n--;
			k = rng.Next(n + 1);
			Card swap = cards[k];
			cards[k] = cards[n];
			cards[n] = swap;
		}
		n=0;
		k=0;
		while (n<cards.Length) {
			if (cards[n].startsHand) {
				Card swap = cards[k];
				cards[k] = cards[n];
				cards[n] = swap;
				k++;
			}
			n++;
		}	
	}
//}
//{ draw cards
	public int startingHandSize,startingTime;
	public static void DrawStartOfGame() {
		foreach (Card card in zoneCards[Zone.Deck]) {
			QueueAnimation(card);
		}
		for (int i=0;i<S.startingHandSize;i++) {
			if (zoneCards[Zone.Deck].Count != 0) {
				zoneCards[Zone.Deck][0].MoveTo(Zone.Hand);
			}
		}
		TestCardsStatus();
		SortZone(Zone.Hand);
		PackZone(Zone.Hand);
	}
//}
//{ test if game-state is valid
	public static bool IsLegal() {
		foreach (Card card in cards) {
			if (!card.IsLegal()) return false;
		}
		foreach (Resource resource in Resource.GetValues(typeof(Resource))) {
			if (resources[resource] < 0) return false;
		}
		return true;
	}
//}
//{ adding and removing resources
	public static Dictionary<Resource,int> resources = new Dictionary<Resource,int>();
	public static void BuildResources() {
		foreach(Resource resource in Resource.GetValues(typeof(Resource))) {
			resources[resource] = 0;
		}
	}
	public static void Add(Resource resource) {
		Add(resource,1);
	}
	public static void Remove(Resource resource) {
		Add(resource,-1);
	}
	public static void Remove(Resource resource,int quantity) {
		Add(resource,-quantity);
	}
	public static void Add(Resource resource,int quantity) {
		resources[resource] += quantity;
		Define.Counter(resource).Set(resources[resource]);
	}
//}
//{ track Cards in each Zone
	public static void NewZone(Card card) {
		// we don't know which zone the card was in previously
		foreach (var zone in zoneCards) {
			zone.Value.Remove(card);
		}
		// cards need to be added to the front (top) of the deck by default, rather than the back
		if (card.zone == Zone.Deck) {
			zoneCards[card.zone].Insert(0,card);
		} else {
			zoneCards[card.zone].Add(card);
		}
		// zones that receive a card need to be re-packed, but zones that lose a card don't
		zonePacked[card.zone] = false;
	}
	public static Dictionary<Zone,List<Card>> zoneCards = new Dictionary<Zone,List<Card>>();
	public static Dictionary<Zone,bool>      zonePacked = new Dictionary<Zone,bool>();
	public static void BuildZoneDict() {
		foreach(Zone zone in Zone.GetValues(typeof(Zone))) {
			zoneCards[zone] = new List<Card>();
			zonePacked[zone] = false;
		}
		foreach (Card card in cards) {
			zoneCards[card.zone].Add(card);
		}
	}
//}
//}
//{ other
//{ arrange cards in the Deck zone during editing
// beware of [ExecuteAlways] when editing MonoBehaviour functions
	public bool packDeckEditor;
	public static void PackDeckEditor() {
		if (cards.Length != 0) {
			Bounds[] boundsList = Packing.PackBounds(cards[0].GetComponent<Renderer>().bounds.size,Define.Bounds(Zone.Deck),cards.Length);
			for (int i=0;i<cards.Length;i++) {
				if (cards[i] != null) {
					boundsList[i].center = new Vector3(boundsList[i].center.x,Define.Bounds(Zone.Deck).max.y - boundsList[i].center.y + Define.Bounds(Zone.Deck).min.y,0);
					cards[i].transform.position = boundsList[i].center;
					cards[i].transform.localScale = Packing.GetLocalScale(boundsList[i],cards[i].gameObject);
				}
			}
		}
	}
//}
//{ utility functions
	public static void CheckDuplicateCards() {
		foreach (Card card in cards) {
			if (cards.Count(x=>x==card)>1) {
				Debug.LogError("Duplicate card in deck");
			}
		}
	}
	public void RestartGame() {
		ResetStaticVariables();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	public string nextLevel;
	public GameObject winScreen;
	public static void WinGame() {
		S.winScreen.SetActive(true);
	}
	public int wincount = 0;
//}
//{ static variables need to be reset on level load
	public static void ResetStaticVariables() {
		selecting=false;
		selector=null;
		cards=new Card[0];
		cardBounds.Clear();
		resources.Clear();
		zonePacked.Clear();
		zoneCards.Clear();
		S=null;
		AnimationQueue.Clear();
		requests.Clear();
		rng = new System.Random();
	}
//}
//}
}