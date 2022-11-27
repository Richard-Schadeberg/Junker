using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
public static class JSON_IO {
    public static void WriteCardsToJSON(Card[] cards) { WriteCardsToJSON(cards, "G:\temp\temp.json"); }
    public static void WriteCardsToJSON(Card[] cards, string filePath) {
        string JSONtext = CardString.CardArrayToString(cards);
        File.WriteAllText(filePath, JSONtext);
    }
    public static CardProperties[] GetCoreCardsProperties() {
        string JSONtext = Define.S.cardDesignsJSON.ToString();
        return CardString.StringToPropertiesArray(JSONtext);
    }
    public static CardProperties GetCoreCard(string name) {
        CardProperties[] cardPropertiesArray = GetCoreCardsProperties();
        CardProperties cardProperties = Array.Find<CardProperties>(cardPropertiesArray, card => card.name == name);
        return cardProperties;
    }
    public static CardProperties[] GetCoreCards(string[] names) {
        CardProperties[] cardPropertiesArray = GetCoreCardsProperties();
        Dictionary<string, CardProperties> cardPropertiesDictionary = cardPropertiesArray.ToDictionary(x => x.name);
        List<CardProperties> cardPropertiesList = new List<CardProperties>();
        foreach (string name in names) {
            cardPropertiesList.Add(cardPropertiesDictionary[name]);
        }
        return cardPropertiesList.ToArray();
    }
}
