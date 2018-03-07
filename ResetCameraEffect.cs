using UnityEngine;
using System.Collections;

public class ResetCameraEffect : MonoBehaviour {
	public float m_ParameterForEffect = 1.0f;
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "player")
        {
            PlayerController playerScript = other.GetComponent<PlayerController>();
            if (playerScript != null && playerScript.IsNetControlPort)
            {
                if (playerScript.mSpeedDaoJuState == DaoJuCtrl.DaoJuType.PenQiJiaSu
                || playerScript.mSpeedDaoJuState == DaoJuCtrl.DaoJuType.FeiXingYi
                || playerScript.mSpeedDaoJuState == DaoJuCtrl.DaoJuType.ShuangYiFeiJi
                || playerScript.mSpeedDaoJuState == DaoJuCtrl.DaoJuType.QianTing
                || playerScript.mSpeedDaoJuState == DaoJuCtrl.DaoJuType.Tank
                || playerScript.mSpeedDaoJuState == DaoJuCtrl.DaoJuType.JiaSuFengShan)
                {
                }
                else
                {
                    playerScript.m_ParameterForEfferct = m_ParameterForEffect;
                }
            }
		}
	}
}
