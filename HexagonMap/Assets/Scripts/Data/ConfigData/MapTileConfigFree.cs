using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileConfigFree : MapTileConfigBase {

	int mBgIndex;	//占用4bit

	public override void Decode (int data)
	{
		base.Decode (data);
		mBgIndex = ((data & 0xF0) >> 4);
	}

	public override int Encode ()
	{
		int ret = base.Encode ();
		ret |= (mBgIndex << 6);
		return ret;
	}

	public void SetBgIndex(int index)
	{
		mBgIndex = index;
	}

	public int GetBgIndex()
	{
		return mBgIndex;
	}
}
