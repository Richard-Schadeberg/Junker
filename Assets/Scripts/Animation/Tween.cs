using System;
public static class Tween {
	public static float timeRequired(float acc,float dec,float vMax) {
		float easing = 0.5f*vMax*vMax/acc + 0.5f*vMax*vMax/dec;
		if (easing > 1) {
			return (float)(Math.Sqrt(2)*(Math.Sqrt(acc/(dec*dec+acc*dec))+Math.Sqrt(dec/(acc*acc+dec*acc))));
		} else {
			float glideLength = 1f - easing;
			return vMax/acc + vMax/dec + glideLength/vMax;
		}
	}
	public static float percentAtTime(float acc,float dec,float vMax,float t) {
		float easing = 0.5f*vMax*vMax/acc + 0.5f*vMax*vMax/dec;
		if (easing > 1) {
			float time = (float)(Math.Sqrt(2)*(Math.Sqrt(acc/(dec*dec+acc*dec))+Math.Sqrt(dec/(acc*acc+dec*acc))));
			if (t>time) return Single.NaN;
			float midTime = time*(dec/(acc+dec));
			if (t<midTime) {
				return 0.5f*t*t*acc;
			} else {
				return 1f - (0.5f*(time-t)*(time-t)*dec);
			}
		} else {
			float glideLength = 1f - easing;
			float time = vMax/acc + vMax/dec + glideLength/vMax;
			if (t>time) return Single.NaN;
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