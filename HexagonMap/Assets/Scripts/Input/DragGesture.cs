using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DragGesture : BaseGesture {
	enum DragState
	{
		Idle,
		TouchDown,
		Draging,
	}

	public DragGesture(IInputDevice input):base(input){}

	public Action<Vector2> beginDragHandler;
	public Action<Vector2, Vector2> dragingHandler;
	public Action<Vector2> endDragHandler;
	DragState mDragState = DragState.Idle;
	Vector2 mTouchDownPos;

	public override void Update ()
	{
		switch (mDragState) {
		case DragState.Idle:
			UpdateIdle ();
			break;
		case DragState.TouchDown:
			UpdateTouchDown ();
			break;
		case DragState.Draging:
			UpdateDraging ();
			break;
		default:
			break;
		}
	}

	void UpdateIdle()
	{
		if (mInput.GetTouchCount () == 1) {
			mTouchDownPos = mInput.GetTouchPos (0);
			mDragState = DragState.TouchDown;
		}
	}

	void UpdateTouchDown()
	{
		if (mInput.GetTouchCount () == 1) {
			Vector2 touchPos = mInput.GetTouchPos (0);
			if ((mTouchDownPos - touchPos).sqrMagnitude > MinDragDis) {
				mTouchDownPos = touchPos;
				mDragState = DragState.Draging;
				if (beginDragHandler != null)
					beginDragHandler.Invoke (mTouchDownPos);
			}
		}
	}

	void UpdateDraging()
	{
		if (mInput.GetTouchCount () == 1) {
			if (dragingHandler != null) {
				Vector2 touchPos = mInput.GetTouchPos (0);
				dragingHandler.Invoke (touchPos, touchPos - mTouchDownPos);
				mTouchDownPos = touchPos;
			}
		} else {
			mDragState = DragState.Idle;
			if (endDragHandler != null)
				endDragHandler.Invoke (mInput.GetTouchPos (0));
		}
	}
}
