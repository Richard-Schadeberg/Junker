using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// helps arrange and edit object in Unity editor
[ExecuteInEditMode]
public class EditorHelper : MonoBehaviour {
    // manually selected pointers for singleton objects
    public Game game;
    public Define define;
    // tick to arrange cards in the deck in order
    public bool packDeck;
    void Update() {
        if (Application.isPlaying) return;
        Game.S = game;
        Define.S = define;
        // animationHandler is used to display cards in deck
        Game.S.animationHandler = new AnimationHandler();
        foreach (Card card in game.cards) {
            card.cardComponents.cardName = card.cardName;
            if (packDeck) {
                game.zoneTracker = new ZoneTracker(game.cards);
                game.animationHandler.AnimateInstant(card,GameAction.Repacking);
            }
        }
    }
}
