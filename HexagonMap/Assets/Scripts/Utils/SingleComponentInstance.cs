using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleComponentInstance<T> : MonoBehaviour where T : Component{

	public static T Instance {
		get;
		private set;
	}

	public virtual void Awake(){
		Instance = this as T;
	}

	public virtual void OnDestroy(){
		Instance = null;
	}
}
