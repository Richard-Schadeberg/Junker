using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputOutput {
    public static void Input(Card card) {
        foreach (Resource input in card.inputs) {
            if (input == Resource.Card) {
                DiscardRequester.RequestDiscard(card);
            } else {
               ResourceTracker.Remove(input);
            }
        }
        Game.GameStateChanged();
    }
    public static void UndoInput(Card card) {
        foreach (Resource input in card.inputs) {
            if (input == Resource.Card) {
                DiscardRequester.CancelRequest();
                if (!Game.S.ReversibleMode) card.credits.UndoDiscards();
            } else {
                ResourceTracker.Add(input);
            }
        }
        Game.GameStateChanged();
    }
    public static void Output(Card card) {
        foreach (Resource output in card.outputs) {
            if (output == Resource.Card) {
                Card drawn = Game.S.zoneTracker.DrawCard();
                if (drawn != null) {
                    card.credits.Draw(drawn);
                    AnimationHandler.Animate(drawn,GameAction.Drawing);
                }
            } else {
                ResourceTracker.Add(output);
            }
        }
        Game.GameStateChanged();
    }
    public static void UndoOutput(Card card) {
        foreach (Resource output in card.outputs) {
            if (output == Resource.Card) {
                card.credits.UndoDraws();
            } else {
            ResourceTracker.Remove(output);
            }
        }
        Game.GameStateChanged();
    }
}
