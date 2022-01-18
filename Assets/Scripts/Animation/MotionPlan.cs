using UnityEngine;
// A motion plan contains information about a card animating between 2 places
// It does a calculation on creation, and then is used each frame to find the card's position
class MotionPlan {
    public MotionPlan(Vector2 origin,Vector2 goal,GameAction gameAction) {
        this.origin = origin;
        this.goal = goal;
        motionType = GetMotionType(origin,goal,gameAction);
        startHorizontal = MotionStartsHorizontal(gameAction);
        distance = MotionDistance(origin,goal,motionType);
    }
    public readonly Vector2 origin;
    public readonly Vector2 goal;
    public readonly MotionType motionType;
    public readonly bool startHorizontal;
    public readonly float distance;
    public enum MotionType {
        Direct,
        Arc,
        Combination
    }
    static bool MotionStartsHorizontal(GameAction gameAction) {
        switch (gameAction) {
            case GameAction.Installing:
                return false;
            case GameAction.Uninstalling:
                return true;
            case GameAction.ClockReturn:
                return true;
            default:
                return false;
        }
    }
    static MotionType GetMotionType(Vector2 origin,Vector2 goal,GameAction gameAction) {
        switch (gameAction) {
            case GameAction.Drawing:
                return MotionType.Direct;
            case GameAction.Installing:
                return (goal.x >  origin.x ? MotionType.Combination : MotionType.Arc);
            case GameAction.Uninstalling:
                return (goal.x >= origin.x ? MotionType.Arc : MotionType.Combination);
            case GameAction.ClockReturn:
                return (goal.x >  origin.x ? MotionType.Combination : MotionType.Arc);
            case GameAction.Repacking:
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