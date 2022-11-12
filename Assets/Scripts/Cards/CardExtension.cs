using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CardExtension : Card {
    Resource[] originalInputs,originalOutputs;
    public override void Start() {
        originalInputs = inputs;
        originalOutputs = outputs;
        base.Start();
    }
    // Static function called whenever gamestate changes that updates each extension
    public static void UpdateExtensions() {foreach (Card card in ZoneTracker.GetCards(Zone.Hand)) if (card is CardExtension) ((CardExtension)card).CopyMostRecentPart();}
    // make the extension a copy of the rightmost installed part, plus the extensions original inputs/outputs
    public void CopyMostRecentPart() {
        Card[] cards = ZoneTracker.GetCardsLeftToRight(Zone.Play);
        // sometimes this gets called before Start()
        if (originalInputs  == null) originalInputs  = inputs;
        if (originalOutputs == null) originalOutputs = outputs;
        // if nothing to copy, revert to original
        if (cards.Length==0) {
            inputs = originalInputs;
            outputs = originalOutputs;
        } else {
            // most recently installed part
            Card top = cards[cards.Length-1];
            // inputs/outputs trimmed to max amount displayable on card
            inputs  = originalInputs. Concat(top.inputs). Take(Define.maxInputs). ToArray();
            outputs = originalOutputs.Concat(top.outputs).Take(Define.maxOutputs).ToArray();
        }
        // update graphical display
        cardComponents.DisplayInputsOutputs(inputs,outputs);
    }

}
