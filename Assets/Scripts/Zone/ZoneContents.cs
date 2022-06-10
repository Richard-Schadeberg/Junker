using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
abstract public class ZoneContents {
    Zone zone;
    public ZoneContents(Zone zone) {this.zone = zone;}
    public virtual void AddCard(Card card)    {if (!Game.S.ReversibleMode) packed = false;} // children do stuff after this
    public virtual void RemoveCard(Card card) {if (!Game.S.ReversibleMode) packed = false;} // children do stuff after this
    public virtual Card[] GetCardsLeftToRight() {return null;}
    public virtual Card[] GetCards() {return null;}
    public virtual int NumCardsInZone() {return 0;}
    // zone packing is computationally expensive, especially for the hand
    // so the zone is not repacked unnecessarily
    public bool packed = false;
    public void PackThisZone() {
        if (packed) return;
        Bounds[] boundsList = Packing.PackBounds(Define.cardAspectRatio,Define.BoundsFromZone(zone),NumCardsInZone(),zone);
        int index = 0;
        // packed bounds are stored in the cards
        foreach (Card card in GetCardsLeftToRight()) {
            card.goalBoundsForCurrentGamestate = boundsList[index];
            index++;
        }
        packed = true;
    }
}
