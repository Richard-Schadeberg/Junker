using System;
using UnityEngine;

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
	public Bounds bounds {
		get {
			if (!ZoneTracker.ZonePacked(zone)) ZoneTracker.PackZone(zone);
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
		Game.S.ReversibleMode = true;
		Game.S.ReversibleMode = false;
		return false;
	}
	public bool PlayableWith(Card card) {return false;}
	void ClickResponse() {
		switch (zone) {
			case Zone.Hand:
				CardInstall.TryInstall(this);
				break;
			case Zone.Play:
				CardInstall.Uninstall(this);
				break;
		}
	}
	public virtual bool IsValid() {return true;}
	public void SetColour(Color colour) {}
}
