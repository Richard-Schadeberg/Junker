using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// determine whether the game is in a legal state
// ie. no negative resources, no breaking rules on installed parts
public static class Validate {
    public static bool ValidState() {
        if (!ResourceTracker.IsLegal()) return false;
        foreach (Card card in Game.S.cards) {
            if (!card.IsLegal()) return false;
        }
        return true;
    }
}