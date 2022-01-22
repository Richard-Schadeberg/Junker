using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// a simple class attached to rectangles to indicate zones
public class ZoneObj : MonoBehaviour {
	public bool maxWidth;
	void Awake() {
		// resize zone to use whole screen width
		if (maxWidth) {
			Vector3 localScale = transform.localScale;
			float desiredRatio = 1920f/1080f;
			float actualRatio = (float)(Screen.width)/(float)(Screen.height);
			localScale.x *= actualRatio/desiredRatio;
			transform.localScale = localScale;
		}
	}
	// clicking on empty space in the hand fills in gaps from missing cards
	public bool isHand;
	void OnMouseDown() {
		if (isHand) {
			foreach (Card card in ZoneTracker.GetCards(Zone.Hand)) {
				AnimationHandler.Animate(card,GameAction.Repacking);
			}
		}
	}
}
