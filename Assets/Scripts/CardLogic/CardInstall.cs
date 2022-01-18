using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardInstall {
    public static void TryInstall(Card card) {
        InputOutput.Input(card);
        Game.S.zoneTracker.MoveCard(card,Zone.Hand,Zone.Play);
        Game.S.animationHandler.Animate(card,GameAction.Installing);
        InputOutput.Output(card);
    }
    public static void InstallReversible(Card card) {
        Game.S.ReversibleMode = true;
        TryInstall(card);
    }
}
