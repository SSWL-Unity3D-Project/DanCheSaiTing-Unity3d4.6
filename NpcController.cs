using UnityEngine;
using System.Collections;
using System;

public class NpcController : MonoBehaviour 
{
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
	private Rigidbody m_playerRig;
	private bool m_IsJiasu = false;
	private bool m_IsJiansu = false;
	public float m_TopSpeedSet = 50.0f;
	public float m_EndSpeedSet = 20.0f;
	//public bool m_IsFirstNpc = false;
	private float m_SpeedIndex = 1.0f;
    [HideInInspector]
    public bool m_IsHit = false;
    [HideInInspector]
    public float m_HitTimmer = 0.0f;
    [HideInInspector]
    public Vector3 m_PlayerHit;
    [HideInInspector]
    public Vector3 m_NpcPos;
    public static int NpcIndexVal = 0;
	void Start ()
    {
        m_playerRig = m_player.GetComponent<Rigidbody>();
        if (NpcIndexVal >= NpcObjArray.Length)
        {
            NpcIndexVal = -1;
        }
        NpcIndexVal++;

        for (int i = 0; i < NpcObjArray.Length; i++)
        {
            NpcObjArray[i].SetActive(NpcIndexVal == i ? true : false);
        }

        m_NpcPathPoint = new Vector3[m_NpcPath.childCount];
		for(int i = 0;i<m_NpcPath.childCount;i++)
		{
			string str = (i+1).ToString();
			m_NpcPathPoint[i] = m_NpcPath.FindChild(str).position;
			mask = 1<<( LayerMask.NameToLayer("shexianjiance"));
		}
	}
//	void Update () 
//	{

//		transform.forward = Vector3.Normalize(m_NpcPathPoint[m_NpcPathNum+1] - m_NpcPathPoint[m_NpcPathNum]);
//		if(Physics.Raycast(transform.position+Vector3.up*25.0f,-Vector3.up,out hit,100.0f,mask.value))
//		{
//			transform.position = hit.point + Vector3.up*0.5f;
//		}
//		transform.localEulerAngles = new Vector3(-18.0f,transform.localEulerAngles.y,transform.localEulerAngles.z);
//		transform.forward = Vector3.Lerp(transform.forward,Vector3.Normalize(m_NpcPathPoint[m_NpcPathNum+1] - m_NpcPathPoint[m_NpcPathNum]),90.0f*Time.deltaTime);
//		transform.position += transform.forward*m_NpcSpeed*Time.deltaTime;
//	}
	void FixedUpdate()
	{
		if(m_NpcPathNum == m_NpcPath.childCount-1)
		{
			return;
		}
		if(PlayerController.GetInstance().timmerstar > 5.0f)
		{
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
					m_NpcSpeed = Mathf.Lerp(m_NpcSpeed,m_playerRig.velocity.magnitude*m_SpeedIndex,10.0f*Time.deltaTime);
					if(m_NpcSpeed>m_TopSpeedSet)
					{
						m_NpcSpeed = m_TopSpeedSet;
					}
				}
				if(m_IsJiansu)
				{
					m_NpcSpeed = Mathf.Lerp(m_NpcSpeed,m_playerRig.velocity.magnitude*m_SpeedIndex,10.0f*Time.deltaTime);
				}
				if(m_NpcSpeed<=20f)
				{
					m_NpcSpeed = UnityEngine.Random.Range(20f, 25f);
				}
				if(m_IsEnd)
				{
					m_NpcSpeed = Mathf.Lerp(m_NpcSpeed,m_EndSpeedSet,10.0f*Time.deltaTime);
				}
				transform.position = Vector3.MoveTowards(transform.position,m_NpcPathPoint[m_NpcPathNum+1],m_NpcSpeed*Time.deltaTime);
				transform.forward = Vector3.Lerp( transform.forward,Vector3.Normalize(m_NpcPathPoint[m_NpcPathNum+1] - transform.position),9.0f*Time.deltaTime);
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
					transform.forward = Vector3.Lerp( transform.forward,Vector3.Normalize(m_NpcPathPoint[m_NpcPathNum+1] - transform.position),15.0f*Time.deltaTime);
					transform.localEulerAngles = new Vector3(-10.0f,transform.localEulerAngles.y,transform.localEulerAngles.z);
				}
			}
		}
		else
		{
			m_NpcSpeed = 2.3f;
			transform.position = Vector3.MoveTowards(transform.position,m_NpcPathPoint[m_NpcPathNum+1],m_NpcSpeed*Time.deltaTime);
			transform.forward = Vector3.Lerp( transform.forward,Vector3.Normalize(m_NpcPathPoint[m_NpcPathNum+1] - transform.position),30.0f*Time.deltaTime);
			transform.localEulerAngles = new Vector3(-10.0f,transform.localEulerAngles.y,transform.localEulerAngles.z);
			if(!m_IsPubu && Physics.Raycast(transform.position+Vector3.up*25.0f,-Vector3.up,out hit,100.0f,mask.value))
			{
				transform.position = hit.point + Vector3.up*0.8f;
			}
		}			
	}
//	void LateUpdate()
//	{	
//		transform.forward = Vector3.Normalize(m_NpcPathPoint[m_NpcPathNum+1] - m_NpcPathPoint[m_NpcPathNum]);
//		if(Physics.Raycast(transform.position+Vector3.up*25.0f,-Vector3.up,out hit,100.0f,mask.value))
//		{
//			transform.position = hit.point + Vector3.up*1.0f;
//		}
//		transform.localEulerAngles = new Vector3(-10.0f,transform.localEulerAngles.y,transform.localEulerAngles.z);
//	}
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
        if (other.tag == "pathpoint")
		{
			m_NpcIndex = Convert.ToInt32(other.name);
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
}
