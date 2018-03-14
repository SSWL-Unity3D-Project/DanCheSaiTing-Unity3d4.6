using UnityEngine;
using System.Collections;
using System;

public class NpcController : MonoBehaviour
{
    /// <summary>
    /// 是否是网络控制端(只有网络控制端才有主动控制权限).
    /// </summary>
    [HideInInspector]
    public bool IsNetControlPort = false;
    /// <summary>
    /// 网络信息同步组件.
    /// </summary>
    public NetworkSynchronizeGame mNetSynGame;
    /// <summary>
    /// 用于npc随机.
    /// </summary>
    public GameObject[] NpcObjArray = new GameObject[4];
	private float m_NpcSpeed = 10.0f;
	private int m_NpcIndex = 0;
	public Transform m_NpcPath;
	private int m_NpcPathNum = 0;
	private Vector3[] m_NpcPathPoint;
	private RaycastHit hit;
	private  LayerMask mask;
	public float TimmerSet = 5.0f;
	private float m_Timmer = 0.0f;
	public PlayerController m_player;
	private bool m_IsJiasu = false;
	private bool m_IsJiansu = false;
	public float m_TopSpeedSet = 50.0f;
	public float m_EndSpeedSet = 20.0f;
	private float m_SpeedIndex = 1.0f;
    [HideInInspector]
    public bool m_IsHit = false;
    [HideInInspector]
    public float m_HitTimmer = 0.0f;
    [HideInInspector]
    public Vector3 m_PlayerHit;
    [HideInInspector]
    public Vector3 m_NpcPos;
    /// <summary>
    /// 随机更新主角的记录时间.
    /// </summary>
    float TimeLastRandPlayer = 0f;
    float TimeRandPlayer = 3f;

    /// <summary>
    /// 排名数据.
    /// </summary>
    RankManage.RankData mRankDt = null;
    /// <summary>
    /// npc跑的圈数.
    /// </summary>
    int QuanShuCount = 0;
    float TimeQuanShuVal = 0f;
    float TimeFinishVal = 0f;
    float RandTimeFinish = 0f;

    /// <summary>
    /// 网络消息控制器.
    /// </summary>
    NetworkView mNetViewCom;
    /// <summary>
    /// 设置Npc人物索引.
    /// </summary>
    public void SetNpcIndex(int index)
    {
        mNetViewCom = GetComponent<NetworkView>();
        InitNpcMode(index);
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            mNetViewCom.RPC("RpcGetNpcIndex", RPCMode.OthersBuffered, index);
        }

