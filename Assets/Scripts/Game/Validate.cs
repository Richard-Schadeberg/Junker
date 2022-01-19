using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Validate {
    public static bool ValidState() {
        if (!ResourceTracker.IsValid()) return false;
        foreach (Card card in Game.S.cards) {
            if (!card.IsValid()) return false;
        }
        return true;
    }
}
