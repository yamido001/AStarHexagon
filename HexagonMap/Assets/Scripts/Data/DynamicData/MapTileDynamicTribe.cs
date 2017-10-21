using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileDynamicTribe : MapTileDynamicBase{

	public override MapTileDynamicType TileType {
		get {
			return MapTileDynamicType.Tribe;
		}
	}
}
