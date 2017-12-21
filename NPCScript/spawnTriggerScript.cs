using UnityEngine;
using System.Collections;

public class spawnTriggerScript : MonoBehaviour {
	public GameObject[] SpawnPointArr;
	public float[] SpawnDelayTime;
	private spawnNpcScript[] spawnNPCSobj;
	private int spawnPointNum = 0;
	private int timeNum = 0;

	// Use this for initialization
	void Start () {
		spawnPointNum = 0;
		timeNum = SpawnDelayTime.Length - 1;
		
		for (int i=0; i<SpawnPointArr.Length; i++)
		{
			if (SpawnPointArr[i])
			{
				spawnPointNum ++;
			}
		}
		
		if (spawnPointNum > 0)
		{
			spawnNPCSobj = new spawnNpcScript[spawnPointNum];
			
			for (int i=0; i < spawnPointNum; i++)
			{
				if (SpawnPointArr[i])
				{
					spawnNPCSobj[i] = SpawnPointArr[i].GetComponent<spawnNpcScript>();
				}
			}
		}
	}

	public void BeginSpawn()
	{
		for (int i=0; i < spawnPointNum; i++)
		{
			if (i <= timeNum - 1)
			{
				spawnNPCSobj[i].readyToSspawn(SpawnDelayTime[i]);
			}
			else
			{
				spawnNPCSobj[i].readyToSspawn(0);
			}
		}
	}
}
