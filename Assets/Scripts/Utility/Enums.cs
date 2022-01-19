// a list of game actions that would require cards to move:
public enum GameAction {
    Drawing,
    Installing,
    Uninstalling,
    ClockReturn,
    Repacking,
    DiscardReturn,
    DrawReverse,
    Discarding
}
public enum Zone {
	Deck,
	Hand,
	Play,
	Junk
}
public enum Resource {
	None = 0,
	Card,
	Battery,
	Electric,
	Coal,
	Heat,
	Scrap,
	Metal,
	Distance,
	Recon,
	Time
}
// whether a card can be played from the hand in the current gamestate
public enum Playability {
	Playable=1,
	Almost=2,
	Unplayable=3
}
