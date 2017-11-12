using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLayout : SingleInstance<MapLayout>{

	public MapLayout()
	{
		InitBlockGridMeshInfo ();
	}

	~MapLayout()
	{
		MapLayout.ClearInstance ();
	}

	//参考资料：http://www.redblobgames.com/grids/hexagons/#line-drawing
	//顶点顺序如下，下面这个是(0,0)点的坐标，（0，0）
	//		地图左边缘||  	  1
	//		地图左边缘||    6 	 2
	//		地图左边缘||(x=0)
	//		地图左边缘||    5		 3
	//		地图左边缘||  	 4
	//	地图下边缘--------------------,顶点4坐在的y轴坐标为0

	//六边形的边长为Radius = MapConst.MapHexagonRadius
	//横向两个六边形中心点之间的距离是 sqrt(3) * Radius
	//纵向两个六边形中心店之间的距离是 Radius / 2 * 3
	//(0,0)六边形的中心点处于 (sqrt(3) * Radius / 2, Radius)

	//地图六边形采用Offset coordinates的“even-q” vertical layout的xy轴坐标兑换后的，偶数行向左偏移
	//
	//
	//
	//
	//		(0,2)(1,2)
	//	  (0,1)(1,1)	
	//		(0,0)(1,0)
	//

	public const float sqrt3 = 1.732050807568877f;
	float mHorizonalSizeHalf = sqrt3 / 2f * MapConst.MapHexagonRadius;
	float mVerticalVerticalCenterPosDistant = MapConst.MapHexagonRadius * 3 / 2f;

	public IntVector3 TilePosToHexCubePos(IntVector2 tilePos)
	{
		IntVector3 ret;
		ret.x = tilePos.y;
		ret.z = tilePos.x - (tilePos.y + (tilePos.y & 1)) / 2;
		ret.y = -ret.x-ret.z;
		return ret;
	}

	public IntVector2 HexCubePosToTilePos(IntVector3 hexCubePos)
	{
		IntVector2 ret;
		ret.y = hexCubePos.x;
		ret.x = hexCubePos.z + (hexCubePos.x + (hexCubePos.x & 1)) / 2;
		return ret;
	}

	public Vector3 GetTilePos(int tileX, int tileY) 
	{
		int offset = 1;
		if ((tileY & 0x1) == 1) {
			//偶数行向左偏移，因为tileY是以0开始，所以偶数行二进制的末尾为1
			offset = 0;
		}
		float xPos = mHorizonalSizeHalf * (tileX * 2 + 1 + offset);
		float zPos = tileY * mVerticalVerticalCenterPosDistant + MapConst.MapHexagonRadius;
		return new Vector3(xPos, 0.01f, zPos);
	}

	public Vector3 GetBlockPos(int blockX, int blockY)
	{
		Vector3 retPos = Vector3.zero;
		retPos.x = MapConst.MapBlockSize.x * MapConst.MapHexagonRadius * sqrt3 * blockX;
		retPos.z = MapConst.MapBlockSize.y * mVerticalVerticalCenterPosDistant * blockY;
		return retPos;
	}

	public Vector2 GetBlockSize()
	{
		Vector3 retSize = Vector3.zero;
		retSize.x = MapConst.MapBlockSize.x * MapConst.MapHexagonRadius * sqrt3;
		retSize.y = MapConst.MapBlockSize.y * mVerticalVerticalCenterPosDistant;
		return retSize;
	}

	public Vector2 GetMapSize()
	{
		Vector2 ret;
		//横向涉及到特定列偏移，所以总共多出一半的大小
		ret.x = (MapConst.MapSize.x + 0.5f) * MapConst.MapHexagonRadius * sqrt3;
		//纵向为(n - 1) * 顶点间隔 + 2 * MapConst.MapHexagonRadius 
		ret.y = (MapConst.MapSize.y - 1) * MapConst.MapHexagonRadius * 3 / 2f + MapConst.MapHexagonRadius * 2f;
		return ret;
	}

	public int[] BlockHexagonGridMeshTriangles {
		get;
		private set;
	}

	public  Vector3[] BlockGridMeshVertices
	{
		get;
		private set;
	}

	public Color[] BlockGridMeshColors
	{
		get;
		private set;
	}

	static Vector3[] vertexOffsetPos = new Vector3[]{
		new Vector3(0f, 0f, 1f) * MapConst.MapHexagonRadius,
		new Vector3(sqrt3 / 2f, 0f, 0.5f) * MapConst.MapHexagonRadius,
		new Vector3(sqrt3 / 2f, 0f, -0.5f) * MapConst.MapHexagonRadius,
		new Vector3(0f, 0f, -1f) * MapConst.MapHexagonRadius,
		new Vector3(-sqrt3 / 2f, 0f, -0.5f) * MapConst.MapHexagonRadius,
		new Vector3(-sqrt3 / 2f, 0f, 0.5f) * MapConst.MapHexagonRadius,
	};

	static Vector3[] outVertexPos = new Vector3[vertexOffsetPos.Length];
	static Vector3[] intVertexPos = new Vector3[vertexOffsetPos.Length];

	void InitBlockGridMeshInfo()
	{
		//获取六边形格子的边，第一版本先不考虑两个六边形相邻时公用边的情况。每个六边形都是从顶点开始向中心点偏移一定的距离.
		int blockTileLineCount = MapConst.MapBlockSize.x * MapConst.MapBlockSize.y * 6;
		BlockGridMeshVertices = new Vector3[blockTileLineCount * 4];				//每个六边形边缘的线由四个顶点
		BlockGridMeshColors = new Color[blockTileLineCount * 4];
		BlockHexagonGridMeshTriangles = new int[blockTileLineCount * 2 * 3];			//每个六边形边缘的线由两个三角面


		for (int x = 0; x < MapConst.MapBlockSize.x; ++x) {
			for (int y = 0; y < MapConst.MapBlockSize.y; ++y) {
				int hexagonLineIndex = (x * MapConst.MapBlockSize.y + y) * 6;
				Vector3 centerPos = GetTilePos (x, y);

				for (int k = 0; k < vertexOffsetPos.Length; ++k) {
					outVertexPos [k] = centerPos + vertexOffsetPos [k];
					intVertexPos [k] = centerPos + vertexOffsetPos [k] * 0.9f;
				}

				//每个六边形有六条边
				//		     o1
				//		     i1
				//	  o6			o2
				//		i6		  i2
				//		i5		  i3	
				//	  o5			o3
				//			 i4
				//		     o4
				//
				//
				for (int k = 0; k < 6; ++k) {
					//每条边的4个顶点的顺序是o1 02 i2 i1
					//每条边的三角形顺序是01 02 i1,  i1 o2 i2
					int nextIndex = k == 5 ? 0 : k + 1;
					int verticesIndex = 4 * (hexagonLineIndex + k);

					BlockGridMeshVertices [verticesIndex] = outVertexPos [k];
					BlockGridMeshVertices [verticesIndex + 1] = outVertexPos [nextIndex];
					BlockGridMeshVertices [verticesIndex + 2] = intVertexPos [nextIndex];
					BlockGridMeshVertices [verticesIndex + 3] = intVertexPos [k];

					int colorsIndex = verticesIndex;
					BlockGridMeshColors[colorsIndex] = new Color(1f, 1f, 1f, 0.8f);
					BlockGridMeshColors[colorsIndex + 1] = new Color(1f, 1f, 1f, 0.8f);
					BlockGridMeshColors[colorsIndex + 2] = new Color(1f, 1f, 1f, 0f);
					BlockGridMeshColors[colorsIndex + 3] = new Color(1f, 1f, 1f, 0f);

					int trianglesIndex = 2 * 3 * (hexagonLineIndex + k);
					BlockHexagonGridMeshTriangles [trianglesIndex] = verticesIndex;
					BlockHexagonGridMeshTriangles [trianglesIndex + 1] = verticesIndex + 1;
					BlockHexagonGridMeshTriangles [trianglesIndex + 2] = verticesIndex + 3;
					BlockHexagonGridMeshTriangles [trianglesIndex + 3] = verticesIndex + 3;
					BlockHexagonGridMeshTriangles [trianglesIndex + 4] = verticesIndex + 1;
					BlockHexagonGridMeshTriangles [trianglesIndex + 5] = verticesIndex + 2;
				}
			}
		}
	}

	#region 方向相关
	public enum MapDirection
	{
		Left,
		Right,
		LeftTop,
		LeftBottom,
		RightTop,
		RightBottom,
	}

	public int GetNextCoordList(IntVector2 coord, IntVector2[] fillArray)
	{
		int curIndex = 0;
		IntVector2 leftCoord = GetMoveDirectionCoord (coord, MapDirection.Left);
		if (MapDataManager.Instance.IsValidTileCoord (leftCoord.x, leftCoord.y))
			fillArray[curIndex++] = leftCoord;

		IntVector2 rightCoord = GetMoveDirectionCoord (coord, MapDirection.Right);
		if (MapDataManager.Instance.IsValidTileCoord (rightCoord.x, rightCoord.y))
			fillArray[curIndex++] = rightCoord;

		IntVector2 leftTopCoord = GetMoveDirectionCoord (coord, MapDirection.LeftTop);
		if (MapDataManager.Instance.IsValidTileCoord (leftTopCoord.x, leftTopCoord.y))
			fillArray[curIndex++] = leftTopCoord;

		IntVector2 leftBottomCoord = GetMoveDirectionCoord (coord, MapDirection.LeftBottom);
		if (MapDataManager.Instance.IsValidTileCoord (leftBottomCoord.x, leftBottomCoord.y))
			fillArray[curIndex++] = leftBottomCoord;

		IntVector2 rightTopCoord = GetMoveDirectionCoord (coord, MapDirection.RightTop);
		if (MapDataManager.Instance.IsValidTileCoord (rightTopCoord.x, rightTopCoord.y))
			fillArray[curIndex++] = rightTopCoord;

		IntVector2 rightBottomCoord = GetMoveDirectionCoord (coord, MapDirection.RightBottom);
		if (MapDataManager.Instance.IsValidTileCoord (rightBottomCoord.x, rightBottomCoord.y))
			fillArray[curIndex++] = rightBottomCoord;
		return curIndex;
	}

	public List<IntVector2> GetNextCoordList(IntVector2 coord, List<IntVector2> fillList = null)
	{
		List<IntVector2> ret = fillList;
		if (null == ret)
			ret = new List<IntVector2> ();
		IntVector2 leftCoord = GetMoveDirectionCoord (coord, MapDirection.Left);
		if (MapDataManager.Instance.IsValidTileCoord (leftCoord.x, leftCoord.y))
			ret.Add (leftCoord);

		IntVector2 rightCoord = GetMoveDirectionCoord (coord, MapDirection.Left);
		if (MapDataManager.Instance.IsValidTileCoord (rightCoord.x, rightCoord.y))
			ret.Add (rightCoord);

		IntVector2 leftTopCoord = GetMoveDirectionCoord (coord, MapDirection.Left);
		if (MapDataManager.Instance.IsValidTileCoord (leftTopCoord.x, leftTopCoord.y))
			ret.Add (leftTopCoord);

		IntVector2 leftBottomCoord = GetMoveDirectionCoord (coord, MapDirection.Left);
		if (MapDataManager.Instance.IsValidTileCoord (leftBottomCoord.x, leftBottomCoord.y))
			ret.Add (leftBottomCoord);

		IntVector2 rightTopCoord = GetMoveDirectionCoord (coord, MapDirection.Left);
		if (MapDataManager.Instance.IsValidTileCoord (rightTopCoord.x, rightTopCoord.y))
			ret.Add (rightTopCoord);

		IntVector2 rightBottomCoord = GetMoveDirectionCoord (coord, MapDirection.Left);
		if (MapDataManager.Instance.IsValidTileCoord (rightBottomCoord.x, rightBottomCoord.y))
			ret.Add (rightBottomCoord);

		return ret;
	}

	public IntVector2 GetMoveDirectionCoord(IntVector2 coord, MapDirection dir)
	{
		IntVector2 ret = coord;
		bool isEvenRow = (coord.y & 0x1) == 1;
		switch (dir) {
		case MapDirection.Left:
			ret.x -= 1;
			break;
		case MapDirection.Right:
			ret.x += 1;
			break;
		case MapDirection.LeftTop:
			ret.y += 1;
			if (isEvenRow)
				ret.x -= 1;
			break;
		case MapDirection.LeftBottom:
			ret.y -= 1;
			if (isEvenRow)
				ret.x -= 1;
			break;
		case MapDirection.RightTop:
			ret.y += 1;
			if (!isEvenRow)
				ret.x += 1;
			break;
		case MapDirection.RightBottom:
			ret.y -= 1;
			if (!isEvenRow)
				ret.x += 1;
			break;
		default:
			break;
		}
		if (!MapDataManager.Instance.IsValidTileCoord (ret.x, ret.y)) {
			ret = MapDataManager.InValidMapCoord;
		}
		return ret;
	}
	#endregion

	Plane mCachedPlane = new Plane(Vector3.up, Vector3.zero);
	int[] mYCenterIndex = new int[2];
	//TODO 异常点坐标目前不能够过滤出来
	public IntVector2 ScreenPosToMapCoord(Camera mapCamera, Vector2 screenPos)
	{
		Vector3 mapPos = ScreenPosToMapPos (mapCamera, screenPos);
		//六边形纵向第一行的中心点的y轴坐标为MapConst.MapHexagonRadius,以后每相邻两行之间的间隔为MapConst.MapHexagonRadius * 3 / 2f

		IntVector2 retCoord = IntVector2.Zero;
		if (mapPos.z < MapConst.MapHexagonRadius) {
			//纵轴方向处于首列
			mYCenterIndex [0] = 0;
			mYCenterIndex [1] = -1;
		} else {
			mYCenterIndex [0] = (int)((mapPos.z - MapConst.MapHexagonRadius) / (MapConst.MapHexagonRadius * 3f / 2f));
			float centerOffset = (mapPos.z - MapConst.MapHexagonRadius) % (MapConst.MapHexagonRadius * 3f / 2f);
			if (centerOffset < MapConst.MapHexagonRadius / 2f) {
				//一定当前这一纵列中
				mYCenterIndex [1] = -1;
			} else if (centerOffset > MapConst.MapHexagonRadius) {
				//和当前中心点的距离超过半径了，处于下一列内
				mYCenterIndex [0] += 1;
				mYCenterIndex [1] = -1;
			} else {
				//处于两列内
				mYCenterIndex [1] = mYCenterIndex [0] + 1;
			}
		}

		if (mYCenterIndex [1] == -1) {
			//能够确定了纵轴方向的坐标
			if ((mYCenterIndex [0] & 0x1) == 1) {
				//偶数行向左偏移了,偶数行坐标从0开始，奇数列坐标从0.5倍宽开始
				retCoord.x = (int)(mapPos.x / (sqrt3 * MapConst.MapHexagonRadius));
			} else {
				retCoord.x = (int)(mapPos.x / (sqrt3 * MapConst.MapHexagonRadius) - 0.5f);
			}
			retCoord.y = mYCenterIndex [0];
		} else {
			//通过x坐标找到可能相邻的两个六边形，然后判断距离哪个六边形的中点近
			//        
			//     * 
			//  *     *
			//	   1
			//  *     *
			//*	   *     *
			//  2     3
			//*	   *    *
			//	*     *
			//     4
			//  *	  *
			//     *
			float floatXIndex = mapPos.x / (sqrt3 * MapConst.MapHexagonRadius);
			int intXIndex = (int)floatXIndex;
			if ((mYCenterIndex [0] & 0x1) == 1) {
				//可能在两行之间，纵向最小坐标为偶数行
				//需要在12、13之间判断
				if (floatXIndex <= 0.5f) {
					//第一列
					retCoord.x = 0;
					retCoord.y = mYCenterIndex [0];
				} else if (floatXIndex % 1 > 0.5f) {
					//12之间比较
					Vector3 pos1 = GetTilePos(intXIndex, mYCenterIndex[1]);
					Vector3 pos2 = GetTilePos(intXIndex, mYCenterIndex[0]);
					retCoord.x = intXIndex;
					if ((mapPos - pos1).sqrMagnitude > (mapPos - pos2).sqrMagnitude) {
						retCoord.y = mYCenterIndex [0];
					} else {
						retCoord.y = mYCenterIndex [1];
					}
				} else {
					//13之间比较
					Vector3 pos1 = GetTilePos(intXIndex - 1, mYCenterIndex[1]);
					Vector3 pos3 = GetTilePos(intXIndex, mYCenterIndex[0]);
					if ((mapPos - pos1).sqrMagnitude > (mapPos - pos3).sqrMagnitude) {
						retCoord.y = mYCenterIndex [0];
						retCoord.x = intXIndex;
					} else {
						retCoord.y = mYCenterIndex [1];
						retCoord.x = intXIndex - 1;
					}
				}
			} else {
				//可能在两行之间，纵向最小坐标为奇数行
				//需要在24、34之间判断
				if (floatXIndex <= 0.5f) {
					//第一列
					retCoord.x = 0;
					retCoord.y = mYCenterIndex [1];
				} else if (floatXIndex % 1 > 0.5f) {
					//24之间比较
					Vector3 pos2 = GetTilePos(intXIndex, mYCenterIndex[1]);
					Vector3 pos4 = GetTilePos(intXIndex, mYCenterIndex[0]);
					retCoord.x = intXIndex;
					if ((mapPos - pos2).sqrMagnitude > (mapPos - pos4).sqrMagnitude) {
						retCoord.y = mYCenterIndex [0];
					} else {
						retCoord.y = mYCenterIndex [1];
					}

				} else {
					//34之间比较
					Vector3 pos3 = GetTilePos(intXIndex, mYCenterIndex[1]);
					Vector3 pos4 = GetTilePos(intXIndex - 1, mYCenterIndex[0]);
					if ((mapPos - pos3).sqrMagnitude > (mapPos - pos4).sqrMagnitude) {
						retCoord.y = mYCenterIndex [0];
						retCoord.x = intXIndex - 1;
					} else {
						retCoord.y = mYCenterIndex [1];
						retCoord.x = intXIndex;
					}
				}
			}
		}
		retCoord.x = Mathf.Min (retCoord.x, MapConst.MapSize.x);
		retCoord.y = Mathf.Min (retCoord.y, MapConst.MapSize.y);
		return retCoord;
	}

	public HashSet<int> GetVisiableBlock(Camera mapCamera, HashSet<int> fillSet = null)
	{
		HashSet<int> ret = fillSet != null ? fillSet : new HashSet<int> ();
		//地图的可见区域是一个四边形，因为当前摄像机是不能旋转的，所以只取左上和右下点
		IntVector2 leftBottom = ScreenPosToMapCoord(mapCamera, new Vector2(0f, 0f));
		IntVector2 leftTop = ScreenPosToMapCoord (mapCamera, new Vector2 (0f, Screen.height));
		IntVector2 rightTop = ScreenPosToMapCoord (mapCamera, new Vector2 (Screen.width, Screen.height));
		IntVector2 leftBottomBlock = MapDataManager.TileCoordToBlockCoord (leftBottom);
		IntVector2 rightTopBlock = MapDataManager.TileCoordToBlockCoord (rightTop);
		IntVector2 leftTopBlock = MapDataManager.TileCoordToBlockCoord (leftTop);
		for (int i = leftTopBlock.x; i <= rightTopBlock.x; ++i) {
			for (int j = leftBottomBlock.y; j <= leftTopBlock.y; ++j) {
				ret.Add(MapDataManager.BlockCoordToBlockId(i, j));
			}
		}
		return ret;
	}

	public Vector3 ScreenPosToMapPos(Camera mapCamera, Vector2 screenPos)
	{
		Ray ray = mapCamera.ScreenPointToRay (new Vector3 (screenPos.x, screenPos.y, 0f));
		float dis;
		if (mCachedPlane.Raycast (ray, out dis)) {
			return ray.GetPoint (dis);
		}
		return Vector3.zero;
	}


}
