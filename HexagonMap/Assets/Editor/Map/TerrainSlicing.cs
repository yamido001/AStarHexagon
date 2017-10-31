using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class TerrainSlicing : Editor
{
	public static string TerrainSavePath = "Assets/Resources/Map/Terrain/";
	public static string TerrainPrefabSavePath = "Assets/Resources/Map/Prefab/Terrain/";

	//开始分割地形
	[MenuItem("编辑器/地形切割")]
	private static void Slicing()
	{
		Terrain terrain = GameObject.FindObjectOfType<Terrain>();
		if (terrain == null)
		{
			Debug.LogError("找不到地形!");
			return;
		}

		TerrainData terrainData = terrain.terrainData;
		Vector3 oldTerrainSize = terrainData.size;
		Vector2 mapMinSize = MapLayout.Instance.GetMapSize ();
		Vector2 mapBlockSize = MapLayout.Instance.GetBlockSize ();
		IntVector2 mapMinIntSize = Vector2ToIntVector2(mapMinSize);
		IntVector2 oldTerrainIntSize = Vector2ToIntVector2 (new Vector2 (oldTerrainSize.x, oldTerrainSize.z));

		if (mapMinIntSize.x != oldTerrainIntSize.x || mapMinIntSize.y != oldTerrainIntSize.y) {
			Debug.LogError (string.Format("地图大小不符合,不附带边缘部分，应该为({0:D},{1:D})", (int)mapMinIntSize.x, (int)mapMinIntSize.y));
			return;
		}

		if (Directory.Exists(TerrainSavePath)) 
			Directory.Delete(TerrainSavePath, true);
		Directory.CreateDirectory(TerrainSavePath);

		if (Directory.Exists(TerrainPrefabSavePath)) 
			Directory.Delete(TerrainPrefabSavePath, true);
		Directory.CreateDirectory(TerrainPrefabSavePath);

		

		//这里我分割的宽和长度是一样的.这里求出循环次数,TerrainLoad.SIZE要生成的地形宽度,长度相同
		//高度地图的分辨率只能是2的N次幂加1,所以SLICING_SIZE必须为2的N次幂
		int slicingCountX = MapConst.MapSize.x / MapConst.MapBlockSize.x;
		int slicingCountY = MapConst.MapSize.y / MapConst.MapBlockSize.y;
		int slicingMinValue = Math.Min (slicingCountX, slicingCountY);

		//得到新地图分辨率
		int newHeightmapResolution = (terrainData.heightmapResolution - 1) / slicingMinValue;
		int newAlphamapResolution = terrainData.alphamapResolution / slicingMinValue;
		int newbaseMapResolution = terrainData.baseMapResolution / slicingMinValue;
		SplatPrototype[] splatProtos = terrainData.splatPrototypes;

		//循环宽和长,生成小块地形
		for (int x = 0; x < slicingCountX; ++x)
		{
			for (int y = 0; y < slicingCountY; ++y)
			{
				//创建资源
				TerrainData newData = new TerrainData();
				string terrainPath = TerrainSavePath + MapConst.GetSubMapViewBgName(x, y, true);
				AssetDatabase.CreateAsset(newData, terrainPath);
				EditorUtility.DisplayProgressBar("正在分割地形", terrainPath, (float)(x * slicingCountX + y) / (float)(slicingCountX * slicingCountY));

				//设置分辨率参数
				newData.heightmapResolution = newHeightmapResolution;
				newData.alphamapResolution = newAlphamapResolution;
				newData.baseMapResolution = newbaseMapResolution;

				//设置大小
				//地块都是相同的，只不过横向和纵向的最后一个需要补齐边缘，所以多出一部分距离
				float xSize = mapBlockSize.x;
				float ySize = mapBlockSize.y;
				if (x == slicingCountX - 1) {
					xSize = oldTerrainSize.x - (slicingCountX - 1) * mapBlockSize.x;
				}
				if (y == slicingCountY - 1) {
					ySize = oldTerrainSize.z - (slicingCountY - 1) * mapBlockSize.y;
				}
				newData.size = new Vector3(xSize, oldTerrainSize.y, ySize);

				//设置地形原型
				SplatPrototype[] newSplats = new SplatPrototype[splatProtos.Length];
				for (int i = 0; i < splatProtos.Length;  ++i)
				{
					newSplats[i] = new SplatPrototype();
					newSplats[i].texture = splatProtos[i].texture;
					newSplats[i].tileSize = splatProtos[i].tileSize;

					float offsetX = (newData.size.x * x) % splatProtos[i].tileSize.x + splatProtos[i].tileOffset.x;
					float offsetY = (newData.size.z * y) % splatProtos[i].tileSize.y + splatProtos[i].tileOffset.y;
					newSplats[i].tileOffset = new Vector2(offsetX, offsetY);
				}
				newData.splatPrototypes = newSplats;

				//设置混合贴图
				float[,,] alphamap = new float[newAlphamapResolution, newAlphamapResolution, splatProtos.Length];
				alphamap = terrainData.GetAlphamaps(x * newData.alphamapWidth, y * newData.alphamapHeight, newData.alphamapWidth, newData.alphamapHeight);
				newData.SetAlphamaps(0, 0, alphamap);

				//设置高度
				int xBase = terrainData.heightmapWidth / slicingCountX;
				int yBase = terrainData.heightmapHeight / slicingCountY;
				float[,] height = terrainData.GetHeights(xBase * x, yBase * y, xBase + 1, yBase + 1);
				newData.SetHeights(0, 0, height);

				string terrainPrefabName = TerrainPrefabSavePath + MapConst.GetSubMapViewBgName(x, y, false) + ".prefab";
				PrefabUtility.CreatePrefab (terrainPrefabName, Terrain.CreateTerrainGameObject(newData));
			}
		}

		EditorUtility.ClearProgressBar();
	}

	static IntVector2 Vector2ToIntVector2(Vector2 vec)
	{
		return new IntVector2 ((int)vec.x, (int)vec.y);
	}
}