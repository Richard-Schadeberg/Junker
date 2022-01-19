using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZoneTracker {
    public Dictionary<Zone, ZoneContents> zoneObjects = new Dictionary<Zone, ZoneContents>();
    HandContents handContents;
    DeckContents deckContents;
    Card[] cards;
    public ZoneTracker(Card[] cards) {
        this.cards = (Card[])cards.Clone();
        BuildDictionary();
        foreach (Card card in cards) {
            deckContents.AddCardToBottom(card);
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
    public void MoveCard(Card card,Zone origin,Zone goal) {
        zoneObjects[origin].RemoveCard(card);
        zoneObjects[goal].AddCard(card);
        card.zone = goal;
        Game.GameStateChanged();
    }
    public static Card[] GetCards(Zone zone) {return Game.S.zoneTracker.zoneObjects[zone].GetCards();}
    public static Card[] GetCardsLeftToRight(Zone zone) {return Game.S.zoneTracker.zoneObjects[zone].GetCardsLeftToRight();}
    public void GameStateChanged() {handContents.isSorted = false;}
    public Card DrawCard() {
        Card drawn = deckContents.DrawCard();
        if (drawn!=null) {
            handContents.AddCard(drawn);
            drawn.zone = Zone.Hand;
        }
        return drawn;
    }
    public int availableDiscards {get {return handContents.availableDiscards;}}
    public bool ZonePacked(Zone zone) {return zoneObjects[zone].packed;}
    public void PackZone(Zone zone) {zoneObjects[zone].PackZone();}
}
