using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileDynamicCity : MapTileDynamicBase{

	public override MapTileDynamicType TileType {
		get {
			return MapTileDynamicType.City;
		}
	}
}
