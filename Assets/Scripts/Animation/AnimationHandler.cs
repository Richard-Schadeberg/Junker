using UnityEngine;
using System.Collections.Generic;
using System;
// queues and fires card motion animations
public class AnimationHandler {
    // time (in seconds) between each animation fired from the queue
    float chainTime = 0.2f;
    Queue<CardAnimation> animationQueue = new Queue<CardAnimation>();
    float timeLastFired=0;
    public void Animate(Card card,GameAction action) {
        // No queueing animations during reversible actions
        if (Game.S.ReversibleMode) return;
        CardAnimation animation = new CardAnimation(card,action);
        animationQueue.Enqueue(animation);
    }
    public void AnimateInstant(Card card,GameAction action) {
        BoundsUtil.SetBounds(card,card.bounds);
    }
    // called by Game each visual frame
    public void Update() {
        if (animationQueue.Count==0) return;
        if (Time.time - timeLastFired > chainTime) {
            timeLastFired = Time.time;
            Fire();
        }
    }
    void Fire() {
        animationQueue.Dequeue().Fire();
        // if the next animation is repacking, it should be fired immediately without waiting for the chain time
        if (animationQueue.Count!=0) {
            if (animationQueue.Peek().gameAction==GameAction.Repacking) {
                animationQueue.Dequeue().Fire();
            }
        }
    }
}
