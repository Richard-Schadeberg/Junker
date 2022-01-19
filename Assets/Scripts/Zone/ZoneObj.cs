using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneObj : MonoBehaviour
{
	public bool maxWidth;
	void Awake() {
		if (maxWidth) {
			Vector3 localScale = transform.localScale;
			float desiredRatio = 1920f/1080f;
			float actualRatio = (float)(Screen.width)/(float)(Screen.height);
			localScale.x *= actualRatio/desiredRatio;
			transform.localScale = localScale;
		}
	}
	public bool isHand;
	void OnMouseDown() {
		if (isHand) {
			foreach (Card card in ZoneTracker.GetCards(Zone.Hand)) {
				AnimationHandler.Animate(card,GameAction.Repacking);
			}
		}
	}
}
