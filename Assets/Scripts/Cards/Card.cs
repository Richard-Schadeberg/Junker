using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
// stores core data for parts in-game
public class Card : MonoBehaviour {
	// design aspects
	public Resource[] inputs;
	public Resource[] outputs;
	// tools can't be discarded and are always in your starting hand
	// singleUse parts discard themselves after use
	// scaleable cards create a tool copy of themselves whenever they are played
	// they can be played any number of times as long as you can pay their inputs
	public bool isTool,winsGame,singleUse,scaleable;
	// some parts restrict the number of parts you can play this turn (0 for no restriction)
	public int partLimit=0;
	// some parts can only be installed as the nth part of the turn (0 for no restriction)
	public int requiredPart=0;
	// MonoBehaviour functions
	public virtual void Start() {
		cardComponents.DisplayInputsOutputs(inputs,outputs);
		// make cards draw fully on top of each other in order to appear as single objects
		cardComponents.SetLayers(gameObject);
		cardComponents.cardName = cardName;
	}
	public void OnMouseUp() {
		if (selectable) {
			if (selected) UnSelect(); else Select();
		} else {
			switch (zone) {
				case Zone.Hand:
					CardInstall.TryInstall(this);
					break;
				case Zone.Play:
					CardInstall.Uninstall(this);
					break;
			}
		}
	}
	void Update() { if (currentAnimation != null) currentAnimation.UpdatePositionAndSize(); }
	void OnDrawGizmos() { cardComponents.DrawGizmos(inputs, outputs, cardName); }
	// Animation
	public CardAnimation currentAnimation = null;
	// find out where the card would be if the current gamestate was represented visually
	public Bounds goalBoundsForCurrentGamestate {
		get {
			ZoneTracker.PackZone(zone);
			return _goalBoundsForCurrentGamestate;
		} 
		set { _goalBoundsForCurrentGamestate = value;}
	} 
	private Bounds _goalBoundsForCurrentGamestate;
	// input/output display and other cosmetic features
	public CardComponents cardComponents {get {return GetComponentInChildren<CardComponents>();}}
	// cardName is a synonym for unity's object name, to make calls shorter and help with debugging
	public String cardName {get {return gameObject.name;}set {gameObject.name = value;}}
	public void UpdateColour() {
		// no need to change colour for temporary actions
		if (Game.S.ReversibleMode) return;
		gameObject.GetComponent<SpriteRenderer>().color = CurrentColour();
	}
	Color CurrentColour() {
		// Cards not in hand are fully lit, tools are still purple
		if (zone != Zone.Hand) return Define.ColourFromPlayability(Playability.Playable, isTool);
		// Colour for discard selection overrides other colouring
		if (selected) return Define.ColourFromSelectable(true);
		if (selectable) return Define.ColourFromSelectable(false);
		// Colour cards in hand according to how soon they can be played. Tools are purple.
		else return Define.ColourFromPlayability(Playability, isTool);
	}
	public void UpdateInOutDarkness() {cardComponents.UpdateInOutDarkness(inputs,Playability==Playability.Playable,zone);}
	// Whether the player can pay for the part
	public Playability Playability {
		get {
			CardPlayable.EvaluatePlayability();
			return _Playability;
		}
		set {_Playability = value;}
	} private Playability _Playability;
	public Zone zone = Zone.Deck;
	// determine whether the card is in a legal state at this time
	public bool IsLegal() {
		// enforce part limit
		if (partLimit>0 && zone==Zone.Play && ZoneTracker.GetCards(Zone.Play).Length > partLimit) return false;
		// enforce required part (ie. ground sonar)
		if (requiredPart>0 && zone==Zone.Play && ZoneTracker.GetCards(Zone.Play).Length < requiredPart) return false;
		if (requiredPart>0 && zone==Zone.Play && ZoneTracker.GetCardsLeftToRight(Zone.Play)[requiredPart-1] != this) return false;
		return true;
	}
	// selection for discard
	public bool selectable = false;
	public bool selected   = false;
	public void MakeSelectable() {
		if (isTool) return;
		// no selecting cards during temporary actions
		if (Game.S.ReversibleMode) return;
		selectable = true;
		selected   = false;
		UpdateColour();
	}
	public void MakeUnselectable() {
		selected   = false;
		selectable = false;
		UpdateColour();
	}
	void Select() {
		selected = true;
		DiscardRequester.Select();
		UpdateColour();
	}
	void UnSelect() {
		selected = false;
		DiscardRequester.CancelSelect();
		UpdateColour();
	}
	// Reversible draw and discard
	public Credits credits = new Credits();
	// Scaleable
	public bool isCopy;
	public bool hasCopy { get { return (tempCopy != null); } }
	public Card tempCopy;
	// Consuming outputs causes those output icons to dim
	// Track them so that they can be brightened again if the resources are returned
	public HashSet<ResourceIcon> consumedIcons = new HashSet<ResourceIcon>();
	// Track if the card has converted batteries into electrics
	public int batteryConversions = 0;
}
