using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataManager : SingleInstance<MapDataManager>{

	public static IntVector2 InValidMapCoord = new IntVector2 (-1, -1);

	int[,] mTileConfigDatas = new int[MapConst.MapSize.x,MapConst.MapSize.y];
	Dictionary<int, MapTileDynamicBase> mDynamicDataDic = new Dictionary<int, MapTileDynamicBase>();
	const string mapDataPath = "Map/Config/mapData";

	public MapDataManager()
	{
		LoadConfigData ();
	}

	public void LoadConfigData()
	{
		TextAsset mapAsset = Resources.Load<TextAsset> (mapDataPath);
		if (null != mapAsset) {
			byte[] mapBytes = mapAsset.bytes;

			for (int i = 0; i < mTileConfigDatas.GetLength(0); ++i) {
				for (int j = 0; j < mTileConfigDatas.GetLength (1); ++j) {
					mTileConfigDatas[i,j] = ReadIntFromByte (mapBytes, (i * mTileConfigDatas.GetLength (1) + j) * 4);
				}
			}
		}

		RandomDynamicData ();
	}

	public void RandomDynamicData()
	{
		mDynamicDataDic.Clear ();
		for (int i = 0; i < mTileConfigDatas.GetLength(0); ++i) {
			for (int j = 0; j < mTileConfigDatas.GetLength (1); ++j) {
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

	#if UNITY_EDITOR
	public void SeedConfigData(int x, int y, int value)
	{
		mTileConfigDatas [x, y] = value;
	}

	public void SaveConfigData()
	{
		int width = mTileConfigDatas.GetLength (0);
		int height = mTileConfigDatas.GetLength (1);
		byte[] saveBytes = new byte[width * height * 4];
		for (int i = 0; i < width; ++i) {
			for (int j = 0; j < height; ++j) {
				WriteIntToByte (mTileConfigDatas [i, j], saveBytes, (i * height + j) * 4);
			}
		}
		System.IO.File.WriteAllBytes (System.IO.Path.Combine (Application.dataPath, "Resources/" + mapDataPath + ".bytes"), saveBytes);
		UnityEditor.AssetDatabase.Refresh ();
	}

	void WriteIntToByte(int value, byte[] bytes, int index)
	{
		bytes [index] 	  = (byte)((value & 0xFF000000) >> 24);
		bytes [index + 1] = (byte)((value & 0x00FF0000) >> 16);
		bytes [index + 2] = (byte)((value & 0x0000FF00) >> 8);
		bytes [index + 3] = (byte)((value & 0x000000FF));
	}
	#endif

	int ReadIntFromByte(byte[] bytes, int index)
	{
		int ret = 0;
		if (bytes.Length > index + 3) {
			ret |= bytes [index] >> 24;
			ret |= bytes [index + 1] >> 16;
			ret |= bytes [index + 2] >> 8;
			ret |= bytes [index + 3];
		}
		return ret;
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
		ret.tileCoord = new IntVector2 (x, y);
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

	public bool IsBlock(int x, int y)
	{
		if (!IsValidTileCoord (x, y))
			return true;
		int tileData = mTileConfigDatas [x, y];
		MapTileConfigType tileType = MapTileConfigBase.DecodeTileType (tileData);
		return tileType == MapTileConfigType.Block;
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
