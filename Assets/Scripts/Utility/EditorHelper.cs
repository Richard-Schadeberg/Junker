using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
            // This is a bit weird
            // If we just set deleteCard to false, the value is still flagged as having been modified compared to the prefab
            // This does the "right click -> revert" operation to reset it to the prefab value
            // For some reason not doing this causes the flag to briefly flip to true when play mode ends.
            SerializedProperty serializedPropertyMyInt = (new UnityEditor.SerializedObject(this)).FindProperty("deleteCard");
            PrefabUtility.RevertPropertyOverride(serializedPropertyMyInt, InteractionMode.UserAction);

            Card card = game.cards[0];
            List<Card> cards = game.cards.ToList();
            cards.RemoveAt(0);
            game.cards = cards.ToArray();
            DestroyImmediate(card.gameObject);
        }
        if (addCard) {
            addCard = false;
            // This is a bit weird
            // If we just set addCard to false, the value is still flagged as having been modified compared to the prefab
            // This does the "right click -> revert" operation to reset it to the prefab value
            // For some reason not doing this causes the flag to briefly flip to true when play mode ends.
            SerializedProperty serializedPropertyMyInt = (new UnityEditor.SerializedObject(this)).FindProperty("addCard");
            PrefabUtility.RevertPropertyOverride(serializedPropertyMyInt, InteractionMode.UserAction);

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
