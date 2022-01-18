using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardMovement
{
    public struct CardAction {
        Card card;
        Zone from;
        Zone to;
        public CardAction(Card card,Zone from,Zone to) {
            this.card = card;
            this.from = from;
            this.to = to;
        }
    }
    public static Stack<CardAction> reversibleActions = new Stack<CardAction>();
    public static void MoveReversible(Card card,Zone zone) 
    {
        reversibleActions.Push(new CardAction(card,card.zone,zone));
        Game.S.zoneTracker.MoveCard(card,card.zone,zone);
        card.zone = zone;
    }
}
