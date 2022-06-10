using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// used to find the best size and location for a number of cards within a zone
public static class Packing {
	public static Bounds[] PackBounds(Vector2 size,Bounds bounds,int number,Zone zone) {return PackBounds(size, bounds, number, false, zone == Zone.Junk);}
	// a lot of this was written through trial-and-error
	public static Bounds[] PackBounds(Vector2 size,Bounds bounds,int number,bool snaking,bool rightToLeft) {
		var solutions = new Bounds[number];
		float touchTopSize =1f;
		float touchSideSize=0f;
		float oldTouchSideSize=0f;
		int rows=0;
		while (touchSideSize<touchTopSize) {
			oldTouchSideSize = touchSideSize;
			rows++;
			touchSideSize  = bounds.size.x/size.x/(float)Math.Ceiling((float)number/rows);
			touchTopSize  = bounds.size.y/size.y/rows;
		}
		Vector2 newSize;
		int columns;
		if (size.y*touchSideSize*rows>bounds.size.y) {
			if (touchTopSize>oldTouchSideSize) {
				newSize = size*touchTopSize;
				columns = (int)Math.Floor(bounds.size.x/newSize.x);
			} else {
				newSize = size*oldTouchSideSize;
				rows--;
				columns = (int)Math.Ceiling((float)number/rows);
			}
		} else {
			newSize = size*touchSideSize;
			columns = (int)Math.Ceiling((float)number/rows);
		}
		int row=0;
		int column=0;
		for(int i=0;i<number;i++) {
			Vector2 min = bounds.min;
			min.x += column * newSize.x;
			min.y += row * newSize.y;
			if (snaking && row % 2 == 1) {
				min.x = bounds.max.x-(min.x+newSize.x/2-bounds.min.x)-newSize.x/2-bounds.size.x+columns*newSize.x;
			}
			if (rightToLeft) {
				min.x += bounds.size.x - newSize.x*(float)Math.Ceiling((float)number/rows);
			}
			solutions[i] = new Bounds(min + (newSize/2),newSize);
			column++;
			if (column == columns) {
				column=0;
				row++;
			}
		}
		return solutions;
	}
}
