using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapTileConfigBase {

	public IntVector2 tileCoord;

	public abstract MapTileConfigType tileType {
		get;
	}

	public virtual void Decode(int data)
	{
		
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
