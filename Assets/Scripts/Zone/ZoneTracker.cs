using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZoneTracker {
    public Dictionary<Zone, ZoneContents> zoneObjects = new Dictionary<Zone, ZoneContents>();
    public HandContents handContents;
    public DeckContents deckContents;
    public PlayContents playContents;
    public JunkContents junkContents;
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
        playContents = new PlayContents();
        zoneObjects.Add(Zone.Play,playContents);
        junkContents = new JunkContents();
        zoneObjects.Add(Zone.Junk,junkContents);
    }
    public static void MoveCard(Card card,Zone origin,Zone goal) {Game.S.zoneTracker._MoveCard(card,origin,goal);}
    public void _MoveCard(Card card,Zone origin,Zone goal) {
        zoneObjects[origin].RemoveCard(card);
        zoneObjects[goal].AddCard(card);
        card.zone = goal;
        Game.GameStateChanged();
    }
    public static Card[] GetCards(Zone zone) {return Game.S.zoneTracker.zoneObjects[zone].GetCards();}
    public static Card[] GetCardsLeftToRight(Zone zone) {return Game.S.zoneTracker.zoneObjects[zone].GetCardsLeftToRight();}
    public void GameStateChanged() {Debug.Log(availableDiscards); handContents.isSorted = false;}
    public Card DrawCard() {
        Card drawn = deckContents.DrawCard();
        if (drawn!=null) {
            handContents.AddCard(drawn);
            drawn.zone = Zone.Hand;
        }
        return drawn;
    }
    public static int availableDiscards {get {return Game.S.zoneTracker.handContents.availableDiscards;}}
    public static bool ZonePacked(Zone zone) {return Game.S.zoneTracker.zoneObjects[zone].packed;}
    public static void PackZone(Zone zone) {Game.S.zoneTracker.zoneObjects[zone].PackZone();}
}
