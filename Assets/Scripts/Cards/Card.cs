using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.Linq;

public class Card : MonoBehaviour
{
	public String cardName; // textBox set to match cardName in editor
	public Resource[] inputs,outputs;
	public bool winsGame,startsHand,singleUse,noDiscard,scaleable;
	public int winCount;
	public void CopyDesign(Card card) { // utility function for editor
		cardName = card.cardName;
		inputs = card.inputs;
		outputs = card.outputs;
		winsGame = card.winsGame;
		startsHand = card.startsHand;
		singleUse = card.singleUse;
		noDiscard = card.noDiscard;
		scaleable = card.scaleable;
		winCount = card.winCount;
		art.GetComponent<SpriteRenderer>().sprite = card.art.GetComponent<SpriteRenderer>().sprite;
		cardComponents.descriptionBox.text = card.cardComponents.descriptionBox.text;
	}
	CardAnimation currentAnimation;
//{ necessities
//{ MonoBehaviour functions
	void Start() {
		cardComponents = GetComponentInChildren<CardComponents>();
		cardComponents.DisplayInputsOutputs(inputs,outputs);
		cardComponents.SetLayers(gameObject);
	}
	
	void Update() {
		if (currentAnimation != null) currentAnimation.Update();
		// if (animating) TravelRoute();
	}
	
	void OnMouseUp() {
		if (!animating||zone!=oldZone) {
			if (selectAble) {
				if (selected) {
					SetColour(Define.S.selectableColour);
					selected = false;
					Game.UnSelect(this);
				} else {
					SetColour(Define.S.selectedColour);
					selected = true;
					Game.Select(this);
				}
			} else {
				switch (zone) {
					case Zone.Hand:
						TryInstall();
						break;
					case Zone.Play:
						Uninstall();
						break;
				}
			}
		}
	}
//}
//}
//{ design elements


//}
//{ Game logic
//{ card's zone (hand,deck,play etc.)
	// [HideInInspector]
	public Zone zone = Zone.Deck;
	
	public void MoveTo(Zone newZone) {
		if (!noDiscard && newZone == Zone.Hand) {
			Game.Add(Resource.Card);
		}
		if (!noDiscard && zone == Zone.Hand) {
			Game.Remove(Resource.Card);
		}
		zone = newZone;
		Game.NewZone(this);
	}
//}
//{ installing/uninstalling
	private bool finishedInstall=false;
	private void TryInstall() {
		if (CanInstall()&&!Game.selecting) { // no installing parts during card selection
			MoveTo(Zone.Play);
			Input();
			Game.QueueAnimation(this);
			finishedInstall=false;
			if (Game.requests.Count==0) {// pause installing the part if there are pending requests from input/output
				FinishInstall();
			}
			Game.TestCardsStatus();
		}
	}
	private void FinishInstall() {
		finishedInstall=true;
		Output();
		if (scaleable) MakeCopy();
		if (winsGame) GameWin();
		AfterInstall();
		Game.TestCardsStatus();
	}
	protected virtual void GameWin() {
		if (winCount == 0) {
			Game.WinGame();
		} else {
			Game.S.wincount++;
			if (Game.S.wincount >= winCount) {
				Game.WinGame();
			}
		}
	}
	protected virtual void AfterInstall() {}
	
	private void Uninstall() { // Uninstall always succeeds
		int i = Game.zoneCards[Zone.Play].IndexOf(this); // first, uninstall the part installed after this
		if (i+1<Game.zoneCards[Zone.Play].Count) {
			Game.zoneCards[Zone.Play][i+1].Uninstall();
		}
		selections.Clear();
		if (finishedInstall) {
			if (scaleable) DestroyCopy();
			UndoOutput();
			finishedInstall=false;
		}
		Game.QueueAnimation(this);
		UndoInput();
		UndoDiscards();
		MoveTo(Zone.Hand);
		AfterUninstall();
		Game.TestCardsStatus();
		if (winCount > 0) Game.S.wincount--;
	}
	protected virtual void AfterUninstall() {}
	
