using UnityEngine;
using UnityEditor;
public class Game : MonoBehaviour {
	public int startingHandSize,startingTime;
// Game needs to be instantiated in order to work with the unity inspector,
//  but its functionality is otherwise static.
//  the solution is to store a static pointer to the instance, to allow
//  static methods to access the instance's properties.
	public static Game S;
	void Awake() {
		S = this;
	}
    // time (in seconds) between each animation fired from the queue
    public float chainTime = 0.2f;
    // tweening constants
 	public float vMax      = 12000;
	public float accTime   =  0.2f;
	public float decTime   =  0.4f;
	public float startDelay=  0.2f;
	void Update() {
		animationHandler.Update();
		// Debug.Log(discardRequester.pendingRequests);
	}
	public AnimationHandler animationHandler = new AnimationHandler();
	public CardMovement cardMovement = new CardMovement();
	public ResourceTracker resourceTracker = new ResourceTracker();
	public DiscardRequester discardRequester = new DiscardRequester();
	public ZoneTracker zoneTracker;
	void Start() {
		zoneTracker = new ZoneTracker(cards);
		ResourceTracker.Add(Resource.Time,startingTime);
		foreach (Card card in cards) {
			animationHandler.AnimateInstant(card,GameAction.Repacking);
		}
		for (int i = 0; i < startingHandSize; i++)
		{
			DrawCard();
		}
		animationHandler.WaitSeconds(startDelay);
	}
	public Card[] cards;
	public static Vector2 cardAspectRatio{get {return (Vector2)S.cards[0].gameObject.GetComponent<SpriteRenderer>().bounds.size;}}
	public static void GameStateChanged() {
		if (Game.S.ReversibleMode) return;
		CardPlayable.GameStateChanged();
		S.zoneTracker.GameStateChanged();
		foreach (Card card in S.cards) {
			card.SetColour();
		}
	}
	public void DrawCard() {
		Card drawnCard = zoneTracker.DrawCard();
		if (drawnCard==null) return;
		GameStateChanged();
		AnimationHandler.Animate(drawnCard,GameAction.Drawing);
	}
	public void DiscardCard() {}
	public bool ReversibleMode = false;
}