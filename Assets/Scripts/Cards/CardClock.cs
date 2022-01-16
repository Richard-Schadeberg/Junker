using System.Collections.Generic;
using System;

public class CardClock : Card
{
	protected override void AfterInstall() {
		queueOther=true;
		queueZone=Zone.Play;
	}
	protected override void QueuedOther() {
		queueOther=false;
		changes.Clear();
		// return installed parts to hand
		List<Card> returnList = new List<Card>(Game.zoneCards[Zone.Play]);
		foreach (Card card in returnList) {
			card.Return();
			if (!(card.noDiscard||card.singleUse)) {
				Game.Remove(Resource.Card);
			}
		}
		Game.TestCardsStatus();
		returning=true;
	}
	protected override void AfterOutput() {
		// reset the 5 main resources
		Reset(Resource.Electric);
		Reset(Resource.Heat);
		Reset(Resource.Metal);
		Reset(Resource.Distance);
		Reset(Resource.Recon);
		// add metal for each scrap
		changes.Push(new Tuple<Resource,int>(Resource.Metal,Game.resources[Resource.Scrap]));
		Game.Add(Resource.Metal,Game.resources[Resource.Scrap]);
		// add a Card for each discardable part returned
		foreach (Card card in Game.zoneCards[Zone.Play]) {
			if (card.noDiscard||card.singleUse) {
				if (card.singleUse) {
				}
			} else {
				changes.Push(new Tuple<Resource,int>(Resource.Card,1));
				Game.Add(Resource.Card);
			}
		}
	}
	protected override void BeforeUndoOutput() {
		while (changes.Count!=0) {
			Tuple<Resource,int> change = changes.Pop();
			Game.Remove(change.Item1,change.Item2);
		}
	}
	Stack<Tuple<Resource,int>> changes = new Stack<Tuple<Resource,int>>();
	private void Reset(Resource resource) {
		changes.Push(new Tuple<Resource,int>(resource,-Game.resources[resource]));
		Game.Remove(resource,Game.resources[resource]);
	}
}
