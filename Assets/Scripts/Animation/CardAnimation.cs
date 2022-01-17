using UnityEngine;
using System;
public enum MotionAction {
    Drawing,
    Installing,
    Uninstalling,
    ClockReturn,
    Repacking
}
public class CardAnimation {
 	const float vMax      = 12000;
	const float accTime   =  0.2f;
	const float decTime   =  0.4f;
	const float chainTime =  0.2f;
    public Card controlledCard;
    MotionPlan motionPlan;
    Action OnCompletion;
    Bounds origin,goal;
    float startTime;
    public CardAnimation(Card controlledCard,Action onCompletion) {}
    public CardAnimation(Bounds origin,Bounds goal,MotionAction motionAction,Card controlledCard,Action onCompletion) {
        motionPlan = new MotionPlan(origin.center,goal.center,motionAction);
        this.origin = origin;
        this.goal = goal;
        this.controlledCard = controlledCard;
        this.OnCompletion = onCompletion;
        startTime = Time.time;
    }
    public void Update() {
		float percent = Tween.percentAtTime(vMax/accTime/motionPlan.distance,vMax/decTime/motionPlan.distance,vMax/motionPlan.distance,Time.time-startTime);
		if (Single.IsNaN(percent)||motionPlan.distance==0) {
			percent = 1.0f;
            OnCompletion();
		}
		SetBounds(new Bounds(MotionPlanPercent.LocationAtPercentage(motionPlan,percent),Vector2.Lerp(origin.size,goal.size,percent)));
    }
	void SetBounds(Bounds bounds) {
		controlledCard.transform.position = bounds.center;
		controlledCard.transform.localScale = Packing.GetLocalScale(bounds,controlledCard.gameObject);
	}
}