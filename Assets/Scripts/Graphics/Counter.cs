using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// a UI object that displays how much you have of a given resource
// obsolete?
public class Counter : MonoBehaviour {
	public Resource resource;
	public Text textObj;
	public SpriteRenderer icon;
    public void Set(int count) {textObj.text = count.ToString();}
	void Start() {icon.sprite = Define.Sprite(resource);}
	// called every frame. TODO: call less often to improve performance
	void Update() {Set(ResourceTracker.Get(resource));}
}
