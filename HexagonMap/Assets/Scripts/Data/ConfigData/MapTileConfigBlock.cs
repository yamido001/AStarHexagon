using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileConfigBlock : MapTileConfigBase {

	protected int mModelKey = MapPrefabDefine.emptyBlockModelKey;	//占用4bit

	public override MapTileConfigType tileType {
		get {
			return MapTileConfigType.Block;
		}
	}

	public override void Decode (int data)
	{
		base.Decode (data);
		mModelKey = ((data & 0xF0) >> 4);
	}

	public override int Encode ()
	{
		int ret = base.Encode ();
		ret |= ((mModelKey << 4) & 0xF0);
		return ret;
	}

	#if UNITY_EDITOR
	public void SetModelkey(int key)
	{
		mModelKey = key;
	}
	#endif

	public int GetModelKey()
	{
		return mModelKey;
	}
}
