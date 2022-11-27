using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
// each level contains 1 Game object, that can be referenced by other objects
// Game contains all data that's not attached to a specific card
// eg. zone tracking, animation queue, resource counters
public class Game : MonoBehaviour {
	public int startingHandSize,startingTurns;
//  Game needs to be instantiated in order to work with the unity inspector,
//  but its functionality is otherwise static.
//  The solution is to store a static pointer to the instance, to allow
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
	// these all contain data that needs to be reset on a new level (hence cannot be static)
	public AnimationHandler animationHandler = new AnimationHandler();
	public ResourceTracker resourceTracker = new ResourceTracker();
	public DiscardRequester discardRequester = new DiscardRequester();
	public IconTracker iconTracker = new IconTracker();
	public ZoneTracker zoneTracker;
	public Clock clock = new Clock();
	void Update() {animationHandler.TryFireQueuedAnimation();}
	void Start() {
		clock.SetTurnsRemaining(startingTurns);
		if (shuffleDeckAtStart) cards.Shuffle();
		BringToolsToTop();
		zoneTracker = new ZoneTracker(cards);
		// move cards into deck to start
		foreach (Card card in cards) {animationHandler.AnimateInstant(card,GameAction.Repacking);}
		GameActions.DrawCards(startingHandSize,null);
		// calculate derived information
		// starting the level is technically a player action
		PlayerActionResolved();
		// wait a few seconds after the level loads before starting animations
		animationHandler.PauseAnimations(startDelay);
	}
	// if calling this on an ordered deck, make sure the tools are already at the top
	// does not introduce bias to a shuffled deck (besides moving tools to top)
	private void BringToolsToTop() {
		// index of top of non-tool deck, moves down deck as tools are shuffled to top
		int swap = 0;
		for (int i=0; i<cards.Length; i++) {
			// swap tool with highest non-tool card
			// if the top card is already a tool, a meaningless swap will occur and the swap destination will move down
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
	// note that tools will move to top even if no shuffle happens
	public bool shuffleDeckAtStart;
	// utility for animations class
	public static Vector2 cardAspectRatio{get {return (Vector2)S.cards[0].gameObject.GetComponent<SpriteRenderer>().bounds.size;}}
	// called whenever the gamestate has changed and won't be changing again until the player takes an action
	// this function flags all derived information as invalid, since the gamestate has changed since it was derived
	public static void PlayerActionResolved() {
		// should not be called while doing temporary actions
		if (Game.S.ReversibleMode) throw new Exception("PlayerActionResolved() but game is still in reversibleMode");
		// card playability is derived and no longer valid
		CardPlayable.isValid = false;
		// zoneTracker derives hand ordering
		S.zoneTracker.PlayerActionResolved();
		// counter displays need to update to reflect possibly new resource counts
		ResourceCounter.UpdateCounters();
		// card visuals need to update to reflect new gamestate
		foreach (Card card in S.cards) {
			card.UpdateColour();
			card.UpdateInOutDarkness();
		}
	}
	// ReversibleMode prevents various knock-on effects like visual changes or discard requesting from triggering during CanInstall()
	public bool ReversibleMode { get { return (ReversibleDepth > 0); } }
	private int ReversibleDepth = 0;
	public static void StartReversibleMode() {S.ReversibleDepth++;}
	public static void EndReversibleMode() {S.ReversibleDepth--;}
}