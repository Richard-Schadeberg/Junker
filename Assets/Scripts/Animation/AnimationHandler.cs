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
        CardAnimation animation = new CardAnimation(card,action);
        Game.S.animationHandler.animationQueue.Enqueue(animation);
        Game.S.animationHandler.PackFor(animation);
    }
    public static void Animate(Card[] cards,GameAction action) {
        // No queueing animations during reversible actions
        if (Game.S.ReversibleMode) return;
        CardAnimation animation = null;
        foreach (Card card in cards) {
            animation = new CardAnimation(card,action);
            Game.S.animationHandler.animationQueue.Enqueue(animation);
        }
        Game.S.animationHandler.PackFor(animation);
    }
    void PackFor(CardAnimation animation) {
        PackZone(animation.goalZone);
        // don't repack hand when cards leave it, so players can accurately click on cards in sequence
        if (animation.originZone!=Zone.Hand) PackZone(animation.originZone);
    }
    public void PackZone(Zone zone) {
        foreach (Card card in ZoneTracker.GetCards(zone)) {
            animationQueue.Enqueue(new CardAnimation(card,GameAction.Repacking));
        }
    }
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