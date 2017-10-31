#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorPainterFree : MapEditorPainter {

	public MapEditorPainterFree() : base(0.1f){}

	public override void RunGUI ()
	{
		base.RunGUI ();
	}

	protected override void DoDraw (IntVector2 pos)
	{
		MapTileConfigFree configFree = new MapTileConfigFree ();
		configFree.tileCoord = pos;
		MapDataManager.Instance.SeedConfigData (pos.x, pos.y, configFree.Encode());
	}
}
#endif
