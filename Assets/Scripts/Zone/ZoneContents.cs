using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ZoneContents {
    public bool packed = false;
    Zone zone;
    public ZoneContents(Zone zone) {
        this.zone = zone;
    }
    public virtual void AddCard(Card card) {
        if (!Game.S.ReversibleMode) {
            packed = false;
        }
    }
    public virtual void RemoveCard(Card card) {
        if (!Game.S.ReversibleMode) {
            packed = false;
        }
    }
    public virtual Card[] GetCardsLeftToRight() {return null;}
    public virtual Card[] GetCards() {return null;}
    public virtual int NumCards() {return 0;}
    public void PackZone() {
        Bounds[] boundsList = Packing.PackBounds(Game.cardAspectRatio,Define.Bounds(zone),NumCards());
        int index = 0;
        foreach (Card card in GetCardsLeftToRight()) {
            card.bounds = boundsList[index];
            index++;
        }
        packed = true;
    }
}
