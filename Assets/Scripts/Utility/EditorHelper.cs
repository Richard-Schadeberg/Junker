using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditorHelper : MonoBehaviour {
    public Game game;
    public bool packDeck;
    void Update() {
        if (Application.isPlaying) return;
        Game.S = game;
        foreach (Card card in game.cards) {
            card.gameObject.name = card.cardName;
            card.cardComponents.cardName = card.cardName;
            if (packDeck) {
                game.zoneTracker = new ZoneTracker(game.cards);
                game.animationHandler.AnimateInstant(card,GameAction.Repacking);
            }
        }
    }
}
