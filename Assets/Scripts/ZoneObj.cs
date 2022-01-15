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
    void OnMouseUp()
    {
        if (isHand) {
			Game.SortZone(Zone.Hand);
			Game.PackZone(Zone.Hand);
			foreach (Card card in Game.zoneCards[Zone.Hand]) {
				card.Animate();
			}
		}
    }
}
