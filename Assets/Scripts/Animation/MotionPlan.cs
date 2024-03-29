using UnityEngine;
// A motion plan contains information about a card animating between 2 places
// It does a calculation on creation, and then is used each frame to find the card's position
// It determines if the card's motion will contain an arc, and how that arc will be executed
class MotionPlan {
    public MotionPlan(Vector2 origin,Vector2 goal,GameAction gameAction) {
        this.origin = origin;
        this.goal   = goal;
        motionType       = GetMotionType(origin,goal,gameAction);
        startsHorizontal = DoesMotionStartHorizontal(gameAction);
        distance         = MotionDistance(origin,goal,motionType);
    }
    // origin/goal is centre of card
    public readonly Vector2 origin;
    public readonly Vector2 goal;
    public readonly MotionType motionType;
    public readonly bool startsHorizontal;
    public readonly float distance;
    public enum MotionType {
        // doesn't necessarily start horizontally
        Direct,
        // a circular arc that either starts or ends horizontally
        Arc,
        // a combination of a vertically sliced semicircle and a horizontal motion, in either order
        // starts AND ends horizontally, startsHorizontal is actually startsWithArc
        Combination
    }
    static MotionType GetMotionType(Vector2 origin,Vector2 goal,GameAction gameAction) {
        switch (gameAction) {
            case GameAction.Installing:
                // must end   horizontally travelling left
                return (goal.x >  origin.x ? MotionType.Combination : MotionType.Arc);
            case GameAction.Uninstalling:
                // must start horizontally travelling right
                return (goal.x >= origin.x ? MotionType.Arc : MotionType.Combination);
            case GameAction.ClockReturn:
                // must start horizontally travelling left
                return (goal.x >  origin.x ? MotionType.Combination : MotionType.Arc);
            default:
                return MotionType.Direct;
        }
    }
    // true for starting horizontal, false for ending horizontal, undefined (false) for neither
    static bool DoesMotionStartHorizontal(GameAction gameAction) {
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
    static float MotionDistance(Vector2 origin,Vector2 goal,MotionType motionType) {
        switch (motionType) {
            case MotionType.Direct:
                return Vector2.Distance(origin,goal);
            case MotionType.Arc:
                return ArcDistance(origin,goal);
            case MotionType.Combination:
                return CombinationDistance(origin,goal);
            default:
                return 0;
        }
    }
    // which end starts horizontal is irrelevant for length calculation, so start is assumed to be horizontal
    static float ArcDistance(Vector2 origin,Vector2 goal) {
        Vector2 path = goal - origin;
        // avoid division by zero
        if (path.y == 0) return path.x;
        float pathNormalSlope = -path.x/path.y;
        Vector2 midPoint = path/2;
        float circleRadius = midPoint.y + pathNormalSlope * -midPoint.x;
        float arcAngle = 2*Mathf.Asin(path.magnitude/(2*circleRadius));
        return circleRadius*arcAngle;
    }
    // which end starts horizontal is irrelevant for length calculation, so start is assumed to be horizontal
    static float CombinationDistance(Vector2 origin,Vector2 goal) {
        Vector2 path = goal - origin;
        float arcDistance = Mathf.PI * Mathf.Abs(path.y) / 2;
        float straightDistance = Mathf.Abs(path.x);
        return arcDistance + straightDistance;
    }
}