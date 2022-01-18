using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoundsUtil {
	public static void SetBounds(Card card,Bounds bounds) {
		card.transform.position = bounds.center;
		card.transform.localScale = GetLocalScale(bounds,card.gameObject);
	}
	public static Vector2 GetLocalScale(Bounds bounds,GameObject you) {
		Vector3 newScale = you.transform.localScale;
		newScale.x *= bounds.size.x/you.GetComponent<Renderer>().bounds.size.x;
		newScale.y *= bounds.size.y/you.GetComponent<Renderer>().bounds.size.y;
		return newScale;
	}
}
