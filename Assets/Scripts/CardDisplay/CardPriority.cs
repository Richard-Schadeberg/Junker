using UnityEngine;
using System.Linq;
public static class CardPriority
{
	public static int Priority(Card card) {
		int[] factors = new int[8];
		factors[7]=(card.winsGame?0:1);
		factors[6]=(card.inputs.Contains(Resource.Time)?0:1);
		factors[5]=(card.noDiscard?0:1);
		factors[4]=(int)card.Status;
		factors[3]=card.inputs.Length;
		factors[2]=card.outputs.Length;
		factors[1]=new []{card.singleUse,card.startsHand,card.scaleable}.Count(x=>x);
		factors[0]=(int)Mathf.Abs(card.gameObject.GetInstanceID()%100);
		int priority=0;
		int significance=1;
		foreach(int x in factors) {
			priority += significance*x;
			significance *= (int)Mathf.Pow(2,(Mathf.Floor((float)30/factors.Length)));
		}
		return (priority);
	}

}
