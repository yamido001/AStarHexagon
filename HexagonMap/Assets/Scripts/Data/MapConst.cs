using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConst {
	public static IntVector2 MapSize = new IntVector2 (64, 64);
	public static IntVector2 MapBlockSize = new IntVector2(16, 16);		//地图上每一个子地块的Tile点的数量
	public static int MapTileSiz = 10;	//地图上每一个tile点的大小
	public static int MapHexagonRadius = 6;
	public const string SubMapViewBgPrefix = "subMapViewTerrain";
	public const int EmptyBlockId = -1;

	public static string GetSubMapViewBgName(int x, int y, bool withSuffix)
	{
		string name = SubMapViewBgPrefix + x.ToString () + "_" + y.ToString ();
		if (withSuffix)
			name += ".asset";
		return name;
	}
}

/// <summary>
/// 地图中编辑器设置的点的类型
/// </summary>
public enum MapTileConfigType
{
	Block,		//阻挡点
	Free,		//空地块，可以运行时设置的
}

public enum MapTileDynamicType
{
	City,
	Tribe,
}