using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapViewBase : MonoBehaviour {

	public SubMapView subMapViewTemplate;
	public Camera mapCamera;

	Transform mCameraCachedTransform;
	InputWrapper mInputWrapper;
	protected Dictionary<int, SubMapView> mSubMapViews = new Dictionary<int, SubMapView>();
	GameObject mObjHightLight;

	#region 生命周期
	public virtual void DoInit ()
	{
		mCameraCachedTransform = mapCamera.transform;
		subMapViewTemplate.gameObject.SetActive (false);
		mInputWrapper = new InputWrapper (OnClick, OnDrag, OnPinching);
		RefreshVisiableSubMapView ();
	}

	public virtual void DoUpdate()
	{
		mInputWrapper.Update ();
	}

	public virtual void DoDestroy ()
	{
		
	}
	#endregion

	public void RefreshMapTile(IntVector2 tileCoord)
	{
		SubMapView subMapView = null;
		int blockId = MapDataManager.TileCoordToBlockId (tileCoord);
		if (mSubMapViews.TryGetValue (blockId, out subMapView)) {
			subMapView.RefreshTile (tileCoord);
		}
	}

	#region 可见地块的刷新
	List<int> mToRemoveBlockIdList = new List<int>();
	/// <summary>
	/// 刷新可见地块，当可见地块改变时调用
	/// </summary>
	void RefreshVisiableSubMapView()
	{
		HashSet<int> visibleBlockSet = GetVisiableMapBlocks ();

		//找到哪些子地块已经变得不可见
		mToRemoveBlockIdList.Clear ();
		var subMapViewEnumerator = mSubMapViews.GetEnumerator ();
		while (subMapViewEnumerator.MoveNext ()) {
			if (!visibleBlockSet.Contains (subMapViewEnumerator.Current.Key)) {
				mToRemoveBlockIdList.Add (subMapViewEnumerator.Current.Key);
			}
		}

		//不可见地块删除
		for (int i = 0; i < mToRemoveBlockIdList.Count; ++i) {
			int toRemoveBlockId = mToRemoveBlockIdList [i];
			SubMapView removeSubMapView = mSubMapViews [toRemoveBlockId];
			mSubMapViews.Remove (toRemoveBlockId);
			RecoverSubMapView (removeSubMapView);
		}

		var visibleBlockSetEnum = visibleBlockSet.GetEnumerator ();
		while (visibleBlockSetEnum.MoveNext ()) {
			if (mSubMapViews.ContainsKey (visibleBlockSetEnum.Current))
				continue;
			SubMapView newSubMapView = CreateSubMapView ();
			mSubMapViews.Add (visibleBlockSetEnum.Current, newSubMapView);
			newSubMapView.SetBlockId (visibleBlockSetEnum.Current);
		}
	}

	HashSet<int> mVisiableBlocks = new HashSet<int>();
	/// <summary>
	/// 获取当前可见地块
	/// </summary>
	/// <returns>The visiable map blocks.</returns>
	HashSet<int> GetVisiableMapBlocks()
	{
		mVisiableBlocks.Clear ();
		MapLayout.Instance.GetVisiableBlock (mapCamera, mVisiableBlocks);
		return mVisiableBlocks;
	}
	#endregion

	#region SubMapView的缓存
	Queue<SubMapView> mCachedSubMapViews = new Queue<SubMapView>();
	SubMapView CreateSubMapView()
	{
		SubMapView ret = null;
		if (mCachedSubMapViews.Count > 0) {
			ret = mCachedSubMapViews.Dequeue ();
			ret.OnOutRecover ();
		} else {
			ret = GameObject.Instantiate<SubMapView>(subMapViewTemplate);
			ret.OnInit ();
			ret.transform.SetParent (transform);
		}
		ret.gameObject.SetActive (true);
		return ret;
	}

	void RecoverSubMapView(SubMapView subMapView)
	{
		subMapView.OnEnterRecover ();
		subMapView.gameObject.SetActive (false);
		mCachedSubMapViews.Enqueue (subMapView);
	}
	#endregion

	#region 摄像机管理
	float CameraHeight
	{
		get{
			return mapCamera.transform.position.y;
		}
		set{
			Vector3 cameraPos = mapCamera.transform.position;
			cameraPos.y = value;
			mapCamera.transform.position = cameraPos;
			OnCameraMove ();
		}
	}
	#endregion

	#region 输入操作
	void OnClick(Vector2 clickPos)
	{
		IntVector2 clickTile = MapLayout.Instance.ScreenPosToMapCoord(mapCamera, clickPos);
		OnClickTile (clickTile);
	}

	void OnDrag(Vector2 curPos, Vector2 deltaPos)
	{
		Vector3 curHitPos = MapLayout.Instance.ScreenPosToMapPos(mapCamera, curPos);
		Vector3 lastHitPos = MapLayout.Instance.ScreenPosToMapPos(mapCamera, curPos - deltaPos);
		MoveCameraOffset (lastHitPos - curHitPos);
	}

	void OnPinching(Vector2 curPos1, Vector2 deltaPos1, Vector2 curPos2, Vector2 deltaPos2)
	{
		float curDis = (curPos1 - curPos2).magnitude;
		float previousDis = ((curPos1 - deltaPos1) - (curPos2 - deltaPos2)).magnitude;
		float rate = (curDis - previousDis) / previousDis;
		CameraHeight = CameraHeight * (1 + rate);
	}
	#endregion

	#region 摄像机操作
	void MoveCameraOffset(Vector3 offsetPos)
	{
		mCameraCachedTransform.position += offsetPos;
		OnCameraMove ();
	}

	void MoveToCamera(Vector3 pos)
	{
		mCameraCachedTransform.position = pos;
		OnCameraMove ();
	}

	void OnCameraMove()
	{
		RefreshVisiableSubMapView ();
	}
	#endregion

	#region 地图点击
	protected virtual void OnClickTile(IntVector2 clickTile)
	{
		if (null == mObjHightLight) {
			mObjHightLight = GameObject.Instantiate(Resources.Load<GameObject> ("Map/Prefab/MapHightLight"));
			mObjHightLight.transform.SetParent (transform);
			mObjHightLight.transform.Reset ();
		}
		mObjHightLight.transform.position = MapLayout.Instance.GetTilePos (clickTile.x, clickTile.y);
	}
	#endregion
}
