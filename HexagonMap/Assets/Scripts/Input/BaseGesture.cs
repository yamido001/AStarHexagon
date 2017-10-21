using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGesture{

	protected IInputDevice mInput;
	protected const float MinDragDis = 15;

	public BaseGesture(IInputDevice inputWrapper)
	{
		mInput = inputWrapper;
	}

	public virtual void Update ()
	{
		
	}
}
