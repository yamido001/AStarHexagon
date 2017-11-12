using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputDevice : IInputDevice{

	int mTouchCount;
	Vector2[] mTouchPos = new Vector2[2];
	float oriDis = 100f;
	float mPreviousAxis = 0f;
	bool mIsPreviousPinching = false;

	public int GetTouchCount()
	{
		return mTouchCount;
	}

	public Vector2 GetTouchPos(int index)
	{
		return mTouchPos[index];
	}

	public void Update()
	{
		if (mIsPreviousPinching) {
			float axis = Input.GetAxis ("Mouse ScrollWheel");
			if (axis == 0) {
				mIsPreviousPinching = false;
			}
			Vector2 screenCenter = new Vector2 (Screen.width / 2f, Screen.height / 2f);
			mTouchPos [0] = screenCenter - new Vector2 (oriDis - mPreviousAxis * 100, 0f);
			mTouchPos [1] = screenCenter + new Vector2 (oriDis - mPreviousAxis * 100, 0f);
		}
		else if (Input.GetMouseButton (0)) {
			mTouchCount = 1;
			mTouchPos [0] = Input.mousePosition.xy ();
		} else {
			mPreviousAxis = Input.GetAxis ("Mouse ScrollWheel");
			if (mPreviousAxis != 0f) {
				mTouchCount = 2;
				mIsPreviousPinching = true;
				Vector2 screenCenter = new Vector2 (Screen.width / 2f, Screen.height / 2f);
				mTouchPos [0] = screenCenter - new Vector2 (oriDis, 0f);
				mTouchPos [1] = screenCenter + new Vector2 (oriDis, 0f);
			} else {
				mTouchCount = 0;
			}
		}
	}
}
