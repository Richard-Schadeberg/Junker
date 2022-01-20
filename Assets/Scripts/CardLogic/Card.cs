using System;
using UnityEngine;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
	public String cardName;
	public Resource[] inputs,outputs;
	public bool winsGame,startsHand,singleUse,noDiscard,scaleable;
	public int partLimit=0;
	public int requiredPart=0;
	public CardAnimation currentAnimation = null;
	public CardComponents cardComponents {get {return GetComponentInChildren<CardComponents>();}}
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
	public Zone zone = Zone.Deck;
	public Bounds finalHandBounds;
	public Bounds bounds {
		get {
			if (!ZoneTracker.ZonePacked(zone)) ZoneTracker.PackZone(zone);
			if (zone==Zone.Hand) finalHandBounds = _bounds;
			return _bounds;
		} 
		set { _bounds = value;}
	} private Bounds _bounds;
	public Playability Playability {
		get {
			if (!CardPlayable.isValid) CardPlayable.EvaluatePlayability();
			return _Playability;
		}
		set {_Playability = value;}
	} private Playability _Playability;
	public int Priority {
		get {
			return CardPriority.Priority(this);
		}
	}
    void OnDrawGizmos() {
		cardComponents.DrawGizmos(inputs,outputs,cardName);
    }
	public bool ImmediatelyPlayable() {
		return CardInstall.CanInstall(this);
	}
	public bool PlayableWith(Card card) {return false;}
	void ClickResponse() {
		if (selectable) {
			if (selected) UnSelect(); else Select();
			SetColour();
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
	public virtual bool IsValid() {return true;}
	public void SetColour() {
		if (Game.S.ReversibleMode) return;
		gameObject.GetComponent<SpriteRenderer>().color = Colour();
	}
	Color Colour() {
		if (zone!=Zone.Hand) return Define.Colour(Playability.Playable);
		if (selected)        return Define.Colour(true);
		if (selectable)      return Define.Colour(false);
		else                 return Define.Colour(Playability);
	}
	public bool selectable = false;
	public bool selected   = false;
	public void MakeSelectable() {
		if (noDiscard) return;
		if (Game.S.ReversibleMode) return;
		selectable = true;
		selected   = false;
		Colour();
		return;
	}
	public void ClearSelectable() {
		selected   = false;
		selectable = false;
		Colour();
	}
	void Select() {
		selected = true;
		DiscardRequest.Select();
	}
	void UnSelect() {
		selected = false;
		DiscardRequest.CancelSelect();
	}
	public Credits credits = new Credits();
}
