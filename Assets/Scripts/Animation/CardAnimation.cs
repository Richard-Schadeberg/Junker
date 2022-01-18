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
    public GameAction gameAction;
    MotionPlan motionPlan; // pre-calculated information about the motion
    Bounds origin,goal;
    float startTime;
    // if assigned, allows animations to be queued to perform after the current one
    public CardAnimation nextAnimation = null;
    public CardAnimation(Card controlledCard,GameAction gameAction) {
        this.controlledCard = controlledCard;
        goal = controlledCard.bounds;
        this.gameAction = gameAction;
    }
    // start playing the animation
    public void Fire() {
        // if the card is already animating, put this animation in that card's queue instead of firing it
        if (controlledCard.currentAnimation != null) {
            CardAnimation animation = controlledCard.currentAnimation;
            while (animation.nextAnimation != null) animation = animation.nextAnimation;
            animation.nextAnimation = this;
        } else {
            origin = controlledCard.gameObject.GetComponent<SpriteRenderer>().bounds;
            startTime = Time.time;
            motionPlan = new MotionPlan(origin.center,goal.center,gameAction);
            controlledCard.currentAnimation = this;
        }
    }
    // called every frame during the animation by the card it's attached to
    public void Update() {
		float percentTravelled = PercentAtTime(Time.time);
        if (percentTravelled > 1f || float.IsNaN(percentTravelled)) percentTravelled = 1f;
        // if animation has been completed:
        Vector2 position = MotionPlanPercent.PositionAtPercentage(motionPlan,percentTravelled);
        Vector2 size = Vector2.Lerp(origin.size,goal.size,percentTravelled);
		BoundsUtil.SetBounds(controlledCard,new Bounds(position,size));
        if (percentTravelled==1f) MotionComplete();
    }
    void MotionComplete() {
        controlledCard.currentAnimation=null;
        // if another animation tried to fire but was blocked by this one, fire it now
        if (nextAnimation != null) {nextAnimation.Fire();}
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
}