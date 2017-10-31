using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileBlock : MapTileBase{

	int mCurModelKey = int.MinValue;
	GameObject mModelObj;

	public MapTileBlock(Transform viewTf) : base(viewTf){}

	protected override void OnInit ()
	{
		
	}

	public void Refresh()
	{
		RefreshModel ();
	}

	protected override void OnDestroy ()
	{
		mCurModelKey = int.MinValue;
		DestroyModel ();
	}

	void RefreshModel()
	{
		MapTileConfigBlock configBlock = mConfigData as MapTileConfigBlock;
		if (mCurModelKey == configBlock.GetModelKey ())
			return;
		DestroyModel ();
		mCurModelKey = configBlock.GetModelKey ();
		string modelName = MapPrefabDefine.GetBlockPrefabName (mCurModelKey);
		if (null != modelName) {
			string resourcePath = "Map/Prefab/Block/" + modelName;
			GameObject prefab = Resources.Load<GameObject> (resourcePath);
			mModelObj = GameObject.Instantiate (prefab);
			Vector3 mapPos = MapLayout.Instance.GetTilePos (configBlock.tileCoord.x, configBlock.tileCoord.y);
			mModelObj.transform.SetParent (mViewTf);
			mModelObj.transform.ResetAndToPos (mapPos);
			mModelObj.name = configBlock.tileCoord.ToString ();
		}
	}

	void DestroyModel()
	{
		if (null != mModelObj) {
			GameObject.DestroyImmediate (mModelObj);
			mModelObj = null;
		}
	}
}
