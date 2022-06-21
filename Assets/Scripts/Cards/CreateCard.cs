using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class CreateCard
{
    public struct CardProperties {
        public string CardName;
        public string Description;
        public Resource[] Inputs;
        public Resource[] Outputs;
        public bool IsTool;
        public bool WinsGame;
        public bool SingleUse;
        public bool Scaleable;
        public int PartLimit;
        public int RequiredPart;
        public CardProperties(
            string cardName,
            string description,
            Resource[] inputs,
            Resource[] outputs,
            bool isTool,
            bool winsGame,
            bool singleUse,
            bool scaleable,
            int partLimit,
            int requiredPart
        ) {
            CardName = cardName;
            Description = description;
            Inputs = inputs;
            Outputs = outputs;
            IsTool = isTool;
            WinsGame = winsGame;
            SingleUse = singleUse;
            Scaleable = scaleable;
            PartLimit = partLimit;
            RequiredPart = requiredPart;
        }
    }
    public static CardProperties SimpleProperties(string name,string description, Resource[] inputs, Resource[] outputs) {
        return new CardProperties(name, description, inputs, outputs, false, false, false, false, 0, 0);
    }
    public static CardProperties SimpleProperties(Resource[] inputs, Resource[] outputs) {
        return SimpleProperties("Simple Card", "Simple Description", inputs, outputs);
    }
    public static Card MakePrefab() {
        GameObject gameObject = (GameObject)Resources.Load("card", typeof(GameObject));
        return gameObject.GetComponent<Card>();
    }
    public static void SetCardProperties(Card card,CardProperties cardProperties) {
        card.cardName = cardProperties.CardName;
        card.cardComponents.description = cardProperties.Description;
        card.inputs = cardProperties.Inputs;
        card.outputs = cardProperties.Outputs;
        card.isTool = cardProperties.IsTool;
        card.winsGame = cardProperties.WinsGame;
        card.singleUse = cardProperties.SingleUse;
        card.scaleable = cardProperties.Scaleable;
        card.partLimit = cardProperties.PartLimit;
        card.requiredPart = cardProperties.RequiredPart;
    }
    public static Card CreateNewCard(CardProperties cardProperties) {
        Card newCard = MakePrefab();
        SetCardProperties(newCard, cardProperties);
        newCard.Start();
        return newCard;
    }
}
