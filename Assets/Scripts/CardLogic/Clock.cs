using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// TODO: work with scaleable
public class Clock {
    // visual indicator for number of turns left
    public ResourceCounter clockDisplay;
    public int turnsRemaining;
    public void SetTurnsRemaining(int turns) {
        turnsRemaining = turns;
        clockDisplay = Define.S.timeCounter;
        clockDisplay.setText(turnsRemaining.ToString());
    }
    // player clicked on "end turn" button
    public void ClockClicked_() {
        Card[] playCards = ZoneTracker.GetCardsLeftToRight(Zone.Play);
        // don't do anything if the clock would have no effect or the player is still selecting discards
        if (playCards.Length == 0 || turnsRemaining == 0 || Game.S.discardRequester.pendingRequests != 0) return;
        turnsRemaining--;
        clockDisplay.setText(turnsRemaining.ToString());
        // dim clock when it can't be used anymore
        if (turnsRemaining==0) clockDisplay.Darken();
        // reset temporary resources
        ResourceTracker.Reset(Resource.Distance);
        ResourceTracker.Reset(Resource.Electric);
        ResourceTracker.Reset(Resource.Recon);
        // metal is special
        ResourceTracker.Reset(Resource.Metal);
        ResourceTracker.Add(Resource.Metal, ResourceTracker.scrap);
        // delete scaleable copies
        foreach (Card card in playCards) {
            CardCopier.DeleteChild(card);
        }
        // re-fetch playCards now that there are no scaleable copies in it
        playCards = ZoneTracker.GetCardsLeftToRight(Zone.Play);
        // fill gaps left by scaleable copies
        Game.S.animationHandler.RePackZone(Zone.Play);
        foreach (Card card in playCards) {
            // no reversing actions past clock usage
            Credits.ClearCredits(card);
            card.batteryConversions = 0;
            ZoneTracker.MoveCard(card, Zone.Play, Zone.Hand);
        }
        AnimationHandler.Animate(playCards, GameAction.ClockReturn);
        // discarded cards get put on the bottom of the deck at end of turn
        Card[] junkedCards = ZoneTracker.GetCardsLeftToRight(Zone.Junk);
        junkedCards.Shuffle();
        foreach (Card card in junkedCards) {
            if (card.isCopy) throw new Exception("Scaleable copy in Junk");
            ZoneTracker.MoveCard(card, Zone.Junk, Zone.Deck);
            // by default, cards are added to the top of the deck
            Game.S.zoneTracker.deckContents.MoveTopToBottom();
        }
        AnimationHandler.Animate(junkedCards, GameAction.Repacking);
        // recalculate icon dimming that represents resource spending
        IconTracker.Reset();
        // ending turn is a player action
        Game.PlayerActionResolved();
    }
    // static version to shorten call
    public static void ClockClicked() {
        Game.S.clock.ClockClicked_();
    }
}
