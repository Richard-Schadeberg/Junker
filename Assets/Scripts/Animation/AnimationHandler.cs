using UnityEngine;
using System.Collections.Generic;
using System.Linq;
// queues and fires card motion animations
// this covers translation and scaling of cards whenever the player takes an action
// a single instance of this is created by Game to store the animation queue
// this is done instead of a static queue to avoid having to reset it on each new level
public class AnimationHandler {
    public Queue<CardAnimation> animationQueue = new Queue<CardAnimation>();
    // when the time (seconds since game launched) passes timeNextFire, the next animation in the queue plays
    // using float causes bugs when the game has been running for 40 days, which is fine
    float timeNextFire=0;
    // calling Animate() will queue a card motion
    // this motion will reflect the gamestate at the time the animation was queued
    public static void Animate(Card card, GameAction action) {
        // No queueing animations during reversible actions
        if (Game.S.ReversibleMode) return;
        CardAnimation animation = new CardAnimation(card, action);
        Game.S.animationHandler.PackZone(card.zone, new Card[1]{card});
        Game.S.animationHandler.animationQueue.Enqueue(animation);
        Game.S.animationHandler.PackZone(animation.goalZone);
    }
    public static void Animate(Card[] cards, GameAction action) {
        Game.S.animationHandler.Animate_(cards, action);
    }
    public void Animate_(Card[] cards,GameAction action) {
        // Do nothing if passed an empty list
        if (cards.Length == 0) return;
        // No queueing animations during reversible actions
        if (Game.S.ReversibleMode) return;
        PackZone(cards[0].zone, cards);
        CardAnimation animation = null;
        foreach (Card card in cards) {
            animation = new CardAnimation(card,action);
            animationQueue.Enqueue(animation);
        }
        if (animation.originZone != Zone.Hand) PackZone(animation.originZone);
    }
    public void PackZone(Zone zone) {
        foreach (Card card in ZoneTracker.GetCards(zone)) {
            animationQueue.Enqueue(new CardAnimation(card, GameAction.Repacking));
        }
    }
    public void PackZone(Zone zone, Card[] excluded) {
        foreach (Card card in ZoneTracker.GetCards(zone)) {
            if (excluded.Contains(card)) continue;
            animationQueue.Enqueue(new CardAnimation(card, GameAction.Repacking));
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