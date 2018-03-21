﻿using UnityEngine;
using System.Collections;

public class TimeScaleSet : MonoBehaviour 
{
	public bool m_IsTimeScaleSet = true;
	public bool m_IsCameraEffect = true;
	public float TimeScaleset = 1.0f;
	public float m_CameraEffectSet = 1.0f;
	public RadialBlur m_CameraEffect;
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "player")
		{
			PlayerControllerForMoiew aiPlayerScript = other.GetComponent<PlayerControllerForMoiew>();
			if (aiPlayerScript != null)
			{
				if (m_IsTimeScaleSet)
				{
					Time.timeScale = TimeScaleset;
				}
				
				if (m_IsCameraEffect)
				{
					m_CameraEffect.SampleStrength = m_CameraEffectSet;
				}
			}

            PlayerController playerScript = other.GetComponent<PlayerController>();
            if (playerScript != null && playerScript.IsNetControlPort)
            {
                if (m_IsTimeScaleSet)
                {
                    Time.timeScale = TimeScaleset;
                }

                if (m_IsCameraEffect)
                {
                    m_CameraEffect.SampleStrength = m_CameraEffectSet;
                }
            }
		}
	}
}
