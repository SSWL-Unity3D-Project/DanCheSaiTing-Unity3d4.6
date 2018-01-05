using UnityEngine;
using System.Collections;

public class npcScript : MonoBehaviour {

	public GameObject ragballObject = null;
	private Transform pathObject = null;
	private float moveSpeed = 0.0f;
	private bool isLoop = false;			//whether make the path as a loop path

	//move path
	private int nodeNum = -1;
	private int curNodeIndex = 0;
	private int curNodeIndexR = 0;
	private Transform [] nodeArray;
	private float[] rootTimeArray;
	private nodeScript[] nodeScriptArr;
	private float mDistance = 0.0f;
	private int movingState = -1;		//-1 is not moving at all, 1 is moving, 2 is moving staying(for root action)
	private float curRootTime = 0.0f;
	private Transform desNode = null;
	private int randNum = 0;
	private Transform NPCTransform = null;
	private bool fanxiang = false;
	private bool hitLe = false;

	//action
	private Animator animatorNPC;
	// Update is called once per frame
	void Update () {

		if (movingState == 1)
		{
			//is moving
			NPCTransform.position = Vector3.MoveTowards(NPCTransform.position, desNode.position, Time.deltaTime * moveSpeed);
			mDistance = Vector3.Distance(NPCTransform.position, desNode.position);

			if (mDistance < 0.001f)
			{
				movingState = -1;
				curNodeIndexR = curNodeIndex;

				if (!fanxiang)
				{
					curNodeIndex ++;

					if (curNodeIndex < nodeNum)
					{
						desNode = nodeArray[ curNodeIndex ];
					}
					else if (isLoop)
					{
						fanxiang = !fanxiang;
						curNodeIndex -= 2;
					}
					else
					{
						//if not loop, then stop
						movingState = -10;
						rootAction();
						return;
					}
				}
				else 
				{
					curNodeIndex --;

					if (curNodeIndex >= 0)
					{
						desNode = nodeArray[ curNodeIndex ];
					}
					else if (isLoop)
					{
						fanxiang = !fanxiang;
						curNodeIndex += 2;
					}
				}
				
				//arrive at dest, find next action
				detectAction();
			}
		}
		else if (movingState == 2)
		{
			//root time waiting
			curRootTime -= Time.deltaTime;
			
			if (curRootTime <= 0.0f)
			{
				//desNode = nodeArray[ curNodeIndex ];
				//NPCTransform.forward = desNode.position - NPCTransform.position;
				//movingState = 1;
				runAction();
			}
		}
	}

	public void initNPCInfor(Transform pathObj, float speed, bool LoopT)
	{
		animatorNPC = GetComponent<Animator>();
		if (ragballObject)
		{
			ragballObject.SetActive(false);
		}
		
		if (animatorNPC)
		{
			animatorNPC.enabled = true;
		}

		nodeNum = -1;
		curNodeIndex = 0;
		NPCTransform = transform;
		hitLe = false;

		pathObject = pathObj;
		moveSpeed = speed;
		isLoop = LoopT;

		if (pathObject)
		{
			nodeNum = pathObject.childCount;
		}

		if (nodeNum <= 1)
		{
			isLoop = false;
		}
		
		if (nodeNum > 0)
		{
			nodeArray = new Transform[ nodeNum ];
			rootTimeArray = new float[ nodeNum ];
			nodeScriptArr = new nodeScript[ nodeNum ];
			
			for (int i=0; i < nodeNum; i++)
			{
				nodeArray[i] = pathObject.FindChild(i.ToString());
			}
			
			for (int i=0; i < nodeNum;i++)
			{
				nodeScriptArr[i] = nodeArray[i].GetComponent<nodeScript>();
			}
			
			for (int i=0; i < nodeNum;i++)
			{
				if (nodeScriptArr[i])
				{
					rootTimeArray[i] = nodeScriptArr[i].getNodeRootTime();
				}
				else
				{
					rootTimeArray[i] = 0;
				}
			}

			runAction();

			/*if (!animatorNPC)
			{
				Debug.Log( gameObject.name + " has no animator ");
			}*/
		}
		else
		{
			movingState = -1;
		}
	}

	void detectAction()
	{
		curRootTime = rootTimeArray[ curNodeIndexR ];

		if (curRootTime > 0)
		{
			rootAction();
		}
		else
		{
			runAction();
		}
	}

	void rootAction()
	{
		randNum = Random.Range (1, 3);

		if (animatorNPC)
		{
			switch (randNum)
			{
			case 1:
				animatorNPC.SetTrigger ("triRoot1");
				break;
			case 2:
				animatorNPC.SetTrigger ("triRoot2");
				break;
			}
		}

		if (movingState != -10)
		{
			movingState = 2;
		}
	}

	void runAction()
	{
		randNum = Random.Range (1, 3);

		if (animatorNPC)
		{
			switch (randNum)
			{
			case 1:
				animatorNPC.SetTrigger ("triRun1");
				break;
			case 2:
				animatorNPC.SetTrigger ("triRun2");
				break;
			}
		}

		desNode = nodeArray[ curNodeIndex ];
		if (Vector3.Distance(desNode.position, NPCTransform.position) > 1f)
		{
			NPCTransform.forward = desNode.position - NPCTransform.position;
		}
		movingState = 1;
	}

	public void NPCOver()
	{
		movingState = -1;
	}

	public void OnTriggerEnter(Collider colObj)
	{
		if (colObj.transform.tag == "player" && !hitLe)
		{
            OnDestroyThis();
        }
	}	

    public void OnDestroyThis()
    {
        if (!hitLe)
        {
            hitLe = true;
            if (ragballObject)
            {
                if (animatorNPC)
                {
                    animatorNPC.enabled = false;
                }
                ragballObject.SetActive(true);
            }
            Invoke("DeadLe", 1.0f);
        }
    }

	void DeadLe()
	{
		if (gameObject)
		{
			Destroy (gameObject);
		}
	}
}
