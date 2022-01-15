using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//{ enum definitions
public enum Resource {
	Card,
	Recycle,
	Battery,
	Electric,
	Coal,
	Heat,
	Scrap,
	Metal,
	Distance,
	Recon,
	Time
}
public enum Zone {
	Deck,
	Hand,
	Play,
	Junk,
	Counter,
	Special
}
public enum Status {
	Playable=1,
	Almost=2,
	Unplayable=3
}
public enum Sound {
	Install,
	Uninstall
}
//}
[ExecuteAlways]
public class Define : MonoBehaviour {
//{ create static pointer to self to allow static methods to reference properties for the inspector's sake
	public static Define S;
	void Update() {
		if (Application.isEditor) {
			S = this;
			BuildResourceSprites();
			BuildZoneObjs();
			BuildStatusColours();
			BuildCounters();
		}
	}
	void Awake() {
		S = this;
	}
	void Start() {
		BuildResourceSprites();
		BuildZoneObjs();
		BuildStatusColours();
		BuildCounters();
		SetCounterIcons();
	}
//}
//{ draw counter Gizmos in the editor
    void OnDrawGizmos() {
		DrawCounterGizmos();
	}
//}
//{ convert Resource to Sprite
	public static Sprite Sprite(Resource resource) {
		if (resourceSprites.Count==0) {
			BuildResourceSprites();
		}
		return resourceSprites[resource];
	}
	public static Dictionary<Resource,Sprite> resourceSprites = new Dictionary<Resource,Sprite>();
	public Sprite 
		cardSprite,
		recycleSprite,
		batterySprite,
		electricSprite,
		coalSprite,
		heatSprite,
		scrapSprite,
		metalSprite,
		distanceSprite,
		reconSprite,
		timeSprite;
	public static void BuildResourceSprites() {
		resourceSprites[Resource.Card] = S.cardSprite;
		resourceSprites[Resource.Recycle] = S.recycleSprite;
		resourceSprites[Resource.Battery] = S.batterySprite;
		resourceSprites[Resource.Electric] = S.electricSprite;
		resourceSprites[Resource.Coal] = S.coalSprite;
		resourceSprites[Resource.Heat] = S.heatSprite;
		resourceSprites[Resource.Scrap] = S.scrapSprite;
		resourceSprites[Resource.Metal] = S.metalSprite;
		resourceSprites[Resource.Distance] = S.distanceSprite;
		resourceSprites[Resource.Recon] = S.reconSprite;
		resourceSprites[Resource.Time] = S.timeSprite;
	}
//}
//{ convert Zone to Bounds
	public static Bounds Bounds(Zone zone) {
		if (zoneObjs.Count==0) {
			BuildZoneObjs();
		}
		return zoneObjs[zone].GetComponent<SpriteRenderer>().bounds;
	}
	public static Dictionary<Zone,ZoneObj> zoneObjs = new Dictionary<Zone,ZoneObj>();
	public ZoneObj
		deckZone,
		handZone,
		playZone,
		junkZone,
		counterZone,
		specialZone;
	public static void BuildZoneObjs() {
		zoneObjs[Zone.Deck] = S.deckZone;
		zoneObjs[Zone.Hand] = S.handZone;
		zoneObjs[Zone.Play] = S.playZone;
		zoneObjs[Zone.Junk] = S.junkZone;
		zoneObjs[Zone.Counter] = S.counterZone;
		zoneObjs[Zone.Special] = S.specialZone;
	}
//}
//{ convert Status to Color
	public static Color Colour(Status status) {
		if (statusColours.Count==0) {
			BuildStatusColours();
		}
		return statusColours[status];
	}
	public static Dictionary<Status,Color> statusColours = new Dictionary<Status,Color>();
	public Color
		playableColour,
		almostColour,
		unplayableColour;
	public static void BuildStatusColours() {
		statusColours[Status.Playable] = S.playableColour;
		statusColours[Status.Almost] = S.almostColour;
		statusColours[Status.Unplayable] = S.unplayableColour;
	}
//}
//{ convert Resource to Counter
	public static Counter Counter(Resource resource) {
		if (counters.Count==0) {
			BuildCounters();
		}
		return counters[resource];
	}
	public static Dictionary<Resource,Counter> counters = new Dictionary<Resource,Counter>();
	public Counter 
		cardCounter,
		recycleCounter,
		batteryCounter,
		electricCounter,
		coalCounter,
		heatCounter,
		scrapCounter,
		metalCounter,
		distanceCounter,
		reconCounter,
		timeCounter;
	public static void BuildCounters() {
		counters[Resource.Card] = S.cardCounter;
		counters[Resource.Recycle] = S.recycleCounter;
		counters[Resource.Battery] = S.batteryCounter;
		counters[Resource.Electric] = S.electricCounter;
		counters[Resource.Coal] = S.coalCounter;
		counters[Resource.Heat] = S.heatCounter;
		counters[Resource.Scrap] = S.scrapCounter;
		counters[Resource.Metal] = S.metalCounter;
		counters[Resource.Distance] = S.distanceCounter;
		counters[Resource.Recon] = S.reconCounter;
		counters[Resource.Time] = S.timeCounter;
	}
//}
//{ get random sound effect files
	private static System.Random rng = new System.Random();
	public AudioClip[] installSounds,uninstallSounds;
	public static int soundNum=0;
	public static AudioClip GetSound(Sound sound) {
		AudioClip[] soundsList;
		switch (sound) {
			case Sound.Install:
				soundsList=S.installSounds;
				break;
			case Sound.Uninstall:
				soundsList=S.uninstallSounds;
				break;
			default:
				soundsList=S.installSounds;
				break;
		}
		return soundsList[rng.Next(soundsList.Length)];
	}
//}
//{ set/draw Counter Resource types
	void DrawCounterGizmos() {
		BuildCounters();
		foreach(var counter in counters) {
			if (counter.Value != null) {
				counter.Value.DrawGizmo(counter.Key);
			}
		}
	}
	public void SetCounterIcons() {
		BuildCounters();
		foreach(var counter in counters) {
			if (counter.Value != null) {
				counter.Value.SetIcon(counter.Key);
			}
		}
	}
//}
	public static Counter[] GetCounterList() {
		return new Counter[] {
			S.recycleCounter,
			S.electricCounter,
			S.heatCounter,
			S.metalCounter,
			S.distanceCounter,
			S.reconCounter,
			
			S.cardCounter,
			S.batteryCounter,
			S.coalCounter,
			S.scrapCounter,
			S.timeCounter};
	}
	public Color selectableColour;
	public Color selectedColour;
}