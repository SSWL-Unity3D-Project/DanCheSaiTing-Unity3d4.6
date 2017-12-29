using UnityEngine;
using System.Collections;

public class lookcamera : MonoBehaviour 
{
	private Transform Mycamera;
	void Start () 
	{
        Mycamera = PlayerController.GetInstance().m_CameraShake.transform;
        //Mycamera = GameObject.Find ("Main Camera").transform;
	}

	void Update () 
	{
		if (Vector3.Distance(transform.position, Mycamera.position) < 300f)
		{
			transform.LookAt (Mycamera.position);
		}
		//transform.eulerAngles = new Vector3( transform.eulerAngles.x, transform.eulerAngles.z, transform.eulerAngles.y);
		//transform.localEulerAngles = new Vector3( transform.localEulerAngles.x, transform.localEulerAngles.z, transform.localEulerAngles.y);
		//transform.forward = Mycamera.position - transform.position;
	}
}
