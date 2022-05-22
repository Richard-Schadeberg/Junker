using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock {
    public ResourceCounter clockDisplay;
    public int timeLeft;
    public void SetTime(int time) {
        timeLeft = time;
        clockDisplay = Define.S.timeCounter;
        clockDisplay.setText(timeLeft.ToString());
    }
    public void ClockClicked_() {
        Card[] playCards = ZoneTracker.GetCardsLeftToRight(Zone.Play);
        if (playCards.Length == 0 || timeLeft == 0 || Game.S.discardRequester.pendingRequests != 0) return;
        timeLeft--;
        clockDisplay.setText(timeLeft.ToString());
        if (timeLeft==0) clockDisplay.Darken();
        ResourceTracker.Reset(Resource.Distance);
        ResourceTracker.Reset(Resource.Electric);
        ResourceTracker.Reset(Resource.Recon);
        ResourceTracker.Reset(Resource.Metal);
        ResourceTracker.Add(Resource.Metal, ResourceTracker.scrap);
        foreach (Card card in playCards) {
            CardCopier.DeleteChild(card);
        }
        playCards = ZoneTracker.GetCardsLeftToRight(Zone.Play);
        Game.S.animationHandler.RePackZone(Zone.Play);
        foreach (Card card in playCards) {
            Credits.ClearCredits(card);
            card.conversions = 0;
            ZoneTracker.MoveCard(card, Zone.Play, Zone.Hand);
        }
        AnimationHandler.Animate(playCards, GameAction.ClockReturn);
        Card[] junkedCards = ZoneTracker.GetCardsLeftToRight(Zone.Junk);
        junkedCards.Shuffle();
        foreach (Card card in junkedCards) {
            if (card.isCopy) continue;
            ZoneTracker.MoveCard(card, Zone.Junk, Zone.Deck);
            Game.S.zoneTracker.deckContents.MoveTopToBottom();
        }
        AnimationHandler.Animate(junkedCards, GameAction.Repacking);
        IconTracker.Reset();
        Game.GameStateChanged();
    }
    public static void ClockClicked() {
        Game.S.clock.ClockClicked_();
    }
}
