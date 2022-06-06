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
    public static void UpdateCounters() {
        Define.S.batteryCounter.setText(ResourceTracker.Get(Resource.Battery).ToString());
        Define.S.fuelCounter.setText(ResourceTracker.Get(Resource.Fuel).ToString());
        Define.S.timeCounter.setText(Game.S.clock.turnsRemaining.ToString());
        Define.S.metalCounter.setText(ResourceTracker.Get(Resource.Metal).ToString() + "/" + ResourceTracker.scrap.ToString());
    }
}
