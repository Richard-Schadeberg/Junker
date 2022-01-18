using UnityEngine;
static class MotionPlanPercent {
    public static Vector2 PositionAtPercentage(MotionPlan motionPlan,float percentage) {
        switch (motionPlan.motionType) {
            case MotionPlan.MotionType.Direct:
                return LocationAtPercentageDirect(motionPlan,percentage);
            case MotionPlan.MotionType.Arc:
                return LocationAtPercentageArc(motionPlan,percentage);
            case MotionPlan.MotionType.Combination:
                return LocationAtPercentageCombination(motionPlan,percentage);
            default:
                return new Vector2(0,0);
        }
    }
    static Vector2 LocationAtPercentageDirect(MotionPlan motionPlan,float percentage) {
        return motionPlan.origin + (motionPlan.goal - motionPlan.origin) * percentage;
    }
    static Vector2 LocationAtPercentageArc(MotionPlan motionPlan,float percentage) {
        Vector2 path = motionPlan.goal - motionPlan.origin;
        if (path.y == 0) return motionPlan.origin + path * percentage; // avoid division by zero
        float pathNormalSlope = -path.x/path.y;
        Vector2 halfPath = path/2;
        float circleRadiusSigned = halfPath.y + pathNormalSlope * -halfPath.x;
        Vector2 circleCentre;
        if (motionPlan.startHorizontal) {
            circleCentre = motionPlan.origin + new Vector2(0,circleRadiusSigned);
        } else {
            circleCentre = motionPlan.goal   - new Vector2(0,circleRadiusSigned);
        }
		float angle = Vector2.SignedAngle(motionPlan.origin-circleCentre,motionPlan.goal-circleCentre);
		angle *= percentage;
		Vector2 centreToPoint = Quaternion.Euler(0, 0, angle) * (motionPlan.origin-circleCentre);
		return (circleCentre + centreToPoint);
    }
    static Vector2 LocationAtPercentageCombination(MotionPlan motionPlan,float percentage) {
        bool inStraightSection;
        float straightDistance = Mathf.Abs(motionPlan.goal.x - motionPlan.origin.x);
        if (motionPlan.startHorizontal) {
            inStraightSection = straightDistance >= motionPlan.distance * percentage;
        } else {
            inStraightSection = straightDistance >= motionPlan.distance * (1 - percentage);
        }
        if (inStraightSection) {
            if (motionPlan.startHorizontal) {
                return motionPlan.origin + new Vector2(   percentage  * motionPlan.distance/straightDistance * (motionPlan.goal.x   - motionPlan.origin.x),0);
            } else {
                return motionPlan.goal   + new Vector2((1-percentage) * motionPlan.distance/straightDistance * (motionPlan.origin.x - motionPlan.goal.x)  ,0);
            }
        } else {
            Vector2 path = motionPlan.goal - motionPlan.origin;
            float arcPercent;
            float arcAngle = Mathf.PI * Mathf.Sign(path.x) * Mathf.Sign(path.y) * (motionPlan.startHorizontal?1:-1); // sets arcAngle to PI or -PI based on direction of rotation in arc section. Positive is CCW.
            Vector2 circleCentre = new Vector2(0,motionPlan.origin.y + path.y/2);
            Vector2 centreToPoint;
            if (!motionPlan.startHorizontal) {
                circleCentre.x = motionPlan.origin.x;
                arcPercent = percentage * motionPlan.distance/(motionPlan.distance-straightDistance);
                arcAngle *= arcPercent;
                centreToPoint = motionPlan.origin - circleCentre;
            } else {
                circleCentre.x = motionPlan.goal.x;
                arcPercent = (1-percentage) * motionPlan.distance/(motionPlan.distance-straightDistance);
                arcAngle *= -arcPercent;
                centreToPoint = motionPlan.goal - circleCentre;
            }
            centreToPoint = Quaternion.AngleAxis(arcAngle * 180/Mathf.PI,Vector3.forward) * centreToPoint;
            return circleCentre + centreToPoint;
        }
    }
}
