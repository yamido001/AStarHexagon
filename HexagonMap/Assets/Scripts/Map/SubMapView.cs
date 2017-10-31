using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMapView : MonoBehaviour {

	Dictionary<int, MapTileBase> mTileDic = new Dictionary<int, MapTileBase> ();
	int mBlockId = MapConst.EmptyBlockId;
	IntVector2 mZeroCoord;
	Terrain mTerrain;
	MeshFilter mGridMeshFilter;
	Transform mCacheTransform;

	/// <summary>
	/// 初始化
	/// </summary>
	public void OnInit()
	{
		mCacheTransform = transform;
	}

	/// <summary>
	/// 删除
	/// </summary>
	public void OnDestroy()
	{
		DestroyTiles ();
		DestroyBackground ();
	}

	/// <summary>
	/// 从缓存中拿出来
	/// </summary>
	public void OnOutRecover()
	{
		
	}

	/// <summary>
	/// 放入缓存，停止使用
	/// </summary>
	public void OnEnterRecover()
	{
		mBlockId = MapConst.EmptyBlockId;
		DestroyTiles ();
		DestroyBackground ();
	}

	public void SetBlockId(int blockId)
	{
		if (mBlockId == blockId)
			return;
		mBlockId = blockId;

		IntVector2 blockCoord = MapDataManager.BlockIdToBlockZeroTileCoord (mBlockId);
		mZeroCoord = MapDataManager.BlockIdToBlockZeroTileCoord (mBlockId);
		gameObject.name = blockCoord.ToString ();
	
		transform.localPosition = MapLayout.Instance.GetBlockPos(blockCoord.x, blockCoord.y);

		DestroyBackground ();
		InitBackground ();
		DestroyTiles ();
		InitTiles ();
	}

	#region 地图上的点
	public void RefreshTile(IntVector2 tileCoord)
	{
		DestroyTile (tileCoord.x, tileCoord.y);
		InitTile(tileCoord.x, tileCoord.y);
	}

	void DestroyTiles()
	{
		var tileDicEnumerator = mTileDic.GetEnumerator ();
		while (tileDicEnumerator.MoveNext ()) {
			tileDicEnumerator.Current.Value.Destory ();
		}
		mTileDic.Clear ();
	}

	void InitTiles()
	{
		for (int i = 0; i < MapConst.MapBlockSize.x + mZeroCoord.x; ++i) {
			for (int j = 0; j < MapConst.MapBlockSize.y + mZeroCoord.y; ++j) {
				InitTile (i, j);
			}
		}
	}

	void InitTile(int x, int y)
	{
		MapTileConfigBase configBase = MapDataManager.Instance.GetTileConfig (x, y);
		int tileKey = MapDataManager.TileCoordToTileKey (x, y);
		switch (configBase.tileType) {
		case MapTileConfigType.Block:
			MapTileBlock tileBlock = new MapTileBlock (mCacheTransform);
			mTileDic.Add (tileKey, tileBlock);
			tileBlock.SetConfigData (configBase);
			tileBlock.Refresh ();
			break;
		case MapTileConfigType.Free:
			{
				MapTileDynamicBase dynamicData = MapDataManager.Instance.GetTileDynamicData (x, y);
				if (null != dynamicData) {
					switch (dynamicData.TileType) {
					case MapTileDynamicType.City:
						break;
					case MapTileDynamicType.Tribe:
						break;
					default:
						break;
					}
				}
			}
			break;
		default:
			break;
		}
	}

	void DestroyTile(int x, int y)
	{
		int tileKey = MapDataManager.TileCoordToTileKey (x, y);
		MapTileBase tile;
		if (mTileDic.TryGetValue (tileKey, out tile)) {
			tile.Destory ();
			mTileDic.Remove (tileKey);
		}
	}
	#endregion

	#region 地图背景
	void InitBackground()
	{
		//创建地图背景
		string resourcePath = "Map/Prefab/Terrain/" + MapConst.GetSubMapViewBgName (mZeroCoord.x, mZeroCoord.y, false);
		GameObject obj = Resources.Load<GameObject>(resourcePath);
		GameObject bgObject = GameObject.Instantiate (obj);
		bgObject.transform.SetParent (transform);
		bgObject.transform.localPosition = Vector3.zero;
		mTerrain = bgObject.GetComponent<Terrain> ();

		//创建地图六边形格子mesh
		if (null == mGridMeshFilter)
			mGridMeshFilter = gameObject.GetComponent<MeshFilter> ();
		if (null == mGridMeshFilter.mesh)
			mGridMeshFilter.mesh = new Mesh ();
		mGridMeshFilter.mesh.vertices = MapLayout.Instance.BlockGridMeshVertices;
		mGridMeshFilter.mesh.triangles = MapLayout.Instance.BlockHexagonGridMeshTriangles;
		mGridMeshFilter.mesh.colors = MapLayout.Instance.BlockGridMeshColors;
	}

	void DestroyBackground()
	{
		if (null != mTerrain) {
			GameObject.DestroyImmediate (mTerrain.gameObject);
			mTerrain = null;
		}
	}
	#endregion
}
