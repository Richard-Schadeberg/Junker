using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// handles the inputs (costs) and outputs (effects) when playing a part
// all inputs and outputs are fully reversible until the turn is ended
public static class InputOutput {
    // pay the inputs on a card
    // reversible actions only
    public static void Input(Card card) {
        foreach (Resource input in card.inputs) {
            if (input == Resource.Card) {
                DiscardRequester.RequestDiscard(card);
            } else {
                ResourceTracker.Remove(input);
                // metal has a UI counter
                if (input == Resource.Metal) ResourceDisplay.Update(Resource.Metal);
                // if a battery need to be converted into electric to pay this cost
                if (input == Resource.Electric && ResourceTracker.Get(Resource.Electric) < 0 && ResourceTracker.Get(Resource.Battery) > 0) {
                    ResourceTracker.Add(Resource.Electric);
                    ResourceTracker.Remove(Resource.Battery);
                    card.batteryConversions++;
                    // dim a battery icon on an installed card to indicate a battery was spent
                    if (!Game.S.ReversibleMode) IconTracker.OutputConsumed(Resource.Battery);
                } else {
                    // dim the icon of the resource spent, if applicable
                    if (!Game.S.ReversibleMode) IconTracker.OutputConsumed(input);
                }
            }
        }
    }
    // reverse all costs paid for the card
    public static void UndoInput(Card card) {
        foreach (Resource input in card.inputs) {
            if (input == Resource.Card) {
                // number of discards needed to undo is unpredictable, best to just clear all of them
                card.credits.UndoDiscards();
                // if the game is waiting for the player to select cards to discard, stop waiting
                DiscardRequester.CancelRequest();
            } else {
                ResourceTracker.Add(input);
                // metal has a UI counter
                if (input == Resource.Metal) ResourceDisplay.Update(Resource.Metal);
                // if batteries were converted, convert them back
                if (input == Resource.Electric && card.batteryConversions>0) {
                    card.batteryConversions--;
                    ResourceTracker.Add(Resource.Battery);
                    ResourceTracker.Remove(Resource.Electric);
                    // handle dimming of battery icons
                    if (!Game.S.ReversibleMode) IconTracker.OutputReturned(Resource.Battery);
                } else {
                    // undo icon dimming from resource spending
                    if (!Game.S.ReversibleMode) IconTracker.OutputReturned(input);
                }
            }
        }
    }
    public static void Output(Card card) {
        // draws are processed as a group outside the loop
        int draws=0;
        // track the index of the output so its corresponding icon can be tracked
        int outputIndex=0;
        foreach (Resource output in card.outputs) {
            if (output == Resource.Card) {
                draws++;
            } else {
                ResourceTracker.Add(output);
                // at the end of each turn, all spent metal is returned
                // scrap tracks how much metal the player has if the spent metal is included
                if (output == Resource.Metal) ResourceTracker.scrap++;
            }
            // record this resource's icon so that it can be dimmed when spent
            if (!Game.S.ReversibleMode) IconTracker.OutputInstalled(output, card.cardComponents.outputIcons[outputIndex]);
            outputIndex++;
        }
        GameActions.DrawCards(draws,card);
    }
    public static void UndoOutput(Card card) {
        foreach (Resource output in card.outputs) {
            if (output == Resource.Card) {
                // no need to only undo the exact number of draws
                card.credits.UndoDraws();
            } else {
                ResourceTracker.Remove(output);
                if (output == Resource.Metal) ResourceTracker.scrap--;
            }
            // this resource's icon can no longer be dimmed to pay for inputs
            if (!Game.S.ReversibleMode) IconTracker.OutputUninstalled(output);
        }
    }
}
