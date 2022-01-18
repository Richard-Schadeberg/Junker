using UnityEngine;
using System.Collections.Generic;

// queues and fires card motion animations
public static class AnimationHandler
{
    public static Queue<CardAnimation> animationQueue = new Queue<CardAnimation>();
    public static void Animate(Card card,GameAction action) {
        if (Game.S.ReversibleMode) return;
        CardAnimation animation = new CardAnimation(card,action);
        animationQueue.Enqueue(animation);
    }
    // called by Game each visual frame
    public static void Update() {

    }
}