	// install the card, see if it's legal before outputting, then uninstall it
	// this means that input(), output(), and moveto() need to be fully reversible
	public bool CanInstall() { 
		// since this moves the card to play and back without animating it,
		// we don't want the game to think it needs to re-pack a zone
		var oldPacking = Game.zonePacked;
		MoveTo(Zone.Play);
		Input();
		bool isLegal = Game.IsLegal();
		UndoInput();
		MoveTo(Zone.Hand);
		Game.zonePacked = oldPacking;
		return isLegal;
	}
	// like CanInstall(), but it completely installs another card first
	public bool CanInstallWith(Card card) {
		// since this moves the card to play and back without animating it,
		// we don't want the game to think it needs to re-pack a zone
		var oldPacking = Game.zonePacked;
		card.MoveTo(Zone.Play);
		card.Input();
		// if the first card needs to discard the second, then it's not possible
		// relevant if the first card discards & draws cards
		Game.Remove(Resource.Card);
		bool possible = Game.IsLegal();
		Game.Add(Resource.Card);
		
		card.Output();
		bool isLegal = CanInstall()&&possible;
		card.UndoOutput();
		card.UndoInput();
		card.MoveTo(Zone.Hand);
		Game.zonePacked = oldPacking;
		return isLegal;
	}
	public virtual bool IsLegal() {return true;}
//}
//{ returning without uninstalling (due to Clock)
	protected bool returning=false;
	public void Return() {
		if (!isCopy) MoveTo(Zone.Hand);
		Game.QueueAnimation(this);
		draws.Clear();
		discards.Clear();
		conversions.Clear();
		if (scaleable) DestroyCopy();
		if (singleUse) MoveTo(Zone.Junk);
		returning=true;
		AfterReturn();
	}
	protected virtual void AfterReturn() {}
//}
//{ inputting and outputting as reversible functions
// calling Input(), Output(), UndoOutput(), and UndoInput() back-to-back should have no effect on game's appearance
	private void Input() {
		BeforeInput();
		foreach (Resource input in inputs) {
			Game.Remove(input);
			switch (input) {
				case Resource.Card:
					Game.RequestSelection(Zone.Hand,this);
					break;
				case Resource.Electric:
					if (Game.resources[input]<0) {
						Game.Add(input);
						Game.Remove(Resource.Battery);
						CreditConversion(input);
					}
					break;
				case Resource.Heat:
					if (Game.resources[input]<0) {
						Game.Add(input);
						Game.Remove(Resource.Coal);
						CreditConversion(input);
					}
					break;
			}
		}
	}
	protected virtual void BeforeInput() {}
	
	private void UndoInput() {
		foreach (Resource input in inputs) {
			Game.Add(input);
			if (input == Resource.Card) {
				Game.UndoLastRequest();
			}
		}
		UndoConversions();
		AfterUndoInput();
	}
	protected virtual void AfterUndoInput() {}
	
	private void Output() {
		foreach (Resource output in outputs) {
			Game.Add(output);
			if (output == Resource.Card) {
				Game.Remove(Resource.Card);
				if (Game.zoneCards[Zone.Deck].Count!=0) {
					Card top = Game.zoneCards[Zone.Deck][0];
					top.MoveTo(Zone.Hand);
					Game.QueueAnimation(top);
					CreditDraw(top);
				}
			}
		}
		AfterOutput();
	}
	protected virtual void AfterOutput() {}
	
