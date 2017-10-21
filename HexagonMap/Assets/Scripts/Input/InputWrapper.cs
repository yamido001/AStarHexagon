using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputWrapper{

	ClickGesture mClickGesture;
	DragGesture mDragGesture;
	IInputDevice mInput = null;

	public InputWrapper(System.Action<Vector2> ClickHandler, System.Action<Vector2, Vector2> DragingHandler)
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
	}

	public void Update()
	{
		mInput.Update ();
		if (null != mClickGesture)
			mClickGesture.Update ();
		if (null != mDragGesture)
			mDragGesture.Update ();
	}
}
