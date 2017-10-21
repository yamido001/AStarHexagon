using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension{

	public static void Reset(this Transform tf)
	{
		tf.localScale = Vector3.one;
		tf.localPosition = Vector3.zero;
		tf.localEulerAngles = Vector3.zero;
	}
}
