using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputDevice : IInputDevice{

	int mTouchCount;
	Vector2 mTouchPos;

	public int GetTouchCount()
	{
		return mTouchCount;
	}

	public Vector2 GetTouchPos()
	{
		return mTouchPos;
	}

	public void Update()
	{
		if (Input.GetMouseButton (0)) {
			mTouchCount = 1;
			mTouchPos = Input.mousePosition.xy();
		} else {
			mTouchCount = 0;
		}
	}
}
