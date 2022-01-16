using System.Linq;

public class CardNoWin : Card
{
    public override bool IsLegal() {
		if (zone!=Zone.Play) return true;
		return (!Game.zoneCards[zone].Any(x=>x.winsGame));
	}
}
