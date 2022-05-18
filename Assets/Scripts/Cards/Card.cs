using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Card : MonoBehaviour
{
	// design aspects
	public Resource[] inputs,outputs;
	public bool isTool,winsGame,singleUse,scaleable,discardAfterUse;
	public int partLimit=0;
	public int requiredPart=0;
	// MonoBehaviour functions
	void Start() {
		cardComponents.DisplayInputsOutputs(inputs,outputs);
		cardComponents.SetLayers(gameObject);
		cardComponents.cardName = cardName;
	}
	void Update() {
		if (currentAnimation != null) currentAnimation.Update();
	}
	void OnMouseUp() {
		ClickResponse(); 
	}
    private void OnMouseOver() {
		if (zone == Zone.Hand) cardComponents.MousedOver();
    }
    // Animation
    public CardAnimation currentAnimation = null;
	public Bounds bounds {
		get {
			ZoneTracker.PackZone(zone);
			return _bounds;
		} 
		set { _bounds = value;}
	} private Bounds _bounds;
	// input/output display and other cosmetic features
	public CardComponents cardComponents {get {return GetComponentInChildren<CardComponents>();}}
	public String cardName {get {return gameObject.name;}set {gameObject.name = value;}}
    void OnDrawGizmos() {
		cardComponents.DrawGizmos(inputs,outputs,cardName);
    }
	public void UpdateColour() {
		// no need to change colour for temporary actions
		if (Game.S.ReversibleMode) return;
		gameObject.GetComponent<SpriteRenderer>().color = Colour();
	}
	public void UpdateInOutDarkness() {
		// no need to change input darkness for temporary actions
		if (Game.S.ReversibleMode) return;
		cardComponents.UpdateInOutDarkness(inputs,Playability==Playability.Playable,zone);
    }
	Color Colour() {
		if (zone!=Zone.Hand) return Define.Colour(Playability.Playable,isTool);
		if (selected)        return Define.Colour(true);
		if (selectable)      return Define.Colour(false);
		else                 return Define.Colour(Playability,isTool);
	}
	public Playability Playability {
		get {
			if (!CardPlayable.isValid) CardPlayable.EvaluatePlayability();
			return _Playability;
		}
		set {_Playability = value;}
	} private Playability _Playability;
	// sorting in hand
	public int Priority {
		get {
			return CardPriority.Priority(this);
		}
	}
	// rules engine
	public Zone zone = Zone.Deck;
	void ClickResponse() {
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
	// determine whether the card is in a legal state at this time
	public virtual bool IsLegal() {
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
	public void ClearSelectable() {
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
	// reversible actions
	public Credits credits = new Credits();
	// scaleable
	public bool isCopy;
	public Card tempCopy;
	// consuming outputs causes those output icons to dim
	public HashSet<ResourceIcon> consumedIcons = new HashSet<ResourceIcon>();
}
