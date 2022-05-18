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
    public void Brighten() {
        spriteRenderer.color = Color.white;
    }
    public void Darken() {
        spriteRenderer.color = Color.gray;
    }
    public void Enable() {
        gameObject.SetActive(true);
    }
    public void Disable() {
        gameObject.SetActive(false);
    }
    private Resource displayedResource;
    public void SetSprite(Resource resource) {
        spriteRenderer.sprite = Define.Sprite(resource);
        displayedResource = resource;
    }
}
