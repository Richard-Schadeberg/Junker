using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CardChanger : Card
{
	public Zone[] zones;
	public Resource[] newInputs,newOutputs;
	public bool affectInput,affectOutput,addToExisting;
	protected override void AfterOutput() {
		foreach (Zone zone in zones) {
			foreach (Card card in Game.zoneCards[zone]) {
				if (card!=this) {
					oldValues.Push(new Tuple<Card,Resource[],Resource[]>(card,card.inputs,card.outputs));
					if (!addToExisting) {
						if (affectInput)  card.inputs  = newInputs;
						if (affectOutput) card.outputs = newOutputs;
					} else {
						if (affectInput) {
							card.inputs =  ((card.inputs.ToList() ).Concat(newInputs.ToList() )).ToArray();
						}
						if (affectOutput) {
							card.outputs = ((card.outputs.ToList()).Concat(newOutputs.ToList())).ToArray();
						}
					}
				}
			}
		}
	}
	protected override void BeforeUndoOutput() {
		while (oldValues.Count>0) {
			Tuple<Card,Resource[],Resource[]> oldValue = oldValues.Pop();
			oldValue.Item1.inputs  = oldValue.Item2;
			oldValue.Item1.outputs = oldValue.Item3;
		}
	}
	protected override void AfterInstall() {
		foreach (Zone zone in zones) {
			foreach (Card card in Game.zoneCards[zone]) {
				card.BuildCard();
			}
		}
	}
	protected override void AfterUninstall() {
		AfterInstall();
	}
	private Stack<Tuple<Card,Resource[],Resource[]>> oldValues = new Stack<Tuple<Card,Resource[],Resource[]>>();
	protected override void AfterReturn() {
		oldValues.Clear();
	}
}
