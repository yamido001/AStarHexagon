#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GuiSelectAction{

	Dictionary<string, Action> mActionDic;
	string[] mKeyArray;
	int mSelectIndex = 0;

	public void Regist(Dictionary<string, Action> actionDic)
	{
		mActionDic = actionDic;
		UpdateKeyArray ();
	}

	public void Regist(string key, Action hdl)
	{
		if (null == mActionDic)
			mActionDic = new Dictionary<string, Action> ();
		if (!mActionDic.ContainsKey (key)) {
			mActionDic.Add (key, hdl);
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
			mActionDic [key].Invoke ();
		}
	}
}
#endif