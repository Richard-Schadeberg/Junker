using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// helps arrange and edit object in Unity editor
[ExecuteInEditMode]
public class EditorHelper : MonoBehaviour {
    // manually selected pointers for singleton objects
    public Game game;
    public Define define;
    // tick to arrange cards in the deck in order
    public bool packDeck;
    // tick to delete the top card of the deck, unticks itself automatically
    public bool deleteCard = false;
    // tick to add a card, unticks itself automatically
    public bool addCard = false;
    // properties of the card to be added
    public CardProperties cardProperties;
    void Update() {
        if (Application.isPlaying) return;
        Game.S = game;
        Define.S = define;
        if (deleteCard) {
            deleteCard = false;
            Card card = game.cards[0];
            List<Card> cards = game.cards.ToList();
            cards.RemoveAt(0);
            game.cards = cards.ToArray();
            DestroyImmediate(card.gameObject);
        }
        if (addCard) {
            addCard = false;
            CreateCard.AddCardFromPropertiesInEditor(cardProperties);
        }
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
