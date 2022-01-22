using UnityEngine;
using System;
// an object representing a card moving from one place to another
// these are created when the card moves within game logic, and then fired when the card is ready to display this motion
public class CardAnimation {

    public Card controlledCard{get;private set;}
    public GameAction gameAction{get;private set;}
    MotionPlan motionPlan; // pre-calculated information about the motion
    Bounds origin,goal;
    float startTime;
    public Zone originZone{get;private set;}
    public Zone goalZone  {get;private set;}
    // if assigned, allows animations to be queued to perform after the current one
    public CardAnimation nextAnimation = null;
    public CardAnimation(Card controlledCard,GameAction gameAction) {
        this.controlledCard = controlledCard;
        goal = controlledCard.bounds;
        this.gameAction = gameAction;
        originZone = OriginZone(gameAction);
        goalZone   = GoalZone  (gameAction);
    }
    // start playing the animation
    public void Fire() {
        // if the card is already animating, put this animation in that card's queue instead of firing it
        if (controlledCard.currentAnimation != null) {
            CardAnimation animation = controlledCard.currentAnimation;
            while (animation.nextAnimation != null) animation = animation.nextAnimation;
            animation.nextAnimation = this;
        } else {
            // reduce cards shuffling around within hand as hand is repacked
            if (goalZone==Zone.Hand) {
                goal = controlledCard.finalHandBounds;
            }
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
        float acceleration = Game.S.vMax/Game.S.accTime;
        float deceleration = Game.S.vMax/Game.S.decTime;
        float timeDiff = time - startTime;
        if (gameAction==GameAction.Repacking) {
            acceleration *= 0.5f;
            deceleration *= 0.5f;
        }
        // Tween expects motion constants to be scaled by distance covered, then scaled back up
        return Tween.PercentAtTime(
            acceleration/motionPlan.distance,
            deceleration/motionPlan.distance,
             Game.S.vMax/motionPlan.distance,
            timeDiff);
    }
    Zone OriginZone(GameAction action) {
        switch (action) {
            case GameAction.Drawing:
                return Zone.Deck;
            case GameAction.Installing:
                return Zone.Hand;
            case GameAction.Uninstalling:
                return Zone.Play;
            case GameAction.ClockReturn:
                return Zone.Play;
            case GameAction.Repacking:
                return controlledCard.zone;
            case GameAction.DiscardReturn:
                return Zone.Junk;
            case GameAction.DrawReverse:
                return Zone.Hand;
            case GameAction.Discarding:
                return Zone.Hand;
            default:
                return Zone.Junk;
        }
    }
    Zone GoalZone(GameAction action) {
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