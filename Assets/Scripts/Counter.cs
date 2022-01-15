using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
	public Text textObj;
    public void Set(int count) {
		textObj.text = count.ToString();
	}
	public SpriteRenderer icon;
	public void SetIcon(Resource resource) {
		icon.sprite = Define.Sprite(resource);
	}
	public void DrawGizmo(Resource resource) {
		Bounds bounds = icon.bounds;
		Gizmos.DrawGUITexture(new Rect(bounds.min.x,bounds.max.y,bounds.size.x,-bounds.size.y),Define.Sprite(resource).texture);
	}
}
