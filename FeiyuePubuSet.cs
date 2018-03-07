using UnityEngine;
using System.Collections;

public class FeiyuePubuSet : MonoBehaviour
{
	public float m_PubuPower = 0.0f;
	void Start () 
	{
	
	}
	void Update () 
	{

	}
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "player")
		{
            PlayerController playerScript = other.GetComponent<PlayerController>();
            if (playerScript != null && playerScript.IsNetControlPort)
            {
                playerScript.m_IsPubu = true;
                playerScript.m_PubuPower = m_PubuPower;
            }
		}
	}
}
