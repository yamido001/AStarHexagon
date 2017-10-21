using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleInstance<T> where T : class, new() {

	private static T mInstance;

	public static T Instance
	{
		get {
			if (null == mInstance) {
				mInstance = new T();
			}
			return mInstance;
		}
	}

	public static void ClearInstance()
	{
		mInstance = null;
	}
}
