using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeckBuild : MonoBehaviour
{
	public static DeckBuild S;
	void Awake() {
		S=this;
	}
	void Start() {
		cameraStartPos = controlledCam.transform.position;
		BuildZoneDict();
		int i=0;
		foreach (DeckBuildCard card in cards) {
			zoneCards[DeckZone.Library].Add(card);
			card.libIndex = i;
			card.Animate(i,DeckZone.Library);
			i++;
		}
	}
	void Update() {
		if (movingCamera) {
			float percent = Tween.tween(8f,16f,8f,Time.time-startTime);
			if (Single.IsNaN(percent)) {
				percent=1.0f;
				movingCamera=false;
			}
			if (!movingRight) percent = 1f - percent;
			Vector3 displacement = new Vector3(percent*libraryZone.bounds.size.x,0,0);
			controlledCam.transform.position = cameraStartPos + displacement;
		}
	}
	public DeckBuildCard[] cards;
	private Vector3 cameraStartPos;
	private float startTime;
    public Camera controlledCam;
	public SpriteRenderer libraryZone,workingZone,deckZone1,deckZone2,deckZone3;
	public GameObject scrolled;
	public bool deckEditing=false;
	private bool movingCamera,movingRight;
	public void SwitchMode() {
		movingCamera=true;
		startTime = Time.time;
		if (deckEditing) {
			deckEditing=false;
			movingRight=false;
		} else {
			deckEditing=true;
			movingRight=true;
		}
	}
	public Dictionary<DeckZone,List<DeckBuildCard>> zoneCards = new Dictionary<DeckZone,List<DeckBuildCard>>();
	public void BuildZoneDict() {
		foreach(DeckZone zone in DeckZone.GetValues(typeof(DeckZone))) {
			zoneCards[zone] = new List<DeckBuildCard>();
		}
	}
	public void AnimateWorking() {
		int i=0;
		foreach (var card in zoneCards[DeckZone.Working]) {
			card.Animate(i,DeckZone.Working);
			i++;
		}
	}
}
