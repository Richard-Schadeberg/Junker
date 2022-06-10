using UnityEngine;
using System.Collections.Generic;
using System.Linq;
// queues and fires card motion animations
// this covers translation and scaling of cards whenever the player takes an action
// a single instance of this is created by Game to store the animation queue
// this is done instead of a static queue to avoid having to reset it on each new level
// changes to the game's state are made instantly, so animations don't always represent the gamestate
public class AnimationHandler {
    // card animations that are waiting to be performed
    public Queue<CardAnimation> animationQueue = new Queue<CardAnimation>();
    // when the time (seconds since game launched) passes timeNextFire, the next animation in the queue plays
    // using float causes bugs when the game has been running for 40 days, which is fine
    float timeNextFire=0;
    // calling Animate() will queue a card motion
    // this motion will reflect the gamestate at the time the animation was queued
    // if a group of cards are moved in a single action (drawing/clockreturning), they are passed as a list
    // this is to cut down on redundant windup/winddown animation
    // static version to make call shorter
    public static void Animate(Card[] cards, GameAction action) {
        Game.S.animationHandler.Animate_(cards, action);
    }
    private void Animate_(Card[] cards,GameAction action) {
        // No queueing animations during reversible actions
        if (Game.S.ReversibleMode) return;
        // Do nothing if passed an empty list
        if (cards.Length == 0) return;
        // first repack the zone the cards are moving into, to make space
        RePackZone(cards[0].zone, cards);
        // last animation object in loop is referenced after loop
        CardAnimation animation = null;
        // second, move the cards
        foreach (Card card in cards) {
            animation = new CardAnimation(card,action);
            animationQueue.Enqueue(animation);
        }
        // finally repack the zone the cards came from, now that there's more space
        // don't repack the hand when cards are leaving it, because people don't want
        // cards in hand moving when they're about to click on them
        if (animation.originZone != Zone.Hand) RePackZone(animation.originZone);
    }
    // animate a single card
    public static void Animate(Card card, GameAction action) {
        Animate(new Card[1] { card }, action);
    }
    // rearrange a zone for when cards have exited it or are about to enter it
    // if cards are animating into a zone, ZoneTracker thinks they are already there
    // so they need to be excluded from repacking
    public void RePackZone(Zone zone, Card[] excluded) {
        foreach (Card card in ZoneTracker.GetCards(zone)) {
            if (excluded.Contains(card)) continue;
            animationQueue.Enqueue(new CardAnimation(card, GameAction.Repacking));
        }
    }
    // no excluded cards
    public void RePackZone(Zone zone) {
        RePackZone(zone, new Card[0]);
    }
    // move a card by instantly teleporting it, without using the queue or smooth motion
    public void AnimateInstant(Card card,GameAction action) {
        BoundsUtil.MoveAndScaleCardToBounds(card,card.goalBoundsForCurrentGamestate);
    }
    public void PauseAnimations(float waitSeconds) {timeNextFire = Time.time + waitSeconds;}
    // called by Game each visual frame
    public void TryFireQueuedAnimation() {
        if (animationQueue.Count==0) return;
        if (Time.time >= timeNextFire) {
            timeNextFire = Time.time + Game.S.chainTime;
            FireNextAnimation();
        }
    }
    void FireNextAnimation() {
        if (animationQueue.Count == 0) return;
        animationQueue.Dequeue().Fire();
        // if the next animation is repacking or installing, it should be fired immediately without waiting for the chain time
        // this is because all repacking actions in a row can be done simultaneously
        // installing is also fired instantly to make the game feel more responsive
        if (animationQueue.Count != 0) {
            if (animationQueue.Peek().gameAction == GameAction.Repacking || animationQueue.Peek().gameAction == GameAction.Installing) {
                FireNextAnimation();
            }
        }
    }
}