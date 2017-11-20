#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapEditorPainter : GuiObject {

	float mInterval;
	float mLastPainterTime;
	string[] mPainterSize;
	IntVector2 mLastSelectCoord = IntVector2.Zero;
	List<IntVector2> mPainCoordList = new List<IntVector2>();

	public MapEditorPainter(float interval, int maxPainterSize)
	{
		mInterval = interval;
		mLastPainterTime = Time.realtimeSinceStartup;
		if (maxPainterSize > 1) {
			mPainterSize = new string[maxPainterSize];
			for (int i = 0; i < mPainterSize.Length; ++i) {
				mPainterSize [i] = (i + 1).ToString ();
			}
		}
	}

	protected int painterSize {
		get;
		set;
	}

	public override void RunGUI ()
	{
		base.RunGUI ();
		PainterSizeGUI ();
		if (null == Camera.current)
			return;
		if (Input.GetMouseButton (1)) {
			IntVector2 mapCoord = MapLayout.Instance.ScreenPosToMapCoord (Camera.current, Input.mousePosition.xy());
			if (mLastSelectCoord != mapCoord) {
				mLastSelectCoord = mapCoord;
				mPainCoordList = MapLayout.Instance.GetRingCoordList (mapCoord, painterSize + 1, mPainCoordList);
				for (int i = 0; i < mPainCoordList.Count; ++i) {
					DoDraw (mPainCoordList[i]);
					MapEditor.Instance.OnMapDataChanged (mPainCoordList[i]);
				}
			}
		}
	}

	protected abstract void DoDraw (IntVector2 pos);

	void PainterSizeGUI()
	{
		if (null != mPainterSize) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("请选择笔刷大小");
			painterSize = GUILayout.SelectionGrid (painterSize, mPainterSize, mPainterSize.Length);
			GUILayout.EndHorizontal ();
		}
	}
}
#endif