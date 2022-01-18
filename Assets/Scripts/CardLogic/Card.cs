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
	public int partLimit=0;
	public int requiredPart=0;
	public CardAnimation currentAnimation;
	CardComponents cardComponents;
	void Start() {
		cardComponents = GetComponentInChildren<CardComponents>();
		cardComponents.DisplayInputsOutputs(inputs,outputs);
		cardComponents.SetLayers(gameObject);
	}
	void OnMouseUp() {
		ClickResponse();
	}
	public Zone zone = Zone.Deck;
	public Bounds bounds {
		get {
			if (!Game.S.zoneTracker.ZonePacked(zone)) Game.S.zoneTracker.PackZone(zone);
			return _bounds;
		} 
		set { _bounds = value;}
	} private Bounds _bounds;
	public Status Status {
		get {
			if (!CardPlayable.isValid) CardPlayable.EvaluatePlayability();
			return _Status;
		}
		set {_Status = value;}
	} private Status _Status;
	public int Priority {
		get {
			return CardPriority.Priority(this);
		}
	}
    void OnDrawGizmos() {
		if (cardComponents==null) {
			cardComponents = GetComponentInChildren<CardComponents>();
		}
		cardComponents.DrawGizmos(inputs,outputs,cardName);
    }
	public bool ImmediatelyPlayable() {
		Game.S.ReversibleMode = true;
		Game.S.ReversibleMode = false;
		return false;
	}
	public bool PlayableWith(Card card) {return false;}
	void ClickResponse() {
		switch (zone) {
			case Zone.Hand:
				CardInstall.Install(this);
				break;
		}
	}
}
