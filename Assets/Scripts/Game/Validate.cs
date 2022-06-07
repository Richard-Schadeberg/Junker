using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// determine whether the game is in a legal state
// ie. no negative resources, no breaking rules on installed parts
public static class Validate {
    public static bool ValidState() {
        // make sure no resources are in the negative
        if (!ResourceTracker.IsLegal()) return false;
        // check all the cards are okay with the current gamestate
        foreach (Card card in Game.S.cards) {
            if (!card.IsLegal()) return false;
        }
        return true;
    }
}