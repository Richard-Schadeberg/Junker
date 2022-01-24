using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class CardCopier {
    public static void CreateCopy(Card card) {
            // copy needs to be flagged "isCopy" before instantiation, so it doesn't create unnecessary icon prefabs
            // since it clones card, this is done by making card isCopy and then reverting
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
            // add the copy to hand
            Game.S.zoneTracker.handContents.AddCard(copy);
            // no discarding temporary copies
            copy.noDiscard = true;
            // track copy so it's deleted when card returns to hand
            card.tempCopy = copy;
            // add "Temporary Copy" to copy, if it doesn't already have it
            if (!card.isCopy) copy.cardComponents.description = copy.cardComponents.description + "\n<i>Temporary Copy</i>";
            // make copy reflect new state. It should already be in the right size/place
            copy.SetColour();
    }
    public static void DeleteCopy(Card card) {
        if (card.tempCopy!=null) {
            // no deleting copies from play
            if (card.tempCopy.zone!=Zone.Play) {
                ZoneTracker.MoveCard(card.tempCopy,Zone.Hand,Zone.Junk);
                // copy is properly deleted by CardAnimation after being moved to Junk
                AnimationHandler.Animate(card.tempCopy,GameAction.DeleteScaleable);
            }
        }
    }
}
