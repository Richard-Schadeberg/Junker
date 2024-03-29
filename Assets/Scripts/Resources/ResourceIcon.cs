using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceIcon : MonoBehaviour {
    private SpriteRenderer spriteRenderer { 
        get {
            if (spriteRenderer_==null) spriteRenderer_ = gameObject.GetComponent<SpriteRenderer>();
            return spriteRenderer_;
        }
    }
    private SpriteRenderer spriteRenderer_;
    public bool brightened { get {
            return brightened_;
        } }
    private bool brightened_;
    public void Brighten() {
        spriteRenderer.color = Color.white;
        brightened_ = true;
    }
    public void Darken() {
        spriteRenderer.color = Color.gray;
        brightened_ = false;
    }
    // icon should only be enabled if you know what it will be displaying
    public void Enable(Resource resource) {
        gameObject.SetActive(true);
        SetSprite(resource);
    }
    public void Disable() {
        displayedResource = Resource.None;
        gameObject.SetActive(false);
    }
    private Resource displayedResource = Resource.None;
    private void SetSprite(Resource resource) {
        if (resource == displayedResource) return;
        spriteRenderer.sprite = Define.SpriteFromResource(resource);
        displayedResource = resource;
    }
}
