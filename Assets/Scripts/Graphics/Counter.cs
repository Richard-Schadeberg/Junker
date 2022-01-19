using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour {
	public Resource resource;
	public Text textObj;
	public SpriteRenderer icon;
	void Start() {icon.sprite = Define.Sprite(resource);}
    public void Set(int count) {textObj.text = count.ToString();}
	public void Update() {Set(ResourceTracker.Get(resource));}
}
