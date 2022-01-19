using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardInstall {
    public static bool CanInstall(Card card) {
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
        Install(card);
    }
    public static void InstallReversible(Card card) {
        Game.S.ReversibleMode = true;
        TryInstall(card);
    }
    public static void Install(Card card) {
        InputOutput.Input(card);
        ZoneTracker.MoveCard(card,Zone.Hand,Zone.Play);
        AnimationHandler.Animate(card,GameAction.Installing);
        InputOutput.Output(card);
    }
    public static void Uninstall(Card card) {
        Card above = Game.S.zoneTracker.playContents.GetAbove(card);
        if (above!=null) Uninstall(above);
        InputOutput.UndoOutput(card);
        ZoneTracker.MoveCard(card,Zone.Play,Zone.Hand);
        AnimationHandler.Animate(card,GameAction.Uninstalling);
        InputOutput.UndoInput(card);
    }
}
