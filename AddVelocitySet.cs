using UnityEngine;
using System.Collections;

public class AddVelocitySet : MonoBehaviour
{
	public float m_TopSpeedSet = 0.0f;
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "player")
		{
            if (PlayerController.GetInstance().mSpeedDaoJuState == DaoJuCtrl.DaoJuType.PenQiJiaSu
                || PlayerController.GetInstance().mSpeedDaoJuState == DaoJuCtrl.DaoJuType.FeiXingYi
                || PlayerController.GetInstance().mSpeedDaoJuState == DaoJuCtrl.DaoJuType.JiaSuFengShan)
            {
            }
            else
            {
                PlayerController.m_pTopSpeed = m_TopSpeedSet;
            }
		}
	}
}
