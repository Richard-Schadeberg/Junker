using UnityEngine;
using UnityEditor;
// each level contains 1 Game object, that can be referenced by other objects
// Game contains all data that's not attached to a specific card
// eg. zone tracking, animation queue, resource counters
public class Game : MonoBehaviour {
	public int startingHandSize,startingTime;
// Game needs to be instantiated in order to work with the unity inspector,
//  but its functionality is otherwise static.
//  the solution is to store a static pointer to the instance, to allow
//  static methods to access the instance's properties.
	public static Game S;
	void Awake() {
		S = this;
	}
    // time (in seconds) between each animation fired from the queue
    public float chainTime = 0.2f;
    // tweening constants
 	public float vMax      = 12000;
	public float accTime   =  0.2f;
	public float decTime   =  0.4f;
	public float startDelay=  0.2f;
	// various singleton classes, will be expanded as they are added
	// these all contain data that needs to be reset on a new level (hence no static properties)
	public AnimationHandler animationHandler = new AnimationHandler();
	public ResourceTracker resourceTracker = new ResourceTracker();
	public DiscardRequester discardRequester = new DiscardRequester();
	public ZoneTracker zoneTracker;
	void Update() {
		animationHandler.Update();
	}
	void Start() {
		zoneTracker = new ZoneTracker(cards);
		ResourceTracker.Add(Resource.Time,startingTime);
		// move cards into deck to start
		foreach (Card card in cards) {
			animationHandler.AnimateInstant(card,GameAction.Repacking);
		}
		for (int i = 0; i < startingHandSize; i++)
		{
			DrawCard();
		}
		animationHandler.WaitSeconds(startDelay);
	}
	// only called at start of game
	// cards drawn later need to be attributed to another card so they can be reversed
	public void DrawCard() {
		Card drawnCard = zoneTracker.DrawCard();
		if (drawnCard==null) return;
		GameStateChanged();
		AnimationHandler.Animate(drawnCard,GameAction.Drawing);
	}
	// list of cards that start in the deck, from top to bottom
	public Card[] cards;
	public static Vector2 cardAspectRatio{get {return (Vector2)S.cards[0].gameObject.GetComponent<SpriteRenderer>().bounds.size;}}
	// call whenever something happens that affects whether a card can be played
	public static void GameStateChanged() {
		// no need to update playability, zone sorting, and card colours during temporary actions
		if (Game.S.ReversibleMode) return;
		CardPlayable.GameStateChanged();
		S.zoneTracker.GameStateChanged();
		foreach (Card card in S.cards) {
			card.SetColour();
		}
	}
	public bool ReversibleMode = false;
}