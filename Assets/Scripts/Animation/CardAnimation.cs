using UnityEngine;
using System;
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
// an object representing a card moving from one place to another
// these are created when the card moves within game logic, and then fired when the card is ready to display this motion
public class CardAnimation {
    // tweening constants
 	const float vMax      = 12000;
	const float accTime   =  0.2f;
	const float decTime   =  0.4f;
	const float chainTime =  0.2f;

    public Card controlledCard;
    GameAction gameAction;
    MotionPlan motionPlan; // pre-calculated information about the motion
    Bounds origin,goal;
    float startTime;
    bool fired = false;
    // if assigned, allows animations to be queued to perform after the current one
    CardAnimation nextAnimation = null;
    public CardAnimation(Card controlledCard,GameAction gameAction) {
        Zone destination = Destination(gameAction);
        goal = controlledCard.bounds;
        this.gameAction = gameAction;
        fired = false;
    }
    // start playing the animation
    public void Fire() {
        origin = controlledCard.gameObject.GetComponent<SpriteRenderer>().bounds;
        startTime = Time.time;
        motionPlan = new MotionPlan(origin.center,goal.center,gameAction);
        fired = true;
        controlledCard.currentAnimation = this;
    }
    // called every frame during the animation by the card it's attached to
    public void Update() {
		float percentTravelled = PercentAtTime(Time.time);
        bool motionComplete = (Single.IsNaN(percentTravelled)||motionPlan.distance==0);
        // if animation has been completed or is already at the destination:
		if (motionComplete) percentTravelled = 1.0f;
        Vector2 position = MotionPlanPercent.PositionAtPercentage(motionPlan,percentTravelled);
        Vector2 size = Vector2.Lerp(origin.size,goal.size,percentTravelled);
		BoundsUtil.SetBounds(controlledCard,new Bounds(position,size));
        if (motionComplete) MotionComplete();
    }
    void MotionComplete() {
        if (nextAnimation != null) nextAnimation.Fire();
    }
    float PercentAtTime(float time) {
        float acceleration = vMax/accTime;
        float deceleration = vMax/decTime;
        float timeDiff = time - startTime;
        // Tween expects motion constants to be scaled by distance covered, then scaled back up
        return Tween.PercentAtTime(
            acceleration/motionPlan.distance,
            deceleration/motionPlan.distance,
                    vMax/motionPlan.distance,
            timeDiff);
    }
    Zone Destination(GameAction action) {
        switch (action) {
            case GameAction.Drawing:
                return Zone.Hand;
            case GameAction.Installing:
                return Zone.Play;
            case GameAction.Uninstalling:
                return Zone.Hand;
            case GameAction.ClockReturn:
                return Zone.Hand;
            case GameAction.Repacking:
                return controlledCard.zone;
            case GameAction.DiscardReturn:
                return Zone.Hand;
            case GameAction.DrawReverse:
                return Zone.Deck;
            case GameAction.Discarding:
                return Zone.Junk;
            default:
                return Zone.Junk;
        }
    }
}