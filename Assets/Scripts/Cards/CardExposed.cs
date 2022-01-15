using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardExposed : Card
{
	public int requiredPlace;
    public override bool IsLegal() {
		if (zone!=Zone.Play) return true;
		return (Game.zoneCards[zone].IndexOf(this)==requiredPlace-1);
	}
}
