using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO: either expand this class or combine it with another
public static class GameActions {
	// Draw cards and credit it to the passed card, or none if null
	public static void DrawCards(int num,Card credit) {
		if (num==0) return;
		List<Card> drawns = new List<Card>();
		for (int i=0;i<num;i++) {
			Card drawn = ZoneTracker.TopCard();
			// if there are no cards in deck, stop drawing
			if (drawn==null) break;
			ZoneTracker.MoveCard(drawn,Zone.Deck,Zone.Hand);
			drawns.Add(drawn);
			// record which card drew this card so it can be reversed later
			if (credit!=null) credit.credits.Draw(drawn);
		}
		if (!Game.S.ReversibleMode) AnimationHandler.Animate(drawns.ToArray(),GameAction.Drawing);
	}
}
