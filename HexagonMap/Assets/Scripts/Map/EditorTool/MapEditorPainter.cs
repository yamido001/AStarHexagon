#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapEditorPainter : GuiObject {

	float mInterval;
	float mLastPainterTime;
	public MapEditorPainter(float interval)
	{
		mInterval = interval;
		mLastPainterTime = Time.realtimeSinceStartup;
	}

	public override void RunGUI ()
	{
		base.RunGUI ();
		if (null == Camera.current)
			return;
		if (mLastPainterTime + mInterval > Time.realtimeSinceStartup)
			return;
		mLastPainterTime = Time.realtimeSinceStartup;
		if (Input.GetMouseButton (1)) {
			IntVector2 mapCoord = MapLayout.Instance.ScreenPosToMapCoord (Camera.current, Input.mousePosition.xy());
			DoDraw (mapCoord);
			MapEditor.Instance.OnMapDataChanged (mapCoord);
		}
	}

	protected abstract void DoDraw (IntVector2 pos);
}
#endif