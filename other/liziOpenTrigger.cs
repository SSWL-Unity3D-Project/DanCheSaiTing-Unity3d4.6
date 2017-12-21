using UnityEngine;
using System.Collections;

public class liziOpenTrigger : MonoBehaviour {
	
	public GameObject[] openLiziArr;
	private int openNum = 0;
	
	// Use this for initialization
	void Start () {
		
		openNum = openLiziArr.Length;
	}
	
	public void openLizi()
	{
		for (int i=0; i < openNum; i++)
		{
			if (openLiziArr[i])
			{
				openLiziArr[i].SetActive(true);
			}
		}
	}
}
