using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	public static Vector2 LinePoint(Vector2 origin,Vector2 destination,float percentage) {
		return origin + (destination-origin)*percentage;
	}
	public static Vector2 ArcPoint(Vector2 origin,Vector2 destination,float percentage) {
		Vector2 route = destination-origin;
		Vector2 midPoint = (origin+destination)/2;
		Vector2 circleCentre = destination;
		circleCentre.y = midPoint.y;
		circleCentre.y -= (destination.x-midPoint.x)*route.x/route.y;
		float angle = Vector2.Angle(origin-circleCentre,destination-circleCentre);
		angle *= percentage;
		Vector2 centreToPoint = Quaternion.Euler(0, 0, angle) * (origin-circleCentre);
		return (circleCentre + centreToPoint);
	}
	
	public static float HookLength(Vector2 origin,Vector2 destination) {
		float leg1 = destination.x-origin.x;
		float leg2 = (float)Math.PI*(destination.y-origin.y)/2f;
		return leg1+leg2;
	}
	public static Vector2 HookPoint(Vector2 origin,Vector2 destination,float percentage) {
		float totalDist = HookLength(origin,destination);
		float doneDist = totalDist*percentage;
		if (doneDist<=destination.x-origin.x) {
			return origin + new Vector2(doneDist,0);
		} else {
			Vector2 midPoint = new Vector2(destination.x,(origin.y+destination.y)/2);
			float circlePercent = (doneDist-(destination.x-origin.x))/((float)Math.PI*(destination.y-origin.y)/2f);
			Vector2 midToPoint = midPoint - destination;
			midToPoint = Quaternion.Euler(0, 0, 180*circlePercent) * midToPoint;
			return midPoint + midToPoint;
		}
	}
	
	public static float ReverseArcLength(Vector2 origin,Vector2 destination) {
		Vector2 route = destination-origin;
		Vector2 midPoint = (origin+destination)/2;
		Vector2 circleCentre = origin;
		circleCentre.y = midPoint.y;
		circleCentre.y -= (midPoint.x-destination.x)*route.x/route.y;
		float angle = Vector2.Angle(origin-circleCentre,destination-circleCentre);
		return 2f*(float)Math.PI*(origin.y-circleCentre.y)*angle/360f;
	}
	public static Vector2 ReverseArcPoint(Vector2 origin,Vector2 destination,float percentage) {
		Vector2 route = destination-origin;
		Vector2 midPoint = (origin+destination)/2;
		Vector2 circleCentre = origin;
		circleCentre.y = midPoint.y;
		circleCentre.y -= (midPoint.x-destination.x)*route.x/route.y;
		float angle = Vector2.Angle(origin-circleCentre,destination-circleCentre);
		angle *= percentage;
		Vector2 centreToPoint = Quaternion.Euler(0, 0, angle) * (origin-circleCentre);
		return (circleCentre + centreToPoint);
	}
	
	public static float ReverseHookLength(Vector2 origin,Vector2 destination) {
		float leg1 = destination.x-origin.x;
		float leg2 = (float)Math.PI*(origin.y-destination.y)/2f;
		return leg1+leg2;
	}
	public static Vector2 ReverseHookPoint(Vector2 origin,Vector2 destination,float percentage) {
		float totalDist = ReverseHookLength(origin,destination);
		float doneDist = totalDist*percentage;
		float remainDist = totalDist-doneDist;
		if (remainDist<=destination.x-origin.x) {
			return destination - new Vector2(remainDist,0);
		} else {
			Vector2 midPoint = new Vector2(origin.x,(origin.y+destination.y)/2);
			float circlePercent = doneDist/((float)Math.PI*(origin.y-destination.y)/2f);
			Vector2 midToPoint = origin - midPoint;
			midToPoint = Quaternion.Euler(0, 0, 180*circlePercent) * midToPoint;
			return midPoint + midToPoint;
		}
	}
}