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
        // prevent animations and unnecessary repacking
        Game.S.ReversibleMode = true;
        // install card
        InputOutput.Input(card);
        ZoneTracker.MoveCard(card,Zone.Hand,Zone.Play);
        // must be able to pay inputs before getting outputs
        bool canInstall = Validate.ValidState();
        // uninstall card
        ZoneTracker.MoveCard(card,Zone.Play,Zone.Hand);
        InputOutput.UndoInput(card);
        Game.S.ReversibleMode = false;
        return canInstall;
    }
    public static bool CanInstallWith(Card card,Card friend) {
        bool canInstall = true;
        Game.S.ReversibleMode = true;

        InputOutput.Input(friend);
        ZoneTracker.MoveCard(friend,Zone.Hand,Zone.Play);
        // if playing friend requires discarding card, then it's not possible to install card
        if (ZoneTracker.availableDiscards==DiscardRequester.S.pendingRequests) canInstall=false;
        if (!Validate.ValidState()) canInstall=false;
        InputOutput.Output(friend);

        InputOutput.Input(card);
        ZoneTracker.MoveCard(card,Zone.Hand,Zone.Play);
        if (!Validate.ValidState()) canInstall = false;
        ZoneTracker.MoveCard(card,Zone.Play,Zone.Hand);
        InputOutput.UndoInput(card);

        InputOutput.UndoOutput(friend);
        ZoneTracker.MoveCard(friend,Zone.Play,Zone.Hand);
        InputOutput.UndoInput(friend);

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
