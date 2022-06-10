using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// allows many basic game aspects to be defined in the editor
// eg. sprites, pointers to UI objects, colours etc.
public class Define : MonoBehaviour {
	// create static pointer to self to allow static methods to reference properties for the inspector's sake
	public static Define _S;
	public static Define S{get {
		if (_S!=null) return _S;
		_S = FindObjectOfType<Define>();
		return _S;
	} set {_S = value;}}
	void Awake() {
		S = this;
	}
	// convert Resource to Sprite
	public Sprite 
		noneSprite,
		cardSprite,
		batterySprite,
		electricSprite,
		fuelSprite,
		scrapSprite,
		metalSprite,
		distanceSprite,
		reconSprite,
		timeSprite;
	public static Sprite SpriteFromResource(Resource resource) {
		switch (resource) {
			case Resource.None:
				return S.noneSprite;
			case Resource.Card:
				return S.cardSprite;
			case Resource.Battery:
				return S.batterySprite;
			case Resource.Electric:
				return S.electricSprite;
			case Resource.Fuel:
				return S.fuelSprite;
			case Resource.Metal:
				return S.metalSprite;
			case Resource.Distance:
				return S.distanceSprite;
			case Resource.Recon:
				return S.reconSprite;
			default:
				return null;
		}
	}
	//convert Zone to Bounds
	public ZoneObj
		deckZone,
		handZone,
		playZone,
		junkZone;
	public static Bounds BoundsFromZone(Zone zone) {
		return ZoneObjFromZone(zone).GetComponent<SpriteRenderer>().bounds;
	}
	public static ZoneObj ZoneObjFromZone(Zone zone) {
		switch (zone) {
			case Zone.Deck:
				return S.deckZone;
			case Zone.Hand:
				return S.handZone;
			case Zone.Play:
				return S.playZone;
			case Zone.Junk:
				return S.junkZone;
			default:
				return null;
		}
	}
	// convert Playability to Color
	public Color
		playableColour,
		almostColour,
		unplayableColour,
		playableColourTool,
		almostColourTool,
		unplayableColourTool;
	public static Color ColourFromPlayability(Playability playability,bool isTool) {
		switch (playability) {		
			case Playability.Playable:
				return isTool ? S.playableColourTool   : S.playableColour;
			case Playability.Almost:
				return isTool ? S.almostColourTool     : S.almostColour;
			case Playability.Unplayable:
				return isTool ? S.unplayableColourTool : S.unplayableColour;
			default:
				// make exceptions obvious
				return Color.green;
		}
	}
	// convert selection to colour
	public Color selectableColour;
	public Color selectedColour;
	public static Color ColourFromSelectable(bool selected) {return selected ? Define.S.selectedColour : Define.S.selectableColour;}
	// Counters
	public ResourceCounter
		metalCounter,
		timeCounter,
		batteryCounter,
		fuelCounter;
	// maximum number of inputs/outputs that can be displayed on a card
	// mechanically capped to the same number
	public const int maxInputs  = 9;
	public const int maxOutputs = 9;
	public static Vector2 cardAspectRatio { get { return Game.cardAspectRatio; } }
}