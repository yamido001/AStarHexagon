using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapTileConfigBase {

	public MapTileConfigType tileType;	//占用4bit


	public virtual void Decode(int data)
	{
		tileType = DecodeTileType (data);
	}

	public virtual int Encode()
	{
		int ret = 0;
		ret |= ((int)tileType << 0);
		return ret;
	}

	public static MapTileConfigType DecodeTileType(int data)
	{
		return (MapTileConfigType)((data & 0xf) >> 0);
	}
}
