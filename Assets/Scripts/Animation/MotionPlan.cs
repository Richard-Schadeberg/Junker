using UnityEngine;
class MotionPlan {
    public MotionPlan(Vector2 origin,Vector2 goal,MotionAction motionAction) {
        this.origin = origin;
        this.goal = goal;
        motionType = GetMotionType(origin,goal,motionAction);
        startHorizontal = MotionStartsHorizontal(motionAction);
        distance = MotionDistance(origin,goal,motionType);
    }
    public readonly MotionType motionType;
    public readonly bool startHorizontal;
    public readonly Vector2 origin;
    public readonly Vector2 goal;
    public readonly float distance;
    public enum MotionType {
        Direct,
        Arc,
        Combination
    }
    static bool MotionStartsHorizontal(MotionAction motionAction) {
        switch (motionAction) {
            case MotionAction.Installing:
                return false;
            case MotionAction.Uninstalling:
                return true;
            case MotionAction.ClockReturn:
                return true;
            default:
                return false;
        }
    }
    static MotionType GetMotionType(Vector2 origin,Vector2 goal,MotionAction motionAction) {
        switch (motionAction) {
            case MotionAction.Drawing:
                return MotionType.Direct;
            case MotionAction.Installing:
                return (goal.x >  origin.x ? MotionType.Combination : MotionType.Arc);
            case MotionAction.Uninstalling:
                return (goal.x >= origin.x ? MotionType.Arc : MotionType.Combination);
            case MotionAction.ClockReturn:
                return (goal.x >  origin.x ? MotionType.Combination : MotionType.Arc);
            case MotionAction.Repacking:
                return MotionType.Direct;
            default:
                return MotionType.Direct;
        }
    }
    static float MotionDistance(Vector2 origin,Vector2 goal,MotionType motionType) {
        switch (motionType) {
            case MotionType.Direct:
                return Vector2.Distance(origin,goal);
            case MotionType.Arc:
                return ArcLength(origin,goal);
            case MotionType.Combination:
                return CombinedLength(origin,goal);
            default:
                return 0;
        }
    }
    static float ArcLength(Vector2 origin,Vector2 goal) { // which end starts horizontal is irrelevant for length calculation, so start is assumed to be horizontal
        Vector2 path = goal - origin;
        if (path.y == 0) return path.x; // avoid division by zero
        float pathNormalSlope = -path.x/path.y;
        Vector2 midPoint = path/2;
        float circleRadius = midPoint.y + pathNormalSlope * -midPoint.x;
        float arcAngle = 2*Mathf.Asin(path.magnitude/(2*circleRadius));
        return circleRadius*arcAngle;
    }
    static float CombinedLength(Vector2 origin,Vector2 goal) { // which end starts horizontal is irrelevant for length calculation, so start is assumed to be horizontal
        Vector2 path = goal - origin;
        float arcDistance = Mathf.PI * Mathf.Abs(path.y) / 2;
        float straightDistance = Mathf.Abs(path.x);
        return arcDistance + straightDistance;
    }
}