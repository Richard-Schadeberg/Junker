using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[ExecuteAlways]
public class Define : MonoBehaviour {
	// create static pointer to self to allow static methods to reference properties for the inspector's sake
	public static Define S;
	void Update() {
		if (Application.isEditor && !Application.isPlaying) {
			S = this;
		}
	}
	void Awake() {
		S = this;
	}
	// convert Resource to Sprite
	public Sprite 
		noneSprite,
		cardSprite,
		batterySprite,
		electricSprite,
		coalSprite,
		heatSprite,
		scrapSprite,
		metalSprite,
		distanceSprite,
		reconSprite,
		timeSprite;
	public static Sprite Sprite(Resource resource) {
		switch (resource) {
			case Resource.None:
				return S.noneSprite;
			case Resource.Card:
				return S.cardSprite;
			case Resource.Battery:
				return S.batterySprite;
			case Resource.Electric:
				return S.electricSprite;
			case Resource.Coal:
				return S.coalSprite;
			case Resource.Heat:
				return S.heatSprite;
			case Resource.Scrap:
				return S.scrapSprite;
			case Resource.Metal:
				return S.metalSprite;
			case Resource.Distance:
				return S.distanceSprite;
			case Resource.Recon:
				return S.reconSprite;
			case Resource.Time:
				return S.timeSprite;
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
	public static Bounds Bounds(Zone zone) {
		return ZoneObj(zone).GetComponent<SpriteRenderer>().bounds;
	}
	public static ZoneObj ZoneObj(Zone zone) {
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
	// convert Status to Color
	public Color
		playableColour,
		almostColour,
		unplayableColour;
	public static Color Colour(Playability status) {
		switch (status) {		
			case Playability.Playable:
				return S.playableColour;
			case Playability.Almost:
				return S.almostColour;
			case Playability.Unplayable:
				return S.unplayableColour;
			default:
				return Color.green;
		}
	}
	//convert Resource to Counter
	public Counter 
		batteryCounter,
		electricCounter,
		coalCounter,
		heatCounter,
		scrapCounter,
		metalCounter,
		distanceCounter,
		reconCounter,
		timeCounter;
	public static Counter Counter(Resource resource) {
		switch (resource) {
			case Resource.Battery:
				return S.batteryCounter;
			case Resource.Electric:
				return S.electricCounter;
			case Resource.Coal:
				return S.coalCounter;
			case Resource.Heat:
				return S.heatCounter;
			case Resource.Scrap:
				return S.scrapCounter;
			case Resource.Metal:
				return S.metalCounter;
			case Resource.Distance:
				return S.distanceCounter;
			case Resource.Recon:
				return S.reconCounter;
			case Resource.Time:
				return S.timeCounter;
			default:
				return null;
		}
	}
	public Color selectableColour;
	public Color selectedColour;
	public static Color Colour(bool selected) {return selected ? Define.S.selectedColour : Define.S.selectableColour;}
}