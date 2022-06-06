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
        return Vector2.Lerp(motionPlan.origin,motionPlan.goal,percentage);
    }
    // a circular arc that either starts or ends horizontally
    static Vector2 LocationAtPercentageArc(MotionPlan motionPlan,float percentage) {
        Vector2 path = motionPlan.goal - motionPlan.origin;
        if (path.y == 0) return LocationAtPercentageDirect(motionPlan,percentage); // avoid division by zero
        float pathNormalSlope = -path.x/path.y;
        Vector2 halfPath = path/2;
        float circleRadiusSigned = halfPath.y + pathNormalSlope * -halfPath.x;
        Vector2 circleCentre;
        if (motionPlan.startsHorizontal) {
            circleCentre = motionPlan.origin + new Vector2(0,circleRadiusSigned);
        } else {
            circleCentre = motionPlan.goal   - new Vector2(0,circleRadiusSigned);
        }
		float angle = Vector2.SignedAngle(motionPlan.origin-circleCentre,motionPlan.goal-circleCentre);
		angle *= percentage;
		Vector2 centreToPoint = Quaternion.Euler(0, 0, angle) * (motionPlan.origin-circleCentre);
		return (circleCentre + centreToPoint);
    }
    // a combination of a semicircle and a horizontal motion, in either order
    // if startHorizontal, the semicircle motion comes first
    static Vector2 LocationAtPercentageCombination(MotionPlan motionPlan,float percentage) {
        float straightSectionDistance = Mathf.Abs(motionPlan.goal.x - motionPlan.origin.x);
        if (InStraightSection(motionPlan, percentage, straightSectionDistance)) {
            if (!motionPlan.startsHorizontal) {
                float horizontalPercentage = percentage  * motionPlan.distance/straightSectionDistance;
                return motionPlan.origin + new Vector2(horizontalPercentage        * (motionPlan.goal.x   - motionPlan.origin.x),0);
            } else {
                float horizontalPercentageFromEnd = (1-percentage) * motionPlan.distance/straightSectionDistance;
                return motionPlan.goal   + new Vector2(horizontalPercentageFromEnd * (motionPlan.origin.x - motionPlan.goal.x),0);
            }
        } else {
            Vector2 path = motionPlan.goal - motionPlan.origin;
            float arcPercent;
            float arcAngle = Mathf.PI * Mathf.Sign(path.x) * Mathf.Sign(path.y) * (!motionPlan.startsHorizontal?1:-1); // sets arcAngle to PI or -PI based on direction of rotation in arc section. Positive is CCW.
            Vector2 circleCentre = new Vector2(0,motionPlan.origin.y + path.y/2);
            Vector2 centreToPoint;
            if (motionPlan.startsHorizontal) {
                circleCentre.x = motionPlan.origin.x;
                arcPercent = percentage * motionPlan.distance/(motionPlan.distance-straightSectionDistance);
                arcAngle *= arcPercent;
                centreToPoint = motionPlan.origin - circleCentre;
            } else {
                circleCentre.x = motionPlan.goal.x;
                arcPercent = (1-percentage) * motionPlan.distance/(motionPlan.distance-straightSectionDistance);
                arcAngle *= -arcPercent;
                centreToPoint = motionPlan.goal - circleCentre;
            }
            centreToPoint = Quaternion.AngleAxis(arcAngle * 180/Mathf.PI,Vector3.forward) * centreToPoint;
            return circleCentre + centreToPoint;
        }
    }
    static bool InStraightSection(MotionPlan motionPlan,float percentage,float straightSectionDistance) {
        if (!motionPlan.startsHorizontal) {
            return straightSectionDistance >= motionPlan.distance * percentage;
        } else {
            return straightSectionDistance >= motionPlan.distance * (1 - percentage);
        }

    }
}
