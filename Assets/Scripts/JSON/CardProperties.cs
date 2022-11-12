using UnityEngine;
using System;
[Serializable]
public class CardProperties {
	// design aspects
	public Resource[] inputs;
	public Resource[] outputs;
	// tools can't be discarded and are always in your starting hand
	// singleUse parts discard themselves after use
	// scaleable cards create a tool copy of themselves whenever they are played
	// they can be played any number of times as long as you can pay their inputs
	public bool isTool, winsGame, singleUse, scaleable;
	// some parts restrict the number of parts you can play this turn (0 for no restriction)
	public int partLimit = 0;
	// some parts can only be installed as the nth part of the turn (0 for no restriction)
	public int requiredPart = 0;
	// check for different card types
	public SpecialCards SpecialCards = SpecialCards.normal;
	public string name, description;
	public CardProperties(Card card) {
		inputs = card.inputs;
		outputs = card.outputs;
		isTool = card.isTool;
		winsGame = card.winsGame;
		singleUse = card.singleUse;
		scaleable = card.scaleable;
		partLimit = card.partLimit;
		requiredPart = card.requiredPart;
		SpecialCards = SpecialCards.normal;
		if (card is CardExtension) SpecialCards = SpecialCards.extension;
		name = card.cardName;
		description = card.cardComponents.description;
	}
	public void ApplyToCard(Card card) {
		card.inputs = inputs;
		card.outputs = outputs;
		card.isTool = isTool;
		card.winsGame = winsGame;
		card.singleUse = singleUse;
		card.scaleable = scaleable;
		card.partLimit = partLimit;
		card.requiredPart = requiredPart;
		card.cardName = name;
		card.cardComponents.description = description;
	}
}