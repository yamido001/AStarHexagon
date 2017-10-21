using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension{

	public static Vector2 xy(this Vector3 pos){
		return new Vector2 (pos.x, pos.y);
	}
}
