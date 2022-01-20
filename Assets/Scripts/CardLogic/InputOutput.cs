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
                card.credits.UndoDiscards();
            } else {
                ResourceTracker.Add(input);
            }
        }
        Game.GameStateChanged();
        }

    public static void Output(Card card) {
        foreach (Resource output in card.outputs) {
            if (output == Resource.Card) {
                // draw card
            } else {
                ResourceTracker.Add(output);
            }
        }
        Game.GameStateChanged();
    }
    public static void UndoOutput(Card card) {
        foreach (Resource output in card.outputs) {
            if (output == Resource.Card) {
                // undo draw
            } else {
            ResourceTracker.Remove(output);
            }
        }
        Game.GameStateChanged();
    }
}
