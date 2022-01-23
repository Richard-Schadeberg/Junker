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
    }
    public static void Output(Card card) {
        int draws=0;
        foreach (Resource output in card.outputs) {
            if (output == Resource.Card) {
                draws++;
            } else {
                ResourceTracker.Add(output);
            }
        }
        GameActions.DrawCards(draws,card);
    }
    public static void UndoOutput(Card card) {
        foreach (Resource output in card.outputs) {
            if (output == Resource.Card) {
                card.credits.UndoDraws();
            } else {
            ResourceTracker.Remove(output);
            }
        }
    }
}
