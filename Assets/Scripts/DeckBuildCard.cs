using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeckZone {
	Library,
	Working,
	Deck
}

public class DeckBuildCard : MonoBehaviour
{
	public Card managedCard;
	public DeckZone zone = DeckZone.Library;
    void Start()
    {
        managedCard.BuildCard();
		GetComponent<Renderer>().sortingOrder = managedCard.GetComponent<Renderer>().sortingOrder + 5;
		managedCard.GetComponent<Collider2D>().enabled=false;
    }
	
	float timeMouseDown;
	const float clickMaxTime = 0.1f;
	private bool dragging;
	void OnMouseDown() {
		timeMouseDown = Time.time;
		if (DeckBuild.S.deckEditing) {
			StartDragging();
		}
	}
	
	void OnMouseUp() {
		if (dragging) {
			StopDragging();
		}
		if (!DeckBuild.S.deckEditing) {
			if (Time.time-timeMouseDown < clickMaxTime) {
				switch (zone) {
					case DeckZone.Library:
						MoveToWorking();
						break;
					case DeckZone.Working:
						MoveToLibrary();
						break;
				}
			}
		}
	}

    void Update()
    {
		if (dragging) FollowMouse();
        if (animating) TravelRoute();
    }
	Vector3 mouseVector;
	private void StartDragging() {
		dragging=true;
		mouseVector=Input.mousePosition - transform.position;
		transform.SetParent(null);
	}
	private void StopDragging() {
		dragging=false;
		if (DeckBuild.S.workingZone.bounds.Contains(GetComponent<Renderer>().bounds.center)) {
			MoveToWorking();
		}
	}
	private void FollowMouse() {
		transform.position = Input.mousePosition - mouseVector;
	}
	private void ChangeZone(DeckZone newZone) {
		zone = newZone;
		foreach (var zone in DeckBuild.S.zoneCards) {
			zone.Value.Remove(this);
		}
		DeckBuild.S.zoneCards[newZone].Add(this);
	}
	private void MoveToWorking() {
		ChangeZone(DeckZone.Working);
		DeckBuild.S.AnimateWorking();
		transform.SetParent(DeckBuild.S.scrolled.transform);
	}
	private void MoveToLibrary() {
		ChangeZone(DeckZone.Library);
		Animate(libIndex,DeckZone.Library);
		DeckBuild.S.AnimateWorking();
	}
	private Vector3 scrolledPos;
	public int libIndex;
	private bool animating;
	private Bounds origin,destination;
	float distance,startTime;
	public void Animate(int index,DeckZone zone) {
		switch (zone) {
			case DeckZone.Working:
				destination = GetBounds(DeckBuild.S.workingZone.bounds,GetComponent<Renderer>().bounds.size,index,2);
				break;
			case DeckZone.Library:
				destination = GetBounds(DeckBuild.S.libraryZone.bounds,GetComponent<Renderer>().bounds.size,index,4);
				break;
		}
		origin = GetComponent<Renderer>().bounds;
		// destination = new Bounds(newCentre,newSize);
		scrolledPos = DeckBuild.S.scrolled.transform.position;
		distance = (origin.center-destination.center).magnitude;
		animating=true;
		startTime = Time.time;
	}
	const float vMax      = 12000;
	const float accTime   =  0.2f;
	const float decTime   =  0.4f;
	private void TravelRoute() {
		float percent = Tween.tween(vMax/accTime/distance,vMax/decTime/distance,vMax/distance,Time.time-startTime);
		if (Single.IsNaN(percent)) {
			percent = 1.0f;
			animating=false;
		}
		Vector3 dest = destination.center;
		dest += DeckBuild.S.scrolled.transform.position - scrolledPos;
		SetBounds(new Bounds(origin.center+percent*(dest-origin.center),origin.size+percent*(destination.size-origin.size)));
	}
	private void SetBounds(Bounds bounds) {
		transform.position = bounds.center;
		transform.localScale = Packing.GetLocalScale(bounds,gameObject);
	}
	private Bounds GetBounds(Bounds bounds,Vector2 mySize,int index,int columns) {
		Vector2 newSize,newCentre;
		newSize = bounds.size/columns;
		newSize.y = newSize.x * mySize.y/mySize.x;
		newCentre = bounds.center;
		newCentre.y = bounds.max.y;
		newCentre.y -= newSize.y/2;
		newCentre.y -= newSize.y * (float)Math.Floor((float)index/columns);
		newCentre.x = bounds.min.x;
		newCentre.x += newSize.x/2;
		newCentre.x += newSize.x * (index % columns);
		return new Bounds(newCentre,newSize);
	}
}
