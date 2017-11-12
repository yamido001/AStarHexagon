using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickGesture : BaseGesture {
	enum ClickState{
		Idle,
		TouchDown,
	}

	public ClickGesture (IInputDevice input) : base (input){}

	public System.Action<Vector2> clickHandler;
	ClickState mState = ClickState.Idle;
	Vector2 mTouchedPos;


	public override void Update ()
	{
		switch (mState) {
		case ClickState.Idle:
			UpdateIdle ();
			break;
		case ClickState.TouchDown:
			UpdateTouchDown ();
			break;
		default:
			break;
		}
	}

	void UpdateIdle()
	{
		if (mInput.GetTouchCount () == 1) {
			mState = ClickState.TouchDown;
			mTouchedPos = mInput.GetTouchPos (0);
		}
	}

	void UpdateTouchDown()
	{
		if (mInput.GetTouchCount () == 0) {
			mState = ClickState.Idle;
			Vector2 touchPos = mInput.GetTouchPos (0);
			if ((touchPos - mTouchedPos).magnitude < MinDragDis) {
				clickHandler.Invoke (touchPos);
			}
		}
	}
}
