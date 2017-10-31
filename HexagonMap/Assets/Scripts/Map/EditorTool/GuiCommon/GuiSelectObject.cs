#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiSelectObject{

	Dictionary<string, GuiObject> mActionDic;
	string[] mKeyArray;
	int mSelectIndex = 0;

	public void Regist(Dictionary<string, GuiObject> actionDic)
	{
		mActionDic = actionDic;
		UpdateKeyArray ();
	}

	public void Regist(string key, GuiObject hdlObject)
	{
		if (null == mActionDic)
			mActionDic = new Dictionary<string, GuiObject> ();
		if (!mActionDic.ContainsKey (key)) {
			mActionDic.Add (key, hdlObject);
			UpdateKeyArray ();
		}
	}

	void UpdateKeyArray()
	{
		mKeyArray = new string[mActionDic.Count];
		int curIndex = 0;
		foreach (var item in mActionDic) {
			mKeyArray [curIndex++] = item.Key;
		}
	}

	public void RunGUI()
	{
		mSelectIndex = GUILayout.SelectionGrid (mSelectIndex, mKeyArray, mKeyArray.Length);
		if (mSelectIndex < mKeyArray.Length) {
			string key = mKeyArray [mSelectIndex];
			mActionDic [key].RunGUI ();
		}
	}
}
#endif