using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MapViewBase {

	public void Awake ()
	{
		DoInit ();
	}

	void Update()
	{
		DoUpdate ();
	}

	void OnDestroy()
	{
		DoDestroy ();
	}

	/// <summary>
	/// 地块的数据改变时调用
	/// </summary>
	/// <param name="blockId">Block identifier.</param>
	public void OnMapTileDataChange(IntVector2 tileCoord)
	{
		RefreshMapTile (tileCoord);
	}

	protected override void OnClickTile (IntVector2 clickTile)
	{
		base.OnClickTile (clickTile);
		switch (mState) {
		case FindPathState.SelectStart:
			mFindPathStart = clickTile;
			break;
		case FindPathState.SelectEnd:
			mFindPathEnd = clickTile;
			break;
		default:
			break;
		}
	}

	void OnGUI(){
		MoveToCoordOnGUI ();
		FindPathOnGUI ();
	}

	#region 寻路操作
	enum FindPathState
	{
		SelectStart,
		SelectEnd,
		Finding,
	}

	IntVector2 mFindPathStart = MapDataManager.InValidMapCoord;
	IntVector2 mFindPathEnd = MapDataManager.InValidMapCoord;
	FindPathState mState;


	void FindPathOnGUI()
	{
		if (GUILayout.Button ("选择起点")) {
			mState = FindPathState.SelectStart;
		}
		if (GUILayout.Button ("选择终点")) {
			mState = FindPathState.SelectEnd;
		}
		if (MapDataManager.Instance.IsValidTileCoord (mFindPathStart.x, mFindPathStart.y) &&
		   MapDataManager.Instance.IsValidTileCoord (mFindPathEnd.x, mFindPathEnd.y)) {
			if (GUILayout.Button ("寻路")) {
				mState = FindPathState.Finding;
				using (new PerformTimer ("寻找路径")) {
					List<IntVector2> findPathList = AStarManager.Instance.FindPath (mFindPathStart, mFindPathEnd);
					if (null != findPathList) {
						System.Text.StringBuilder pathSb = new System.Text.StringBuilder ();
						for (int i = 0; i < findPathList.Count; ++i) {
							pathSb.Append (findPathList [i].ToString ());
							if (i != findPathList.Count - 1)
								pathSb.Append ("->");
						}
						ShowPath (findPathList);
					} else {
						Debug.LogError ("未找到从 " + mFindPathStart.ToString() + " 到 " + mFindPathEnd.ToString() + " 的路径");
					}
				}
			}
		}
	}

	List<GameObject> mCurPathNodeList = new List<GameObject>();
	void ShowPath(List<IntVector2> path)
	{
		for (int i = 0; i < mCurPathNodeList.Count; ++i) {
			RecyclePathNode (mCurPathNodeList [i]);
		}
		mCurPathNodeList.Clear ();

		for (int i = 0; i < path.Count; ++i) {
			IntVector2 curCoord = path [i];
			GameObject pathNode = CreatePathNode ();
			pathNode.transform.position = MapLayout.Instance.GetTilePos (curCoord.x, curCoord.y);
			mCurPathNodeList.Add (pathNode);
		}

		for (int i = 0; i < mCurPathNodeList.Count; ++i) {
			if (i == mCurPathNodeList.Count - 1)
				continue;
			GameObject curNode = mCurPathNodeList [i];
			GameObject nextNode = mCurPathNodeList [i + 1];
			curNode.transform.LookAt (nextNode.transform.position);
		}
	}

	Queue<GameObject> mPathNodeCached = new Queue<GameObject>();
	GameObject CreatePathNode()
	{
		GameObject ret = null;
		if (mPathNodeCached.Count > 0) {
			ret = mPathNodeCached.Dequeue ();
		} else {
			GameObject prefabObj = Resources.Load<GameObject> ("Map/Prefab/FindPath/PathNode");
			ret = GameObject.Instantiate (prefabObj);
		}
		ret.SetActive (true);
		return ret;
	}

	void RecyclePathNode(GameObject node)
	{
		node.SetActive (false);
		mPathNodeCached.Enqueue (node);
	}
	#endregion

	#region 位置跳转逻辑
	int mMoveToX;
	int mMoveToY;
	void MoveToCoordOnGUI()
	{
		GUILayout.Label ("输入跳转坐标");
		string moveXStr = GUILayout.TextField (mMoveToX.ToString()); 
		string moveYStr = GUILayout.TextField (mMoveToY.ToString());
		int.TryParse (moveXStr, out mMoveToX);
		int.TryParse (moveYStr, out mMoveToY);
		if (GUILayout.Button ("跳转")) {
			IntVector2 tileCoord = new IntVector2 (mMoveToX, mMoveToY);
			if (MapDataManager.Instance.IsValidTileCoord (tileCoord.x, tileCoord.y)) {
				MoveToCoord (tileCoord);
			}
		}
	}
	void MoveToCoord(IntVector2 tileCoord)
	{
		Vector3 curFouceMapPos = MapLayout.Instance.ScreenPosToMapPos (mapCamera, new Vector2 (Screen.width / 2f, Screen.height / 2f));
		Vector3 moveToCoordMapPos = MapLayout.Instance.GetTilePos (tileCoord.x, tileCoord.y);
		MoveCameraOffset (moveToCoordMapPos - curFouceMapPos);
	}
	#endregion
}
