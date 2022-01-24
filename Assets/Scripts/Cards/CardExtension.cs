using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CardExtension : Card {
    Resource[] originalInputs,originalOutputs;
    // copy-paste from base
    void Start() {
        originalInputs = inputs;
        originalOutputs = outputs;
		cardComponents.DisplayInputsOutputs(inputs,outputs);
		cardComponents.SetLayers(gameObject);
		cardComponents.cardName = cardName;
    }
    public static void UpdateExtensions() {
        foreach (Card card in ZoneTracker.GetCards(Zone.Hand)) if (card is CardExtension) (card as CardExtension).UpdateExtension();
    }
    public void UpdateExtension() {
        Card[] cards = ZoneTracker.GetCardsLeftToRight(Zone.Play);
        // if nothing to copy, revert to original
        if (cards.Length==0) {
            inputs = originalInputs;
            outputs = originalOutputs;
        } else {
            // most recently installed part
            Card top = cards[cards.Length-1];
            inputs  = originalInputs. Concat(top.inputs). ToArray();
            outputs = originalOutputs.Concat(top.outputs).ToArray();
        }
        // no need to validate if inputs/outputs is the right length
        cardComponents.DisplayInputsOutputs(inputs,outputs);
    }
}
