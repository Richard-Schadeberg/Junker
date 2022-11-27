using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CreateCard {
    // first item in array is closest to top of deck
    public static void AddCardsFromPropertiesToBottomOfDeck(CardProperties[] cardPropertiesArray) {
        foreach (CardProperties cardProperties in cardPropertiesArray) {
            GameObject newCardObj = MonoBehaviour.Instantiate(Define.GetCardPrefab(cardProperties.SpecialCards));
            Card newCard = newCardObj.GetComponent<Card>();
            cardProperties.ApplyToCard(newCard);
            // add the new card to Game.cards
            Card[] cardsCopy = new Card[Game.S.cards.Length + 1];
            Array.Copy(Game.S.cards, cardsCopy, Game.S.cards.Length);
            Game.S.cards = cardsCopy;
            Game.S.cards[Game.S.cards.Length - 1] = newCard;
            // add the copy to deck (add rather than move, as zonetracker doesn't know it exists yet)
            Game.S.zoneTracker.deckContents.AddCard(newCard);
            // deckContents adds new cards to top of deck by default
            Game.S.zoneTracker.deckContents.MoveTopToBottom();
            newCard.zone = Zone.Deck;
            Game.S.animationHandler.AnimateInstant(newCard, GameAction.Repacking);
        }
    }
    public static Card AddCardFromPropertiesToBottomOfDeck(CardProperties cardProperties) { 
        AddCardsFromPropertiesToBottomOfDeck(new CardProperties[] { cardProperties });
        return Game.S.cards.Last<Card>();
    }
}
