using UnityEngine;
using System.Collections.Generic;
using System;
// queues and fires card motion animations
public class AnimationHandler {
    public Queue<CardAnimation> animationQueue = new Queue<CardAnimation>();
    float timeNextFire=0;
    // calling Animate() will queue a card motion
    // this motion will reflect the gamestate at the time the animation was queued
    public static void Animate(Card card,GameAction action) {
        // No queueing animations during reversible actions
        if (Game.S.ReversibleMode) return;
        PackFor(CardAnimation.GoalZone(action,card),CardAnimation.OriginZone(action,card));
        Game.S.animationHandler.animationQueue.Enqueue(new CardAnimation(card,action));
    }
    public static void PackFor(Zone goal,Zone origin) {
        PackZone(goal);
        // don't repack hand when cards leave it, so players can accurately click on cards in sequence
        if (origin!=Zone.Hand) PackZone(origin);
    }
    public static void PackCards(IEnumerable<Card> cards) {foreach (Card card in cards) {Animate(card,GameAction.Repacking);}}
    public static void PackZone(Zone zone) {PackCards(ZoneTracker.GetCards(zone));}
    public void AnimateInstant(Card card,GameAction action) {
        BoundsUtil.SetBounds(card,card.bounds);
    }
    public void WaitSeconds(float wait) {timeNextFire = Time.time + wait;}
    // called by Game each visual frame
    public void Update() {
        if (animationQueue.Count==0) return;
        if (Time.time >= timeNextFire) {
            timeNextFire = Time.time + Game.S.chainTime;
            Fire();
        }
    }
    void Fire() {
        animationQueue.Dequeue().Fire();
        // if the next animation is repacking, it should be fired immediately without waiting for the chain time
        if (animationQueue.Count!=0) {
            if (animationQueue.Peek().gameAction==GameAction.Repacking) {
                Fire();
            }
        }
    }
}
