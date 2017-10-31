#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MapViewBase{

	public static MapEditor Instance {
		get;
		private set;
	}

	GuiSelectObject mSelectPainter;

	void Awake()
	{
		Instance = this;
		DoInit ();
	}
	
	// Update is called once per frame
	void Update () {
		DoUpdate ();
	}

	void OnDestroy(){
		DoDestroy ();
		Instance = this;
	}

	public override void DoInit ()
	{
		base.DoInit ();
		mSelectPainter = new GuiSelectObject ();
		mSelectPainter.Regist ("Block", new MapEditorPainterBlock ());
		mSelectPainter.Regist ("Free", new MapEditorPainterFree ());
	}

	void OnGUI()
	{
		mSelectPainter.RunGUI ();
		if (GUILayout.Button ("保存")) {
			MapDataManager.Instance.SaveConfigData ();
		}
	}

	public void OnMapDataChanged(IntVector2 tileCoord)
	{
		RefreshMapTile (tileCoord);
	}
}
#endif