using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataManager : SingleInstance<MapDataManager>{

	int[,] mTileConfigDatas = new int[MapConst.MapSize.x,MapConst.MapSize.y];
	Dictionary<int, MapTileDynamicBase> mDynamicDataDic = new Dictionary<int, MapTileDynamicBase>();

	public MapDataManager()
	{
		LoadConfigData ();
	}

	public void LoadConfigData()
	{
		for (int i = 0; i < mTileConfigDatas.Rank; ++i) {
			for (int j = 0; j < mTileConfigDatas.GetLength (i); ++j) {

				int value = Random.Range (0, 1);
				if (value == 0) {
					MapTileConfigBlock block = new MapTileConfigBlock ();
					block.tileType = MapTileConfigType.Block;
					block.SetBgIndex(Random.Range(0, 2));
					mTileConfigDatas [i, j] = block.Encode ();
				} else {
					MapTileConfigFree freeTile = new MapTileConfigFree ();
					freeTile.tileType = MapTileConfigType.Free;
					freeTile.SetBgIndex(Random.Range(0, 2));
					mTileConfigDatas [i, j] = freeTile.Encode ();
				}
			}
		}
		RandomDynamicData ();
	}

	public void RandomDynamicData()
	{
		mDynamicDataDic.Clear ();
		for (int i = 0; i < mTileConfigDatas.Rank; ++i) {
			for (int j = 0; j < mTileConfigDatas.GetLength (i); ++j) {
				MapTileConfigType tileType = MapTileConfigBase.DecodeTileType (mTileConfigDatas [i, j]);
				if (tileType == MapTileConfigType.Free) {
					int value = Random.Range (0, 1);
					MapTileDynamicBase dynamicData = null;
					if (value == 0) {
						dynamicData = new MapTileDynamicCity ();
					} else {
						dynamicData = new MapTileDynamicTribe ();
					}
					mDynamicDataDic[TileCoordToTileKey(i, j)] = dynamicData;
				}
			}
		}
	}

	public MapTileConfigBase GetTileConfig(int x, int y)
	{
		if (!IsValidTileCoord (x, y))
			return null;
		int tileData = mTileConfigDatas [x, y];
		MapTileConfigType tileType = MapTileConfigBase.DecodeTileType (tileData);
		MapTileConfigBase ret = null;
		switch (tileType) {
		case MapTileConfigType.Block:
			ret = new MapTileConfigBlock ();
			break;
		case MapTileConfigType.Free:
			ret = new MapTileConfigFree ();
			break;
		default:
			break;
		}
		ret.Decode (tileData);
		return ret;
	}

	public MapTileDynamicBase GetTileDynamicData(int x, int y)
	{
		int tileKey = TileCoordToTileKey (x, y);
		MapTileDynamicBase ret = null;
		mDynamicDataDic.TryGetValue (tileKey, out ret);
		return ret;
	}

	public bool IsValidTileCoord(int x, int y)
	{
		if (x < 0 || x >= mTileConfigDatas.GetLength(0))
			return false;
		if (y < 0 || y >= mTileConfigDatas.GetLength (1))
			return false;
		return true;
	}

	#region 子地块逻辑
	/// <summary>
	/// 获取地图上点所在的block的Id
	/// </summary>
	/// <param name="tileCoord">Tile coordinate.</param>
	public static int TileCoordToBlockId(IntVector2 tileCoord)
	{
		return TileCoordToBlockId (tileCoord.x, tileCoord.y);
	}
		
	public static int TileCoordToBlockId(int x, int y)
	{
		int blockX = x / MapConst.MapBlockSize.x;
		int blockY = y / MapConst.MapBlockSize.y;
		return (blockX << 16) | blockY;
	}

	public static int BlockCoordToBlockId(int x, int y)
	{
		return (x << 16) | y;
	}

	public static IntVector2 TileCoordToBlockCoord(IntVector2 tileCoord)
	{
		return new IntVector2 (tileCoord.x / MapConst.MapBlockSize.x, tileCoord.y / MapConst.MapBlockSize.y);
	}

	public static IntVector2 BlockIdToBlockZeroTileCoord(int blockId)
	{
		return BlockIdToBlockCoord(blockId) * MapConst.MapBlockSize;
	}

	public static IntVector2 BlockIdToBlockCoord(int blockId)
	{
		IntVector2 blockCoord;
		blockCoord.x = blockId >> 16;
		blockCoord.y = blockId & 0xFFFF;
		return blockCoord;
	}

	public static int TileCoordToTileKey(int coordX, int coordY)
	{
		return (coordX << 16) | coordY;
	}

	public static IntVector2 TileKeyToTileCoord(int tileKey)
	{
		IntVector2 ret;
		ret.x = tileKey >> 16;
		ret.y = tileKey & 0xFFFF;
		return ret;
	}
	#endregion
}
