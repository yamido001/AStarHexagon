using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension{

	public static void Reset(this Transform tf)
	{
		tf.localScale = Vector3.one;
		tf.localEulerAngles = Vector3.zero;
		tf.localPosition = Vector3.zero;
	}

	public static void ResetAndToLocalPos(this Transform tf, Vector3 pos)
	{
		tf.localScale = Vector3.one;
		tf.localEulerAngles = Vector3.zero;
		tf.localPosition = pos;
	}

	public static void ResetAndToWorldPos(this Transform tf, Vector3 pos)
	{
		tf.localScale = Vector3.one;
		tf.localEulerAngles = Vector3.zero;
		tf.position = pos;
	}
}
