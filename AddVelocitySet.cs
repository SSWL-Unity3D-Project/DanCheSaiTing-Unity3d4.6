using UnityEngine;
using System.Collections;

public class AddVelocitySet : MonoBehaviour
{
	public float m_TopSpeedSet = 0.0f;
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "player")
		{
            if (PlayerController.GetInstance().mDaoJuState == DaoJuCtrl.DaoJuType.PenQiJiaSu
                || PlayerController.GetInstance().mDaoJuState == DaoJuCtrl.DaoJuType.FeiXingYi
                || PlayerController.GetInstance().mDaoJuState == DaoJuCtrl.DaoJuType.JiaSuFengShan)
            {
            }
            else
            {
                PlayerController.m_pTopSpeed = m_TopSpeedSet;
            }
		}
	}
}
