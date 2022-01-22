using UnityEngine;
// provides unitless tweening with quadratic start, quadratic end, and constant speed in between
static class Tween {
	// time (in seconds) to complete the tween
	public static float timeRequired(float acc,float dec,float vMax) {
		float easing = 0.5f*vMax*vMax/acc + 0.5f*vMax*vMax/dec;
		if (easing > 1) {
			return (Mathf.Sqrt(2f)*(Mathf.Sqrt(acc/(dec*dec+acc*dec))+Mathf.Sqrt(dec/(acc*acc+dec*acc))));
		} else {
			float glideLength = 1f - easing;
			return vMax/acc + vMax/dec + glideLength/vMax;
		}
	}
	// how far along the path (from 0 to 1) the tween is at the given time
	// sometimes returns a NaN, which represents a completed tween (1)
	// acc, dec, and vMax are units/second, t is in seconds
	public static float PercentAtTime(float acc,float dec,float vMax,float t) {
		float easing = 0.5f*vMax*vMax/acc + 0.5f*vMax*vMax/dec;
		if (easing > 1) {
			float time = Mathf.Sqrt(2)*(Mathf.Sqrt(acc/(dec*dec+acc*dec))+Mathf.Sqrt(dec/(acc*acc+dec*acc)));
			if (t>time) return 1f;
			float midTime = time*(dec/(acc+dec));
			if (t<midTime) {
				return 0.5f*t*t*acc;
			} else {
				return 1f - (0.5f*(time-t)*(time-t)*dec);
			}
		} else {
			float glideLength = 1f - easing;
			float time = vMax/acc + vMax/dec + glideLength/vMax;
			if (t>time) return 1f;
			if (t<vMax/acc) {
				return 0.5f*t*t*acc;
			} else if (time-t<vMax/dec) {
				return 1f - (0.5f*(time-t)*(time-t)*dec);
			} else {
				return 0.5f*vMax*vMax/acc + vMax * (t - vMax/acc);
			}
		}
	}
}