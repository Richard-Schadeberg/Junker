using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoundsUtil {
	public static void SetBounds(Card card,Bounds bounds) {
		card.transform.position = bounds.center;
		card.transform.localScale = GetLocalScale(bounds,card.gameObject);
	}
	// based on current bounds and desired bounds, get the required localscale
	public static Vector2 GetLocalScale(Bounds desiredBounds,GameObject you) {
		Vector3 newScale = you.transform.localScale;
		newScale.x *= desiredBounds.size.x/you.GetComponent<Renderer>().bounds.size.x;
		newScale.y *= desiredBounds.size.y/you.GetComponent<Renderer>().bounds.size.y;
		return newScale;
	}
}
