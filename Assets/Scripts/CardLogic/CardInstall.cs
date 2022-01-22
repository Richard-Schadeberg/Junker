using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardInstall {
    public static void Install(Card card) {
        InputOutput.Input(card);
        ZoneTracker.MoveCard(card,Zone.Hand,Zone.Play);
        AnimationHandler.Animate(card,GameAction.Installing);
        if (Game.S.ReversibleMode || DiscardRequester.S.pendingRequests==0) InputOutput.Output(card);
    }
    public static void Uninstall(Card card) {
        Card above = Game.S.zoneTracker.playContents.GetAbove(card);
        if (above!=null) Uninstall(above);
        if (Game.S.ReversibleMode || DiscardRequester.S.pendingRequests==0) InputOutput.UndoOutput(card);
        ZoneTracker.MoveCard(card,Zone.Play,Zone.Hand);
        AnimationHandler.Animate(card,GameAction.Uninstalling);
        InputOutput.UndoInput(card);
    }
    // reversibly installs and uninstalls the card to determine if it's possible to play
    public static bool CanInstall(Card card) {
        if (CardPlayable.isValid) {return (card.Playability==Playability.Playable);}
        Game.S.ReversibleMode = true;
        InputOutput.Input(card);
        ZoneTracker.MoveCard(card,Zone.Hand,Zone.Play);
        bool canInstall = Validate.ValidState();
        ZoneTracker.MoveCard(card,Zone.Play,Zone.Hand);
        InputOutput.UndoInput(card);
        Game.S.ReversibleMode = false;
        return canInstall;
    }
    public static void TryInstall(Card card) {
        if (CanInstall(card)) Install(card);
    }
    public static void ClockReturn(Card card) {
        ZoneTracker.MoveCard(card,Zone.Play,Zone.Hand);
        AnimationHandler.Animate(card,GameAction.ClockReturn);
    }
}
