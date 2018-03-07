using UnityEngine;
using System.Collections;

public class ResetWaterSpeed : MonoBehaviour
{
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
                playerScript.m_LimitSpeed = 0.0f;
            }
		}
	}
}
