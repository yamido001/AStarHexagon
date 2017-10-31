using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : SingleInstance<AStarManager> {


	class AStarNode
	{
		public IntVector2 coord;
		public float valueG;
		public float valueH;
		public AStarNode previousNode;
	}

	List<AStarNode> mOpenList = new List<AStarNode>();
	HashSet<int> mCloseSet = new HashSet<int>();

	public List<IntVector2> FindPath(IntVector2 fromCoord, IntVector2 toCoord)
	{
		if (!MapDataManager.Instance.IsValidTileCoord (fromCoord.x, fromCoord.y))
			return null;
		if (!MapDataManager.Instance.IsValidTileCoord (toCoord.x, toCoord.y))
			return null;

		mOpenList.Clear ();
		mCloseSet.Clear ();

		AStarNode node = new AStarNode ();
		node.coord = fromCoord;
		node.valueG = 0f;
		node.valueH = CalDis(fromCoord, toCoord);
		mOpenList.Add (node);

		AStarNode finallyNode = null;
		IntVector2[] moveDirectionArray = new IntVector2[6];

		System.Text.StringBuilder logSb = new System.Text.StringBuilder ();

		while (mOpenList.Count > 0) {
			AStarNode curNode = mOpenList [0];
			if (curNode.coord == toCoord) {
				finallyNode = curNode;
				break;
			}
			mOpenList.RemoveAt (0);

//			logSb.Append ("\n" + curNode.coord.ToString() + "  ==>  ");
//			for (int i = 0; i < mOpenList.Count; ++i) {
//				logSb.Append (mOpenList [i].coord.ToString () + "\t\t");
//			}
//
//			if (curNode.coord.x == 2 && curNode.coord.y == 0) {
//				int a = 12;
//				++a;
//			}

			int dirCount = MapLayout.Instance.GetNextCoordList (curNode.coord, moveDirectionArray);
			for (int i = 0; i < dirCount; ++i) {
				IntVector2 childCoord = moveDirectionArray [i];
				if (MapDataManager.Instance.IsBlock (childCoord.x, childCoord.y))
					continue;

				bool isChildNodeInOpen = false;
				for (int j = 0; j < mOpenList.Count; ++j) {
					AStarNode openNode = mOpenList [j];
					if (openNode.coord == childCoord) {
						//当前的childCoord已经在了openListh中了
						if (openNode.valueG > curNode.valueG + MapConst.MapHexagonRadius * MapLayout.sqrt3) {
							//openNode的路径值大于了当前节点的路径值和当前点到openNode的路径值的和。代表从当前点连接到openNode是更快捷的路径
							openNode.previousNode = curNode;
							openNode.valueG = curNode.valueG + MapConst.MapHexagonRadius * MapLayout.sqrt3;
						} else {
							//不需要修改openNode的前节点，当前openNode是快捷的
						}
						isChildNodeInOpen = true;
					}
				}

				if (!isChildNodeInOpen && !mCloseSet.Contains(childCoord.GetHashCode())) {
					AStarNode childNode = new AStarNode ();
					childNode.previousNode = curNode;
					childNode.valueG = curNode.valueG + MapConst.MapHexagonRadius * MapLayout.sqrt3;
					childNode.coord = childCoord;
					childNode.valueH =  CalDis(childCoord, toCoord);
					mOpenList.Add (childNode);
				}
			}
			mOpenList.Sort (delegate(AStarNode x, AStarNode y) {
				return (x.valueG + x.valueH).CompareTo(y.valueG + y.valueH);
			});
			mCloseSet.Add (curNode.coord.GetHashCode ());
		}

//		Debug.LogError (logSb.ToString());

		if (null != finallyNode) {
			List<IntVector2> retList = new List<IntVector2> ();
			while (finallyNode != null) {
				retList.Add (finallyNode.coord);
				finallyNode = finallyNode.previousNode;
			}
			retList.Reverse ();
			return retList;
		}
		return null;
	}

	float CalDis(IntVector2 fromCoord, IntVector2 toCoord)
	{
		Vector2 fromMapPos = MapLayout.Instance.GetTilePos (fromCoord.x, fromCoord.y);
		Vector2 toMapPos = MapLayout.Instance.GetTilePos (toCoord.x, toCoord.y);
		return (fromMapPos - toMapPos).sqrMagnitude;
	}
}
