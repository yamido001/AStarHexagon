using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PinchGesture : BaseGesture {

	public PinchGesture (IInputDevice inputDevice) : base (inputDevice){}

	enum state
	{
		idle,
		pinching,
	}

	state mState = state.idle;
	Vector2[] mPreviousPos = new Vector2[2];
	public Action<Vector2, Vector2, Vector2, Vector2> pinchHandler;

	public override void Update ()
	{
		base.Update ();
		if (mState == state.idle) {
			UpdateIdle ();
		} else {
			UpdatePitching ();
		}
	}

	void UpdateIdle()
	{
		if (mInput.GetTouchCount () == 2) {
			mState = state.pinching;
			mPreviousPos [0] = mInput.GetTouchPos (0);
			mPreviousPos [1] = mInput.GetTouchPos (1);
		}
	}

	void UpdatePitching()
	{
		if (mInput.GetTouchCount () == 2) {
			if (null != pinchHandler)
				pinchHandler.Invoke (mInput.GetTouchPos (0), mInput.GetTouchPos (0) - mPreviousPos[0], 
									 mInput.GetTouchPos (1), mInput.GetTouchPos (1) - mPreviousPos[1]);
		} else {
			mState = state.idle;
		}
	}
}
