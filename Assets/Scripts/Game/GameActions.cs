using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameActions {
	// Draw cards and credit it to the passed card, or none if null
	public static void DrawCards(int num,Card credit) {
		if (num==0) return;
		List<Card> drawns = new List<Card>();
		for (int i=0;i<num;i++) {
			Card drawn = ZoneTracker.TopCard();
			if (drawn==null) break;
			ZoneTracker.MoveCard(drawn,Zone.Deck,Zone.Hand);
			drawns.Add(drawn);
			if (credit!=null) credit.credits.Draw(drawn);
		}
		if (!Game.S.ReversibleMode) AnimationHandler.Animate(drawns.ToArray(),GameAction.Drawing);
	}
}
