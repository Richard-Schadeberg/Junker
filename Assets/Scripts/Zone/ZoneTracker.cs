using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// provides pointers and method access for each of the 4 zones
public class ZoneTracker {
    public ZoneTracker(Card[] cards) {
        BuildDictionary();
        foreach (Card card in cards) {
            deckContents.AddCard(card);
            // add cards to bottom instead of top, to preserve order
            deckContents.MoveTopToBottom();
        }
    }
    // each zone has a general parent pointer (ZoneContents) and a specialised child pointer (HandContents etc.)
    // yay polymorphism
    public Dictionary<Zone, ZoneContents> zoneObjects = new Dictionary<Zone, ZoneContents>();
    public HandContents handContents;
    public DeckContents deckContents;
    public PlayContents playContents;
    public JunkContents junkContents;
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
    // does not trigger animations
    public static void MoveCard(Card card,Zone origin,Zone goal) {
        Game.S.zoneTracker.zoneObjects[origin].RemoveCard(card);
        Game.S.zoneTracker.zoneObjects[goal].  AddCard(card);
        card.zone = goal;
    }
    public static Card[] GetCards(Zone zone)            {return Game.S.zoneTracker.zoneObjects[zone].GetCards();}
    public static Card[] GetCardsLeftToRight(Zone zone) {return Game.S.zoneTracker.zoneObjects[zone].GetCardsLeftToRight();}
    public void PlayerActionResolved() {handContents.isSorted = false;}
    public static Card TopCard() {return Game.S.zoneTracker.deckContents.TopCard();}
    // how many cards could be discarded (ie. don't have "can't be discarded")
    // possible bug: if card gains or loses "can't be discarded" while in hand
    public static int availableDiscards {get {return Game.S.zoneTracker.handContents.availableDiscards;}}
    public static bool ZonePacked(Zone zone) {return Game.S.zoneTracker.zoneObjects[zone].packed;}
    // packzone works out where each card in that zone should be displayed
    // can be called frequently; will return early if call was redundant
    public static void PackZone(Zone zone) {if (!ZonePacked(zone)) Game.S.zoneTracker.zoneObjects[zone].PackThisZone();}
}
