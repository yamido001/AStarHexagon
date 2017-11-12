using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputWrapper{

	ClickGesture mClickGesture;
	DragGesture mDragGesture;
	PinchGesture mPinchGesture;
	IInputDevice mInput = null;

	public InputWrapper(System.Action<Vector2> ClickHandler, System.Action<Vector2, Vector2> DragingHandler
						, System.Action<Vector2, Vector2, Vector2, Vector2> PinchingHandler)
	{
		mInput = new MouseInputDevice ();
		if (null != ClickHandler) {
			mClickGesture = new ClickGesture (mInput);
			mClickGesture.clickHandler = ClickHandler;
		}
		if (null != DragingHandler) {
			mDragGesture = new DragGesture (mInput);
			mDragGesture.dragingHandler = DragingHandler;
		}
		if (null != PinchingHandler) {
			mPinchGesture = new PinchGesture (mInput);
			mPinchGesture.pinchHandler = PinchingHandler;
		}
	}

	public void Update()
	{
		mInput.Update ();
		if (null != mClickGesture)
			mClickGesture.Update ();
		if (null != mDragGesture)
			mDragGesture.Update ();
		if (null != mPinchGesture)
			mPinchGesture.Update ();
	}
}
