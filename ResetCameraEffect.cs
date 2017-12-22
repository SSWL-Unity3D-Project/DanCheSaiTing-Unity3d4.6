using UnityEngine;
using System.Collections;

public class ResetCameraEffect : MonoBehaviour {
	public float m_ParameterForEffect = 1.0f;
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
                PlayerController.GetInstance().m_ParameterForEfferct = m_ParameterForEffect;
            }
		}
	}
}
