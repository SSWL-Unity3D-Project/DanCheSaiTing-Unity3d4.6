using UnityEngine;
using System.Collections;

public class liziCloseTrigger : MonoBehaviour {
	
	public GameObject[] closeLiziArr;
	private int closeNum = 0;
	
	// Use this for initialization
	void Start () {
		
		closeNum = closeLiziArr.Length;
	}
	
	public void closeLizi()
	{
		for (int i=0; i < closeNum; i++)
		{
			if (closeLiziArr[i])
			{
				closeLiziArr[i].SetActive(false);
			}
		}
	}
}
