using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CardString {
    public static string CardArrayToString(Card[] cards) {
        DeckProperties deckProperties = new DeckProperties();
        foreach (Card card in cards) {
            deckProperties.cardPropertiesList.Add(new CardProperties(card));
        }
        return JsonUtility.ToJson(deckProperties,true);
    }
    public static string CardToString(Card card) {
        CardProperties cardProperties = new CardProperties(card);
        return JsonUtility.ToJson(cardProperties,true);
    }
    public static CardProperties[] StringToPropertiesArray(string jsonString) {
        return JsonUtility.FromJson<DeckProperties>(jsonString).cardPropertiesList.ToArray();
    }
}
