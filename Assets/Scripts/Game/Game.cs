using UnityEngine;
using UnityEditor;
public enum Zone {
	Deck,
	Hand,
	Play,
	Junk
}
public class Game : MonoBehaviour {
// Game needs to be instantiated in order to work with the unity inspector,
//  but its functionality is otherwise static.
//  the solution is to store a static pointer to the instance, to allow
//  static methods to access the instance's properties.
	public static Game S;
	void Awake() {
		S = this;
		zoneTracker = new ZoneTracker(cards);
	}
    // time (in seconds) between each animation fired from the queue
    public float chainTime = 0.2f;
    // tweening constants
 	public float vMax      = 12000;
	public float accTime   =  0.2f;
	public float decTime   =  0.4f;
	void Update() {
		animationHandler.Update();
	}
	public int startingHandSize;
	public AnimationHandler animationHandler = new AnimationHandler();
	public CardMovement cardMovement = new CardMovement();
	public ResourceTracker resourceTracker = new ResourceTracker();
	void Start() {
		foreach (Card card in cards) {
			animationHandler.AnimateInstant(card,GameAction.Repacking);
		}
		for (int i = 0; i < startingHandSize; i++)
		{
			DrawCard();
		}
	}
	public Card[] cards;
	public static Vector2 cardAspectRatio{get {return (Vector2)S.cards[0].gameObject.GetComponent<SpriteRenderer>().bounds.size;}}
	public ZoneTracker zoneTracker;
	public static void GameStateChanged() {
		if (Game.S.ReversibleMode) return;
		CardPlayable.GameStateChanged();
		S.zoneTracker.GameStateChanged();
	}
	public void DrawCard() {
		Card drawnCard = zoneTracker.DrawCard();
		if (drawnCard==null) return;
		GameStateChanged();
		animationHandler.Animate(drawnCard,GameAction.Drawing);
	}
	public void DiscardCard() {}
	public bool ReversibleMode = false;
}