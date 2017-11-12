using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputDevice
{
	int GetTouchCount();
	Vector2 GetTouchPos(int index);
	void Update();
}
