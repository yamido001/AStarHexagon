using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPrefabDefine{

	#region 地图阻挡点

	static Dictionary<int, string> blockModelList = new Dictionary<int, string>(){
		{1, "Block1"},
		{2, "Block2"},
		{3, "Block3"},
	};

	#if UNITY_EDITOR
	public const int emptyBlockModelKey = 0;
	public static List<int> GetBlockModelKeyList()
	{
		return new List<int> (blockModelList.Keys);
	}
	#endif

	static string[] blockQuickFindArray;
	static int blockFindKeyOffset;
	public static string GetBlockPrefabName(int modelKey)
	{
		//快速查询，只是验证想法，这里应该直接 Dictionary 性能不应该有问题
		if (null == blockQuickFindArray) {
			int minKey = int.MaxValue;
			int maxKey = int.MinValue;
			Dictionary<int, string>.Enumerator modelDicEnumerator = blockModelList.GetEnumerator ();
			while (modelDicEnumerator.MoveNext ()) {
				int curKey = modelDicEnumerator.Current.Key;
				minKey = Mathf.Min (minKey, curKey);
				maxKey = Mathf.Max (maxKey, curKey);
			}
			blockQuickFindArray = new string[maxKey - minKey + 1];
			blockFindKeyOffset = -minKey;
			modelDicEnumerator = blockModelList.GetEnumerator ();
			while (modelDicEnumerator.MoveNext ()) {
				int curKey = modelDicEnumerator.Current.Key;
				blockQuickFindArray [curKey + blockFindKeyOffset] = modelDicEnumerator.Current.Value;
			}
		}
		int findIndex = blockFindKeyOffset + modelKey;
		if(findIndex >= 0 && findIndex < blockQuickFindArray.Length)
			return blockQuickFindArray[findIndex];
		return "Block1";
	}
	#endregion


}
