using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardInstall
{
    public static void Install(Card card) {
        Game.S.zoneTracker.MoveCard(card,Zone.Hand,Zone.Play);
        InputOutput.Input(card);
        InputOutput.Output(card);
    }
    public static void InstallReversible(Card card) {
        Game.S.ReversibleMode = true;
        Install(card);
    }
}
