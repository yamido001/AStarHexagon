using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapTileBase{

	protected MapTileConfigBase mConfigData;
	protected Transform mViewTf;

	public MapTileBase(Transform viewTf)
	{
		mViewTf = viewTf;
		OnInit ();
	}

	public void SetConfigData(MapTileConfigBase configData)
	{
		mConfigData = configData;
	}

	public void Destory()
	{
		OnDestroy ();
	}

	protected abstract void OnInit ();

	protected abstract void OnDestroy();
}
