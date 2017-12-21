using UnityEngine;
using System.Collections;
using System;

public class LoadingTest : MonoBehaviour {

	// Use this for initialization
	void Start()
	{
		XkGameCtrl.IsLoadingLevel = false;
		Invoke("DelayLoadingLevel", 2.5f);
	}

	void DelayLoadingLevel()
	{
		if (XkGameCtrl.IsLoadingLevel) {
			return;
		}
		StartCoroutine(CheckUnloadUnusedAssets());
	}

	
	IEnumerator CheckUnloadUnusedAssets()
	{
		bool isLoop = true;
		GC.Collect();
		AsyncOperation asyncVal = Resources.UnloadUnusedAssets();
		
		do {
			if (!asyncVal.isDone) {
				yield return new WaitForSeconds(0.5f);
			}
			else {
				//Debug.Log("loadedLevel === "+Application.loadedLevel);
				if (!XkGameCtrl.IsLoadingLevel) {
					if (Application.loadedLevel < 4) {
						Application.LoadLevel(4);
					}
					else {
						Application.LoadLevel(0);
					}
				}
				yield break;
			}
		} while (isLoop);
	}
}