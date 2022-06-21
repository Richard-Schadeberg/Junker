using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
// an object representing a card moving from one place to another
// these are created when the card moves within game logic, and then fired when the card is ready to display this motion
public class CardAnimation {

    public Card controlledCard{get;private set;}
    public GameAction gameAction{get;private set;}
    private MotionPlan motionPlan; // pre-calculated information about the motion
    private Bounds originBounds,goalBounds;
    private float startTime; // in seconds
    public readonly Zone originZone;
    public readonly Zone goalZone; // currently redundant
    // allows animations to be queued to perform after the current one
    public CardAnimation nextAnimation = null;
    public CardAnimation(Card controlledCard,GameAction gameAction) {
        this.controlledCard = controlledCard;
        // the goal is to make the card's size and position reflect the gamestate at the time CardAnimation is created
        goalBounds = controlledCard.goalBoundsForCurrentGamestate;
        this.gameAction = gameAction;
        originZone = OriginZone(gameAction);
        goalZone   = GoalZone  (gameAction);
    }
    // start playing the animation
    public void Fire() {
        // if the attached card has been deleted, do nothing and the next queued animation will fire in a bit
        if (controlledCard == null) {
            // hack to make the next animation fire
            Game.S.animationHandler.PauseAnimations(-200);
            return;
        }
        // if the card is already animating, add this animation to that animation's queue instead of firing it
        // it will fire again when the card is ready for this animation
        if (controlledCard.currentAnimation != null) {
            CardAnimation lastAnimation = controlledCard.currentAnimation;
            // find end of queue
            while (lastAnimation.nextAnimation != null) lastAnimation = lastAnimation.nextAnimation;
            lastAnimation.nextAnimation = this;
        } else {
            // where the card visually is when the animation starts
            originBounds = controlledCard.gameObject.GetComponent<SpriteRenderer>().bounds;
            startTime = Time.time;
            motionPlan = new MotionPlan(originBounds.center,
                                          goalBounds.center,
                                          gameAction);
            controlledCard.currentAnimation = this;
        }
    }
    // called every frame during the animation by the card it's attached to
    public void UpdatePositionAndSize() {
		float percentTravelled = PercentAtTime(Time.time);
        // if the motion was overshot or invalid (zero distance), move the card to its destination
        if (percentTravelled > 1f || float.IsNaN(percentTravelled)) percentTravelled = 1f;
        Vector2 position = MotionPlanPercent.PositionAtPercentage(motionPlan,percentTravelled);
        Vector2 size     = Vector2.Lerp(originBounds.size,goalBounds.size,   percentTravelled);
		BoundsUtil.MoveAndScaleCardToBounds(controlledCard,new Bounds(position,size));
        if (percentTravelled == 1f) {
            controlledCard.currentAnimation = null;
            // if another animation tried to fire but was blocked by this one, fire it now
            if (nextAnimation != null) { nextAnimation.Fire(); }
        }
    }
    // how far through the animation the card should be at a given time
    float PercentAtTime(float time) {
        float acceleration = Game.S.vMax/Game.S.accTime;
        float deceleration = Game.S.vMax/Game.S.decTime;
        float timeDiff     = time - startTime;
        // halve acceleration during repacking, otherwise it looks jarring
        if (gameAction==GameAction.Repacking) {
            acceleration *= 0.5f;
            deceleration *= 0.5f;
        }
        // Tween expects motion constants to be scaled down by distance covered, then scaled back up
        return Tween.PercentAtTime(
            acceleration/motionPlan.distance,
            deceleration/motionPlan.distance,
            Game.S.vMax /motionPlan.distance,
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