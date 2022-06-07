using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class CardCopier {
    public static void CreateCopy(Card card) {
        // copy needs to be flagged "isCopy" before instantiation, so it doesn't create unnecessary icon prefabs
        // since it clones card, this is done by making card isCopy and then reverting
        // TODO: check if this is still necessary, as cardComponents doesn't seem to use isCopy
        // sometimes card is already a copy
        bool wasCopy = card.isCopy;
        card.isCopy = true;
        Card copy = MonoBehaviour.Instantiate(card);
        card.isCopy = wasCopy;
        // Instantiate renames object
        copy.cardName = card.cardName;
        // add the copy to Game.cards
        Card[] cardsCopy = new Card[Game.S.cards.Length+1];
        Array.Copy(Game.S.cards,cardsCopy,Game.S.cards.Length);
        Game.S.cards = cardsCopy;
        Game.S.cards[Game.S.cards.Length-1] = copy;
        // no discarding temporary copies
        copy.isTool = true;
        // add the copy to hand (add rather than move, as zonetracker doesn't know it exists yet)
        Game.S.zoneTracker.handContents.AddCard(copy);
        copy.zone = Zone.Hand;
        // track copy so it's deleted when card returns to hand
        card.tempCopy = copy;
        // add "Temporary Copy" to copy's card text, if the original didn't already have it
        if (!card.isCopy) copy.cardComponents.description = copy.cardComponents.description + "\n<i>Temporary Copy</i>";
        // darken/brighten the copy's inputs/outputs
        copy.UpdateColour();
    }
    // recursively delete the temporary copies of a card
    // newest copy is deleted first
    // currently no animation
    public static void DeleteChild(Card card) {
        if (card.hasCopy) {
            Card child = card.tempCopy;
            // recur
            DeleteChild(child);
            // move card to Junk zone, to ensure its current zone knows it's gone
            ZoneTracker.MoveCard(child, child.zone,Zone.Junk);
            // delete card from Junk zone
            Game.S.zoneTracker.junkContents.RemoveCard(child);
            // delete card from Game's list of cards
            List<Card> cards = Game.S.cards.ToList();
            cards.Remove(child);
            Game.S.cards = cards.ToArray();
            // destroy the gameobject and its children
            MonoBehaviour.Destroy(child.gameObject);
            // note on the parent that the child is gone
            card.tempCopy = null;
        }
    }
}
