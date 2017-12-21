using UnityEngine;
using System.Collections;

public class spawnNpcScript : MonoBehaviour {

	public GameObject NPCPrefab = null;
	public Transform pathNodeObj = null;
	public float lifeTime = -1.0f;			//the total time of NPC --- if less than zero will not be limited by lifetime
	public float speed = 0.0f;
	public bool isLoop = false;
	/**
	 * GameDiffSt == 1 -> 简单（不产生npc）.
	 * GameDiffSt == 2 -> 正常（不产生npc）.
	 * GameDiffSt == 3 -> 困难(产生npc).
	 */
	[Range(1, 3)]public int GameDiffSt = 3;
	private GameObject NPCObj = null;
	private float delayTime = 0.0f;
	private float lifeTimeNow = 0.0f;
	private npcScript scriptObj = null;
	private GameObject myCamera = null;
	private float xPos = 0.0f;
	private float yPos = 0.0f;

	public void readyToSspawn(float timeT)
	{
		switch (PlayerController.GameGradeVal) {
		case 1:
			if (GameDiffSt <= 2) {
				return;
			}
			break;
		case 2:
			if (GameDiffSt == 2) {
				return;
			}
			break;
		}

		if (!NPCPrefab)
		{
			return;
		}
		
		myCamera = GameObject.Find ("Main Camera");
		delayTime = timeT;
		CancelInvoke ("spawnDetect");
		InvokeRepeating ("spawnDetect", 0.018f, 0.06f);
	}

	void spawnDetect()
	{
		if (delayTime <= 0.0f)
		{
			CancelInvoke ("spawnDetect");
			spawnOneHere();

			/*if (lifeTime <= 0)
			{
				Debug.Log(gameObject.name + " life time is " + lifeTime);
				lifeTime = 10;
			}
			
			if (NPCObj)
			{
				lifeTimeNow = lifeTime;
				CancelInvoke ("lifeTimeDetect");
				InvokeRepeating ("lifeTimeDetect", 0.018f, 0.5f);
			}*/
			return;
		}
		
		delayTime -= 0.06f;
	}
	
	//repeat for lifetiming
	void lifeTimeDetect()
	{
		if (!NPCObj)
		{
			CancelInvoke ("lifeTimeDetect");
			return;
		}
		if (lifeTimeNow <= 0.0f)
		{
			xPos = myCamera.camera.WorldToScreenPoint (NPCObj.transform.position).x;
			yPos = myCamera.camera.WorldToScreenPoint (NPCObj.transform.position).y;

			if (xPos < -5 || xPos > Screen.width + 5 || yPos < -5 || yPos > Screen.height + 5)
			{
				//delete npc
				CancelInvoke ("lifeTimeDetect");
				
				if (NPCObj)
				{
					//only for the lifetime end
					NPCDead(NPCObj);
				}
			}
		}
		
		lifeTimeNow -= 0.5f;
	}
	
	public void NPCDead(GameObject deleteObj)
	{
		if (scriptObj)
		{
			scriptObj.NPCOver ();
		}
		DestroyObject(deleteObj);
		NPCObj = null;
	}

	void spawnOneHere()
	{
		//NPCObj = Instantiate(NPCPrefab, transform.position, Quaternion.identity) as GameObject;
		NPCObj = Instantiate(NPCPrefab, transform.position, transform.rotation) as GameObject;

		if (!NPCObj)
		{
			return;
		}
		
		//find the NPC scritp and init the information of the NPC
		scriptObj = NPCObj.GetComponent<npcScript>();
		
		if (!scriptObj)
		{
			//Debug.Log(NPCObj.name + " has no npcScript");
			return;
		}
		
		scriptObj.initNPCInfor (pathNodeObj, speed, isLoop);
	}

	public void deleteAAA()
	{
		if (NPCObj)
		{
			//only for the lifetime end
			NPCDead(NPCObj);
		}
	}
}
