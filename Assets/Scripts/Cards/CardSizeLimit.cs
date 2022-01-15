using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSizeLimit : Card
{
    public int maxSize;
	public override bool IsLegal() {
		if (zone!=Zone.Play) return true;
		return (Game.zoneCards[zone].Count<=maxSize);
	}
}
