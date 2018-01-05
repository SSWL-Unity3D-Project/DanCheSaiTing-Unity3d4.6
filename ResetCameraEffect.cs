using UnityEngine;
using System.Collections;

public class ResetCameraEffect : MonoBehaviour {
	public float m_ParameterForEffect = 1.0f;
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "player")
        {
            if (PlayerController.GetInstance().mSpeedDaoJuState == DaoJuCtrl.DaoJuType.PenQiJiaSu
                || PlayerController.GetInstance().mSpeedDaoJuState == DaoJuCtrl.DaoJuType.FeiXingYi
                || PlayerController.GetInstance().mSpeedDaoJuState == DaoJuCtrl.DaoJuType.ShuangYiFeiJi
                || PlayerController.GetInstance().mSpeedDaoJuState == DaoJuCtrl.DaoJuType.QianTing
                || PlayerController.GetInstance().mSpeedDaoJuState == DaoJuCtrl.DaoJuType.Tank
                || PlayerController.GetInstance().mSpeedDaoJuState == DaoJuCtrl.DaoJuType.JiaSuFengShan)
            {
            }
            else
            {
                PlayerController.GetInstance().m_ParameterForEfferct = m_ParameterForEffect;
            }
		}
	}
}
