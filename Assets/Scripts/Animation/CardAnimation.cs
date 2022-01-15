using UnityEngine;
public enum MotionAction {
    Drawing,
    Installing,
    Uninstalling,
    ClockReturn,
    Repacking
}
public static class CardAnimation {
 	const float vMax      = 12000;
	const float accTime   =  0.2f;
	const float decTime   =  0.4f;
	const float chainTime =  0.2f;
}