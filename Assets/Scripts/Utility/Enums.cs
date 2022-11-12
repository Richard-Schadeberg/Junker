// a list of game actions that would require cards to move:
public enum GameAction {
    Installing,
    Uninstalling,
    Drawing,
    DrawReverse,
    Discarding,
    DiscardReturn,
    Repacking,
    ClockReturn
}
public enum Zone {
	Deck,
	Hand,
	Play,
	Junk
}
public enum Resource {
	Electric,
	Distance,
	Recon,
	Fuel,
	Metal,
	Battery,
	Card,
	None
}
// whether a card can be played from the hand in the current gamestate
public enum Playability {
	Playable=1,
	Almost=2,
	Unplayable=3
}
public enum SpecialCards {
	normal,
	extension
}
