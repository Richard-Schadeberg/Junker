using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZoneTracker {
    Dictionary<Zone, ZoneContents> zoneObjects = new Dictionary<Zone, ZoneContents>();
    HandContents handContents;
    DeckContents deckContents;
    Card[] cards;
    public ZoneTracker(Card[] cards) {
        this.cards = (Card[])cards.Clone();
        BuildDictionary();
        foreach (Card card in cards) {
            zoneObjects[Zone.Deck].AddCard(card);
        }
    }
    void BuildDictionary() {
        handContents = new HandContents();
        zoneObjects.Add(Zone.Hand,handContents);
        deckContents = new DeckContents();
        zoneObjects.Add(Zone.Deck,deckContents);
        zoneObjects.Add(Zone.Play,new PlayContents());
        zoneObjects.Add(Zone.Junk,new JunkContents());
    }
    public void MoveCard(Card card,Zone origin,Zone goal)
    {
        zoneObjects[origin].RemoveCard(card);
        zoneObjects[goal].AddCard(card);
    }
    public Card[] HandCards() {return handContents.GetCards();}
    public void GameStateChanged() {handContents.isSorted = false;}
    public Card DrawCard() {return deckContents.DrawCard();}
    public int availableDiscards {get {return handContents.availableDiscards;}}
    public bool ZonePacked(Zone zone) {return zoneObjects[zone].packed;}
    public void PackZone(Zone zone) {zoneObjects[zone].PackZone();}
}