        if (IsNetControlPort)
        {
            //只允许npc主控制端调用该方法.
            mNetSynGame.Init(mNetViewCom);
        }
    }

    [RPC]
    void RpcGetNpcIndex(int index)
    {
        InitNpcMode(index);
    }

    /// <summary>
    /// 初始化Npc人物模型.
    /// </summary>
    void InitNpcMode(int index)
    {
        Debug.Log("InitNpcMode -> index == " + index);
        for (int i = 0; i < NpcObjArray.Length; i++)
        {
            NpcObjArray[i].SetActive(index == i ? true : false);
        }
        mRankDt = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.RankDtManage.AddRankDt((RankManage.RankEnum)index, false);
    }

    void Start ()
    {
        m_NpcPath = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mNpcDt.m_NpcPathArray[(int)NpcState - 1];
        m_player = GetPlayerController();
        SSGameCtrl.GetInstance().mPlayerDataManage.mAiNpcData.AddAiNpcTr(transform);
        
        m_NpcPathPoint = new Vector3[m_NpcPath.childCount];
		for(int i = 0;i<m_NpcPath.childCount;i++)
		{
			string str = (i+1).ToString();
			m_NpcPathPoint[i] = m_NpcPath.FindChild(str).position;
			mask = 1<<( LayerMask.NameToLayer("shexianjiance"));
		}
	}

    /// <summary>
    /// 运动的路程.
    /// </summary>
    float mDistanceMove = 0.0f;
    private Vector3 PosRecord;

    void FixedUpdate()
	{
        if (!IsNetControlPort)
        {
            return;
        }

        if (QuanShuCount >= PlayerController.GetInstance().QuanShuMax && Time.time - TimeFinishVal >= RandTimeFinish)
        {
            return;
        }
        
        int pathNum = (m_NpcPathNum + 1) % m_NpcPath.childCount;
        if (PlayerController.GetInstance().timmerstar > 5.0f)
        {
            float length = Vector3.Distance(PosRecord, transform.position);
            mDistanceMove += length;
            PosRecord = transform.position;
            if (mRankDt != null)
            {
                mRankDt.UpdataDisMoveValue(mDistanceMove);
                SendNpcDisMoveVal(mDistanceMove);
            }

            if (Time.time - TimeLastRandPlayer > TimeRandPlayer)
            {
                TimeLastRandPlayer = Time.time;
                TimeRandPlayer = (UnityEngine.Random.Range(0, 100) % 4) + 2f;
                m_player = GetPlayerController();
                //Debug.Log(name + " -> randPlayer is " + m_player.name + ", time " + Time.time.ToString("f1") + ", TimeRandPlayer " + TimeRandPlayer);
            }

			if(!m_IsHit)
			{
				m_Timmer+=Time.deltaTime;
				if(m_Timmer>TimmerSet)
				{
					if(m_NpcIndex <= m_player.PathNum)
					{
						m_IsJiansu = false;
						m_IsJiasu = true;
						m_SpeedIndex = UnityEngine.Random.Range(1.2f,1.5f);
					}
					else
					{
						m_IsJiasu = false;
						m_IsJiansu = true;
						m_SpeedIndex = UnityEngine.Random.Range(0.5f,0.8f);
					}
					m_Timmer = 0.0f;
				}

				if(m_IsJiasu)
				{
					m_NpcSpeed = Mathf.Lerp(m_NpcSpeed, (m_player.SpeedMovePlayer * m_SpeedIndex) / 3.6f, 10.0f * Time.deltaTime);
					if(m_NpcSpeed>m_TopSpeedSet)
					{
						m_NpcSpeed = m_TopSpeedSet;
					}
				}

				if(m_IsJiansu)
				{
					m_NpcSpeed = Mathf.Lerp(m_NpcSpeed, (m_player.SpeedMovePlayer * m_SpeedIndex) / 3.6f, 10.0f * Time.deltaTime);
				}

				if(m_NpcSpeed<=20f)
				{
					m_NpcSpeed = UnityEngine.Random.Range(20f, 25f);
				}

				if(m_IsEnd)
				{
					m_NpcSpeed = Mathf.Lerp(m_NpcSpeed,m_EndSpeedSet,10.0f*Time.deltaTime);
				}
				transform.position = Vector3.MoveTowards(transform.position,m_NpcPathPoint[pathNum],m_NpcSpeed*Time.deltaTime);
				transform.forward = Vector3.Lerp( transform.forward,Vector3.Normalize(m_NpcPathPoint[pathNum] - transform.position),9.0f*Time.deltaTime);
				transform.localEulerAngles = new Vector3(-10.0f,transform.localEulerAngles.y,transform.localEulerAngles.z);
				if(!m_IsPubu && Physics.Raycast(transform.position+Vector3.up*25.0f,-Vector3.up,out hit,100.0f,mask.value))
				{
					transform.position = hit.point + Vector3.up*0.8f;
				}
			}
			else
			{
				m_HitTimmer+=Time.deltaTime;
				if(m_HitTimmer>0.4f)
				{
					m_HitTimmer = 0.0f;
					m_IsHit = false;
					rigidbody.isKinematic = true;
				}
				else
				{
					rigidbody.isKinematic = false;
					rigidbody.AddForce(Vector3.Normalize(m_NpcPos - m_PlayerHit)*80.0f,ForceMode.Force);
					transform.forward = Vector3.Lerp( transform.forward,Vector3.Normalize(m_NpcPathPoint[pathNum] - transform.position),15.0f*Time.deltaTime);
					transform.localEulerAngles = new Vector3(-10.0f,transform.localEulerAngles.y,transform.localEulerAngles.z);
				}
			}
		}
		else
		{
			m_NpcSpeed = 2.3f;
			transform.position = Vector3.MoveTowards(transform.position,m_NpcPathPoint[pathNum],m_NpcSpeed*Time.deltaTime);
			transform.forward = Vector3.Lerp( transform.forward,Vector3.Normalize(m_NpcPathPoint[pathNum] - transform.position),30.0f*Time.deltaTime);
			transform.localEulerAngles = new Vector3(-10.0f,transform.localEulerAngles.y,transform.localEulerAngles.z);
			if(!m_IsPubu && Physics.Raycast(transform.position+Vector3.up*25.0f,-Vector3.up,out hit,100.0f,mask.value))
			{
				transform.position = hit.point + Vector3.up*0.8f;
			}
		}			
	}
    
	private bool m_IsPubu = false;
	private bool m_IsEnd = false;
    public enum NpcEnum
    {
        Null,
        Npc01,
        Npc02,
        Npc03,
    }
    public NpcEnum NpcState = NpcEnum.Null;
	void OnTriggerEnter(Collider other)
    {
        if (!IsNetControlPort)
        {
            return;
        }

        if (other.tag == "finish")
        {
            if (Time.time - TimeQuanShuVal >= 20f)
            {
                TimeQuanShuVal = Time.time;
                QuanShuCount++;
                if (PlayerController.GetInstance().QuanShuMax <= QuanShuCount)
                {
                    TimeFinishVal = Time.time;
                    RandTimeFinish = UnityEngine.Random.Range(0.5f, 2.5f);
                    mRankDt.UpdateRankDtTimeFinish(Time.time);
                    SendNpcUpdateRankDtTimeFinish(Time.time);
                }
            }
        }

        if (other.tag == "pathpoint")
		{
			m_NpcIndex = Convert.ToInt32(other.name);
            mRankDt.UpdateRankDtPathPoint(m_NpcIndex, Time.time);
            SendNpcUpdateRankDtPathPoint(m_NpcIndex, Time.time);
        }

		if(other.tag == "npc1" && NpcState == NpcEnum.Npc01)
		{
			m_NpcPathNum = Convert.ToInt32(other.name)-1;
		}

		if(other.tag == "npc2" && NpcState == NpcEnum.Npc02)
		{
			m_NpcPathNum = Convert.ToInt32(other.name)-1;
        }

        if (other.tag == "npc3" && NpcState == NpcEnum.Npc03)
        {
            m_NpcPathNum = Convert.ToInt32(other.name) - 1;
        }

        if (other.tag == "pubuNpc")
		{
			m_IsPubu = true;
		}

		if(other.tag == "outpubuNpc")
		{
			m_IsPubu = false;
		}

		if(other.tag == "dapubunpc")
		{
			m_IsEnd = true;
		}
	}

    /// <summary>
    /// 当导弹击中npc时.
    /// </summary>
    public void OnDaoDanHit(Vector3 posDaoDan)
    {
        if (Vector3.Dot(posDaoDan, transform.position) > 0f)
        {
            m_PlayerHit = transform.position;
            m_NpcPos = posDaoDan;
        }
        else
        {
            m_PlayerHit = posDaoDan;
            m_NpcPos = transform.position;
        }
        m_IsHit = true;
    }

    /// <summary>
    /// 获取主角控制脚本.
    /// </summary>
    PlayerController GetPlayerController()
    {
        int count = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mPlayerControllerList.Count;
        if (count == 0)
        {
            return null;
        }

        int randVal = UnityEngine.Random.Range(0, 100) % count;
        return SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mPlayerControllerList[randVal];
    }

    /// <summary>
    /// 设置Npc的枚举状态.
    /// </summary>
    public void SetNpcState(NpcEnum npcState)
    {
        NpcState = npcState;
        name = npcState.ToString();
    }
    
    public void SetIsNetControlPort(bool isNetControl)
    {
        IsNetControlPort = isNetControl;
    }

    /// <summary>
    /// 发送Npc更新排名路径的消息.
    /// </summary>
    void SendNpcUpdateRankDtPathPoint(int index, float time)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                if (Network.peerType == NetworkPeerType.Server && NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0)
                {
                    //没有玩家选择链接服务器进行联机游戏.
                }
                else
                {
                    mNetViewCom.RPC("RpcNpcUpdateRankDtPathPoint", RPCMode.Others, index, time);
                }
            }
        }
    }

    /// <summary>
    /// 接收Npc更新排名路径的消息.
    /// </summary>
    [RPC]
    void RpcNpcUpdateRankDtPathPoint(int index, float time)
    {
        //Debug.Log("RpcNpcUpdateRankDtPathPoint...");
        mRankDt.UpdateRankDtPathPoint(index, time);
    }

    /// <summary>
    /// 发送Npc更新到达终点的消息.
    /// </summary>
    void SendNpcUpdateRankDtTimeFinish(float time)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                if (Network.peerType == NetworkPeerType.Server && NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0)
                {
                    //没有玩家选择链接服务器进行联机游戏.
                }
                else
                {
                    mNetViewCom.RPC("RpcNpcUpdateRankDtTimeFinish", RPCMode.Others, time);
                }
            }
        }
    }

    /// <summary>
    /// 接收Npc更新到达终点的消息.
    /// </summary>
    [RPC]
    void RpcNpcUpdateRankDtTimeFinish(int index, float time)
    {
        //Debug.Log("RpcNpcUpdateRankDtTimeFinish...");
        mRankDt.UpdateRankDtTimeFinish(time);
    }

    /// <summary>
    /// 发送Npc比赛运动路程的消息.
    /// </summary>
    void SendNpcDisMoveVal(float disVal)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                if (Network.peerType == NetworkPeerType.Server && NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0)
                {
                    //没有玩家选择链接服务器进行联机游戏.
                }
                else
                {
                    mNetViewCom.RPC("RpcNpcDisMoveVal", RPCMode.Others, disVal);
                }
            }
        }
    }

    /// <summary>
    /// 接收Npc比赛运动路程的消息.
    /// </summary>
    [RPC]
    void RpcNpcDisMoveVal(float disVal)
    {
        if (mRankDt != null)
        {
            mRankDt.UpdataDisMoveValue(disVal);
        }
    }
}