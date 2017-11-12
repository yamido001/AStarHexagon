using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PerformTimer : IDisposable {

	string mName;
	int mStartTickCount;

	public PerformTimer(string name)
	{
		mName = name;
		mStartTickCount = Environment.TickCount;
	}

	public void Dispose()
	{
		Debug.Log (mName + "  " + (Environment.TickCount - mStartTickCount));
	}
}
