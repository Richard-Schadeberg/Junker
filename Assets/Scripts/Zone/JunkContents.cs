using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JunkContents : ZoneContents
{
    Zone zone;
    protected HashSet<Card> cards = new HashSet<Card>();
    public JunkContents() : base(Zone.Junk) {}
    public override void AddCard(Card card) {
        base.AddCard(card);
        cards.Add(card);
    }
    public override void RemoveCard(Card card) {
        base.RemoveCard(card);
        cards.Remove(card);
    }
    public override Card[] GetCardsLeftToRight() {
        return cards.ToArray();
    }
    public override int NumCards()
    {
        return cards.Count;
    }
}
