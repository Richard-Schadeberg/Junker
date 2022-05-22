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
    public virtual int NumCards() {return 0;}
    // zone packing is computationally expensive, especially for the hand
    // so the zone is not repacked unnecessarily
    public bool packed = false;
    public void PackZone() {
        Bounds[] boundsList = Packing.PackBounds(Game.cardAspectRatio,Define.Bounds(zone),NumCards(),false,zone==Zone.Junk);
        int index = 0;
        foreach (Card card in GetCardsLeftToRight()) {
            card.goalBoundsForCurrentGamestate = boundsList[index];
            index++;
        }
        packed = true;
    }
}
