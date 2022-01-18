using UnityEngine;
using UnityEditor;
public enum Zone {
	Deck,
	Hand,
	Play,
	Junk
}
[ExecuteAlways]
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
	void Update() {
#if UNITY_EDITOR
		// editor code:
		if (!EditorApplication.isPlaying) {
			S = this;
		}
#endif
		AnimationHandler.Update();
	}
	public int startingHandSize;
	void Start() {
		for (int i = 0; i < startingHandSize; i++)
		{
			DrawCard();
		}
	}
	public Card[] cards;
	public static Vector2 cardsize
	{
		get
		{
			return (Vector2)S.cards[0].gameObject.GetComponent<SpriteRenderer>().bounds.size;
		}
	}
	public ZoneTracker zoneTracker;
	public static void GameStateChanged() {
		CardPlayable.GameStateChanged();
		S.zoneTracker.GameStateChanged();
	}
	public void DrawCard() {
		Card drawnCard = zoneTracker.DrawCard();

	}
	public void DiscardCard() {}
	public bool ReversibleMode = false;
}