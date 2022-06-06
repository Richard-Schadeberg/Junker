using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
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
	void Awake() {S = this;}
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
	public IconTracker iconTracker = new IconTracker();
	public ZoneTracker zoneTracker;
	public Clock clock = new Clock();
	void Update() {
		animationHandler.StepAnimations();
	}
	void Start() {
		clock.SetTurnsRemaining(startingTime);
		if (shuffle) cards.Shuffle();
		BringToolsToTop();
		zoneTracker = new ZoneTracker(cards);
		// move cards into deck to start
		foreach (Card card in cards) {
			animationHandler.AnimateInstant(card,GameAction.Repacking);
		}
		GameActions.DrawCards(startingHandSize,null);
		animationHandler.PauseAnimations(startDelay);
	}
	// if calling this on an ordered deck, make sure the tools are already at the top
	private void BringToolsToTop() {
		// index of top of non-tool deck, moves down deck as tools are shuffled to top
		int swap = 0;
		for (int i=0; i<cards.Length; i++) {
			// swap tool with highest non-tool card
			// if the top card is a tool, a meaningless swap will occur and the swap destination will move down
			// this prevents swap from moving a tool lower into the deck
			if (cards[i].isTool) {
				Card temp = cards[i];
				cards[i] = cards[swap];
				cards[swap] = temp;
				swap++;
            }
        }
    }
	// list of cards that start in the deck, from top to bottom
	public Card[] cards;
	// if the deck is not shuffled, make sure tools are already at top to avoid unusual motion 
	public bool shuffle;
	public static Vector2 cardAspectRatio{get {return (Vector2)S.cards[0].gameObject.GetComponent<SpriteRenderer>().bounds.size;}}
	// call whenever something happens that affects whether a card can be played
	public static void PlayerActionResolved() {
		// no need to update playability, zone sorting, and card colours during temporary actions
		if (Game.S.ReversibleMode) return;
		CardPlayable.PlayerActionResolved();
		S.zoneTracker.GameStateChanged();
		ResourceCounter.UpdateCounters();
		foreach (Card card in S.cards) {
			card.UpdateColour();
			card.UpdateInOutDarkness();
		}
	}
	public static void StartReversibleMode() {
		S.ReversibleDepth++;
    }
	public static void EndReversibleMode() {
		S.ReversibleDepth--;
    }
	public bool ReversibleMode {get {return (ReversibleDepth>0);}}
	private int ReversibleDepth;
}