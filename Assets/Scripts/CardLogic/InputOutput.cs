using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputOutput {
    public static void Input(Card card) {
        foreach (Resource input in card.inputs) {
            if (input == Resource.Card) {
                DiscardRequester.RequestDiscard(card);
            } else {
                ResourceTracker.Remove(input);
                if (input == Resource.Metal) ResourceDisplay.Update(Resource.Metal);
                if (input == Resource.Electric && ResourceTracker.Get(Resource.Electric) < 0 && ResourceTracker.Get(Resource.Battery) > 0) {
                    ResourceTracker.Add(Resource.Electric);
                    ResourceTracker.Remove(Resource.Battery);
                    card.batteryConversions++;
                    if (!Game.S.ReversibleMode) IconTracker.OutputConsumed(Resource.Battery);
                } else {
                    if (!Game.S.ReversibleMode) IconTracker.OutputConsumed(input);
                }
            }
        }
    }
    public static void UndoInput(Card card) {
        foreach (Resource input in card.inputs) {
            if (input == Resource.Card) {
                DiscardRequester.CancelRequest();
                if (!Game.S.ReversibleMode) card.credits.UndoDiscards();
            } else {
                ResourceTracker.Add(input);
                if (input == Resource.Metal) ResourceDisplay.Update(Resource.Metal);
                if (input == Resource.Electric && card.batteryConversions>0) {
                    card.batteryConversions--;
                    ResourceTracker.Add(Resource.Battery);
                    ResourceTracker.Remove(Resource.Electric);
                    if (!Game.S.ReversibleMode) IconTracker.OutputReturned(Resource.Battery);
                } else {
                    if (!Game.S.ReversibleMode) IconTracker.OutputReturned(input);
                }
            }
        }
        Game.PlayerActionResolved();
    }
    public static void Output(Card card) {
        int draws=0;
        int i=0;
        foreach (Resource output in card.outputs) {
            if (output == Resource.Card) {
                draws++;
            } else {
                ResourceTracker.Add(output);
                if (output == Resource.Metal) ResourceTracker.scrap++;
            }
            if (!Game.S.ReversibleMode) IconTracker.OutputInstalled(output, card.cardComponents.outputIcons[i]);
            i++;
        }
        GameActions.DrawCards(draws,card);
        Game.PlayerActionResolved();
    }
    public static void UndoOutput(Card card) {
        foreach (Resource output in card.outputs) {
            if (output == Resource.Card) {
                card.credits.UndoDraws();
            } else {
                ResourceTracker.Remove(output);
                if (output == Resource.Metal) ResourceTracker.scrap--;
            }
            if (!Game.S.ReversibleMode) IconTracker.OutputUninstalled(output);
        }
    }
}
