using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputOutput {
    public static void Input(Card card) {
        foreach (Resource input in card.inputs) {
            if (input == Resource.Card) {
                DiscardRequest.RequestDiscard(card);
            } else {
               ResourceTracker.Remove(input);
            }
        }
        Game.GameStateChanged();
    }
    public static void UndoInput(Card card) {
        foreach (Resource input in card.inputs) {
            if (input == Resource.Card) {
                DiscardRequest.CancelRequest();
            } else {
               ResourceTracker.Add(input);
            }
        }
        Game.GameStateChanged();
        }

    public static void Output(Card card) {
        foreach (Resource output in card.outputs) {
            ResourceTracker.Add(output);
        }
        Game.GameStateChanged();
    }
    public static void UndoOutput(Card card) {
        foreach (Resource output in card.outputs) {
            ResourceTracker.Remove(output);
        }
        Game.GameStateChanged();
    }
}