	private void UndoOutput() {
		BeforeUndoOutput();
		foreach (Resource output in outputs) {
			Game.Remove(output);
			if (output==Resource.Card) Game.Add(output);
		}
		UndoDraws();
	}
	protected virtual void BeforeUndoOutput() {}
//}
//{ discard/draw/conversion as reversible actions
	// when a difficult to reverse action happens, it is recorded
	// this allows it to be manually reversed when the card is uninstalled
	// note the CanInstall() won't cause discards or conversions, so they won't always need to be reversed
	// if the deck is empty, draws won't happen and therefore won't need to be reversed
	public Stack<Card>     draws       = new Stack<Card>();
	public Stack<Card>     discards    = new Stack<Card>();
	public Stack<Resource> conversions = new Stack<Resource>();
	public void CreditDraw(Card card) {
		draws.Push(card);
	}
	public void CreditDiscard(Card card) {
		discards.Push(card);
	}
	public void CreditConversion(Resource resource) {
		conversions.Push(resource);
	}
	public void UndoDraws() {
		while (draws.Count != 0) {
			Card card = draws.Pop();
			card.MoveTo(Zone.Deck);
			Game.QueueAnimation(card);
		}
	}
	public void UndoDiscards() {
		while (discards.Count != 0) {
			Card card = discards.Pop();
			card.MoveTo(Zone.Hand);
			Game.QueueAnimation(card);
			Game.Remove(Resource.Card);
		}
	}
	private void UndoConversions() {
		while (conversions.Count != 0) {
			switch (conversions.Pop()) {
				case (Resource.Electric):
					Game.Remove(Resource.Electric);
					Game.Add(Resource.Battery);
					break;
				case (Resource.Heat):
					Game.Remove(Resource.Heat);
					Game.Add(Resource.Coal);
					break;
			}
		}
	}
//}
//{ scaleable
// scaleable parts add a copy of themselves to your hand when installed
// this copy can't be discarded and disappears after the clock is played
	[HideInInspector]
	public bool isCopy=false;
	private Card copyCard=null;
	private void MakeCopy() {
		copyCard = Instantiate(this);
		copyCard.noDiscard = true;
		copyCard.isCopy = true;
		if (!isCopy) {
			copyCard.cardComponents.descriptionBox.text += "\n<i>Temporary copy</i>";
		}
		List<Card> gameCards = Game.cards.ToList();
		gameCards.Add(copyCard);
		Game.cards = gameCards.ToArray();
		copyCard.MoveTo(Zone.Hand);
		if (oldZone==Zone.Play) Game.QueueAnimation(copyCard);
	}
	private void DestroyCopy() {
		if (copyCard!=null) {
			copyCard.DestroyCopy();
			copyCard.MoveTo(Zone.Junk);
			Game.QueueAnimation(copyCard);
			copyCard=null;
		}
	}
	private void DestroySelf() {
		Game.zoneCards[zone].Remove(this);
		List<Card> gameCards = Game.cards.ToList();
		gameCards.Remove(this);
		Game.cards = gameCards.ToArray();
		Destroy(gameObject);
	}
//}
//{ using cards the game has selected
// during input/output, request selections from the game
// here, we use the selections the game gives us
	private List<Card> selections = new List<Card>();
	public void GiveSelection(Card card) {
		selections.Add(card);
	}
	public void LoseSelection(Card card) {
		selections.Remove(card);
	}
	public void ProcessSelections() {
		BeforeProcessSelections();
		while (selections.Count > 0) {
			selections[0].MoveTo(Zone.Junk);
			CreditDiscard(selections[0]);
			Game.QueueAnimation(selections[0]);
			selections.RemoveAt(0);
			Game.Add(Resource.Card);
		}
		FinishInstall();
	}
	protected virtual void BeforeProcessSelections() {}
	public bool selectAble=false;
	public bool selected=false;
	public void EndSelection() {
		SetColour(Define.Colour(status));
		selectAble=false;
		selected = false;
	}
	public void MakeSelectable() {
		if (noDiscard&&zone==Zone.Hand) return;
		if (!selected) {
			SetColour(Define.S.selectableColour);
			selectAble=true;
		}
	}
//}
//}
//{ user interface/display
//{ set colour
	public void SetColour(Color color) {
		GetComponent<SpriteRenderer>().color = color;
	}
//}
//{ Animation between zones
	[HideInInspector]
	public bool animating = false;
	private Bounds origin,goal;
	private float startTime,tweenTime;
	[HideInInspector]
	public Zone oldZone = Zone.Deck;
	public void AnimateInstant() {
		SetBounds(Game.cardBounds[this]);
	}
	public void Animate() {
		if (zone==Zone.Deck&&oldZone==Zone.Deck) {
			if (Game.zoneCards[Zone.Deck].IndexOf(this)==0) {
				foreach (Card card in Game.zoneCards[Zone.Deck]) {
					if (card!=this) {
						card.Animate();
					}
				}
			}
		}
		// if animating to a new zone, animate cards in the new zone out of the way
		if (zone != oldZone) {
			if (oldZone==Zone.Deck) {
				Game.PackZone(Zone.Deck);
				if (Game.zoneCards[Zone.Deck].Count!=0) {
					Game.QueueAnimation(Game.zoneCards[Zone.Deck][0]);
				}
			}
			Game.SortZone(zone);
			Game.PackZone(zone);
			oldZone=zone;
			foreach (Card card in Game.zoneCards[zone]) {
				if (card.zone==card.oldZone) {
					card.Animate();
				}
			}
		}
		Game.PackZone(zone);
		// if it's already doing the queued animation, don't interrupt it
		if (goal != Game.cardBounds[this]) {
			goal  = Game.cardBounds[this];
			origin = GetComponent<Renderer>().bounds;
			startTime = Time.time;
			
			currentAnimation = new CardAnimation(origin,goal,MotionAction.Drawing,this,FinishAnimation);

			motionPlan = new MotionPlan(origin.center,goal.center,MotionAction.Drawing);
			tweenTime = Tween.timeRequired(vMax/accTime/motionPlan.distance,vMax/decTime/motionPlan.distance,vMax/motionPlan.distance);
			animating = true;
		}
	}
	MotionPlan motionPlan;
	const float vMax      = 12000;
	const float accTime   =  0.2f;
	const float decTime   =  0.4f;
	const float chainTime =  0.2f;
	// called during Update() to set the card's position
	protected void TravelRoute() {
		float percent = Tween.percentAtTime(vMax/accTime/motionPlan.distance,vMax/decTime/motionPlan.distance,vMax/motionPlan.distance,Time.time-startTime);
		if (Single.IsNaN(percent)||motionPlan.distance==0) {
			percent = 1.0f;
			FinishAnimation();
		}
		SetBounds(new Bounds(TravelPoint(origin.center,goal.center,percent),origin.size+percent*(goal.size-origin.size)));
	}
	private void FinishAnimation() {
		returning=false;
		animating=false;
		if (zone==Zone.Junk&&isCopy) 		DestroySelf();
		if (zone==queueZone&&queueOther)	QueuedOther();
		if (queueWin) {};
	}
	public bool animatingStart() {
		return (animating&&Time.time-startTime<chainTime);
	}
	private void SetBounds(Bounds bounds) {
		transform.position = bounds.center;
		transform.localScale = Packing.GetLocalScale(bounds,gameObject);
	}
	private bool queueWin;
	protected bool queueOther=false;
	protected Zone queueZone;
	protected virtual void QueuedOther() {}
	
	
	private float TravelDistance(Vector2 origin,Vector2 destination) {
		MotionPlan motionPlan = new MotionPlan(origin,destination,MotionAction.Drawing);
		return motionPlan.distance;
		// if (zone==Zone.Hand&&returning&&origin.y>destination.y) {
		// 	if (origin.x>destination.x) {
		// 		return Tween.ReverseArcLength(origin,destination);
		// 	} else {
		// 		return Tween.ReverseHookLength(origin,destination);
		// 	}
		// }
		// if (zone==Zone.Play&&origin.y<destination.y) {
		// 	if (origin.x>destination.x) {
		// 		return Tween.ArcLength(origin,destination);
		// 	} else {
		// 		return Tween.HookLength(origin,destination);
		// 	}
		// }
		// return Tween.LineLength(origin,destination);
	}
	private Vector2 TravelPoint(Vector2 origin,Vector2 destination,float percentage) {
		return MotionPlanPercent.LocationAtPercentage(motionPlan,percentage);
	}
//}
//{ populate card with input/output icons when the scene starts
	private Stack<GameObject> icons = new Stack<GameObject>();
	public void BuildCard() {
		while (icons.Count()>0) { // delete extra icon objects from last BuildCard()
			Destroy(icons.Pop());
		}
		int iconNum = 0;
		foreach (Resource input in inputs) {
			if (iconNum<maxInputs) { // sometimes there's a 10th input that needs to not be rendered
				icons.Push(MakeIcon(input,iconNum));
			}
			iconNum++;
		}
		iconNum = 9;
		foreach (Resource output in outputs) {
			if (iconNum<(maxInputs+maxOutputs)) { // sometimes there's a 10th output that needs to not be rendered
				icons.Push(MakeIcon(output,iconNum));
			}
			iconNum++;
		}
		if (cardComponents.nameBox != null) {
			cardComponents.nameBox.SetText(cardName);
		}
		SetLayers();
	}
	public Canvas canvas;
	public SpriteRenderer art;
	public void SetLayers() {
		int layer = 10+(int)Math.Abs(gameObject.GetInstanceID()) % 32000;
		// give each instance its own layer
		GetComponent<Renderer>().sortingOrder=layer;
		canvas.sortingOrder=layer+1;
		art.sortingOrder=layer+1;
		foreach (GameObject icon in icons) {
			icon.GetComponent<Renderer>().sortingOrder=layer+1;
		}
	}
	private GameObject MakeIcon(Resource resource,int iconNum) {
		// Vector2 iconSize = cardComponents.GetIconSize();
		// Vector2 iconCentre = cardComponents.GetIconCentre(iconNum,iconSize);
		// GameObject newIcon = Instantiate(iconPrefab,iconCentre,Quaternion.identity);
		GameObject newIcon = Instantiate(iconPrefab);
		// SpriteRenderer spriter = newIcon.GetComponent<SpriteRenderer>();
		// spriter.sprite = Define.Sprite(resource);
		// Vector2 spriteSize = spriter.bounds.size;
		// spriter.transform.localScale = iconSize/spriteSize;
		// spriter.transform.SetParent(transform);
		return newIcon;
	}
//}
//{ Gizmos to preview inputs/outputs in editor
// Doesn't work without a Define object or properly set resource boxes
	public GameObject iconPrefab;
	public CardComponents cardComponents;
	private const int grouping = 3;
	private const int maxInputs = 9;
	private const int maxOutputs = 9;
    void OnDrawGizmos() {
		if (cardComponents==null) {
			cardComponents = GetComponentInChildren<CardComponents>();
		}
		if (cardComponents.resourceBoxes.Length==(maxInputs+maxOutputs)/grouping&&Define.S!=null) cardComponents.DrawGizmos(inputs,outputs,cardName);
    }
//}
//{ check where in the hand a card needs to be displayed
	// wincons and clock are always on the left, because they never leave the hand
	// regular cards are sorted by how soon they can be played
	// undiscardable cards go on the left, because they leave the hand less often
	// within that, simpler cards are on the left
	// InstanceID is used as the last sorter to avoid reshuffling cards due to ties
	[HideInInspector]
	public Status status = Status.Playable;
	public int Priority() {
		int[] factors = new int[8];
		factors[7]=(winsGame?0:1);
		factors[6]=(inputs.Contains(Resource.Time)?0:1);
		factors[5]=(noDiscard?0:1);
		factors[4]=(int)status;
		factors[3]=inputs.Length;
		factors[2]=outputs.Length;
		factors[1]=new []{singleUse,startsHand,scaleable}.Count(x=>x);
		factors[0]=(int)Math.Abs(gameObject.GetInstanceID()%100);
		int priority=0;
		int significance=1;
		foreach(int x in factors) {
			priority += significance*x;
			significance *= (int)Math.Pow(2,(Math.Floor((float)30/factors.Length)));
		}
		return (priority);
	}
//}
//}
}
