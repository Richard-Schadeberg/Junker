using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceCounter : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    public bool clockButton;
    public void setText(string text) { textBox.text = text; }
    public void OnMouseDown() {
        Clock.ClockClicked();
    }
    public void Darken() {
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>()) {
            spriteRenderer.color = Color.gray;
        }
    }
}
