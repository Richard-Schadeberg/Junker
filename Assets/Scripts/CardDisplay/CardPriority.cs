using UnityEngine;
using System.Linq;
public static class CardPriority {
	// low priority number cards go on the left
	// rarely moved cards (tools) are kept on the left to reduce hand rearranging
	// otherwise players will usually think about cards from left to right
	public static int Priority(Card card) {
		int[] factors = new int[8];
		// most to least significant factors listed top to bottom
		// never leaves hand until the game is over
		factors[7]=(card.winsGame?0:1);
		factors[6] = 0; // deprecated
		// only leaves hand to be played (can't be discarded)
		factors[5]=(card.isTool?0:1);
		// arrange other cards in roughly the order they'll be played in
		factors[4]=(int)card.Playability;
		// simpler cards on left
		factors[3]=card.inputs.Length;
		factors[2]=card.outputs.Length;
		// unique mechanics on right
		factors[1]=new []{card.scaleable,card.partLimit>0}.Count(x=>x);
		// finally, break ties using ID, as ties can cause strange behaviour
		factors[0]=(int)Mathf.Abs(card.gameObject.GetInstanceID()%100);
		int priority=0;
		int significance=1;
		foreach(int x in factors) {
			priority += significance*x;
			// more significant factors add more to priority, without going over the int limit
			significance *= (int)Mathf.Pow(2,(Mathf.Floor((float)30/factors.Length)));
		}
		return (priority);
	}
}