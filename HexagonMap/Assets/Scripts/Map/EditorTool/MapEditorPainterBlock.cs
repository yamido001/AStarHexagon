#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorPainterBlock : MapEditorPainter {

	int mModelIndex = 0;
	int[] mModelIntArray;
	string[] mModelStrArray;

	public MapEditorPainterBlock() : base(0.1f, 5){
		var modelKeyList = MapPrefabDefine.GetBlockModelKeyList ();
		mModelIntArray = new int[modelKeyList.Count];
		mModelStrArray = new string[mModelIntArray.Length + 1];

		for (int i = 0; i < modelKeyList.Count; ++i) {
			mModelIntArray[i] = modelKeyList [i];
			mModelStrArray[i] = modelKeyList [i].ToString ();
		}
		mModelStrArray [mModelStrArray.Length - 1] = "空";
	}

	public override void RunGUI ()
	{
		base.RunGUI ();
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("选择阻挡点类型");
		mModelIndex = GUILayout.SelectionGrid (mModelIndex, mModelStrArray, mModelStrArray.Length);
		GUILayout.EndHorizontal ();
	}

	protected override void DoDraw (IntVector2 pos)
	{
		MapTileConfigBlock configBlock = new MapTileConfigBlock ();
		configBlock.tileCoord = pos;

		if (mModelIndex >= mModelIntArray.Length) {
			configBlock.SetModelkey (MapPrefabDefine.emptyBlockModelKey);
		} else {
			configBlock.SetModelkey (mModelIntArray [mModelIndex]);
		}

		MapDataManager.Instance.SeedConfigData (pos.x, pos.y, configBlock.Encode());
	}
}
#endif