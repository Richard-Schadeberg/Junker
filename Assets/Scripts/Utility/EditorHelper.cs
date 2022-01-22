using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// helps arrange and edit object in Unity editor
[ExecuteInEditMode]
public class EditorHelper : MonoBehaviour {
    public Game game;
    public Define define;
    // tick to arrange cards in the deck in order
    public bool packDeck;
    void Update() {
        if (Application.isPlaying) return;
        Game.S = game;
        Define.S = define;
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
