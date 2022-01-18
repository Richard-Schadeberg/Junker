using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputOutput {
    public static void Input(Card card) {
        foreach (Resource input in card.inputs) {
            ResourceTracker.Remove(input);
        }
        Game.GameStateChanged();
    }
    public static void Output(Card card) {
        foreach (Resource output in card.outputs) {
            ResourceTracker.Add(output);
        }
        Game.GameStateChanged();
    }
}
