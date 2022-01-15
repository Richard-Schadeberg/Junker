using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardCopier : Card
{
	void Update() {
		if (animating) TravelRoute();
		if (queueRebuild) {
			queueRebuild=false;
			BuildCard();
		}
	}
	public Resource[] originalInputs,originalOutputs;
	public Card copiedCard=null;
	public bool queueRebuild=false;
	protected override void BeforeInput() {
		if (copiedCard==null) { // should be set in Start(), but this is easier
			originalInputs=inputs;
			originalOutputs=outputs;
		}
		int myIndex = Game.zoneCards[Zone.Play].IndexOf(this); // Input() is called after card in in play
		if (myIndex==0) {
			ResetCard();
		} else {
			CopyCard(Game.zoneCards[Zone.Play][myIndex-1]);
		}
	}
	private void ResetCard() {
		if (inputs!=originalInputs||outputs!=originalOutputs) {
			inputs = originalInputs;
			outputs= originalOutputs;
			copiedCard=null;
			queueRebuild=true;
		}
	}
	private void CopyCard(Card card) {
		if (card!=copiedCard) {
			copiedCard=card;
			List<Resource> rList;
			rList = card.inputs.ToList();
			rList = rList.Concat(originalInputs).ToList();
			inputs = rList.ToArray();
			
			rList = card.outputs.ToList();
			outputs = rList.ToArray();
			queueRebuild=true;
		}
	}
}
