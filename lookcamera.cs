using UnityEngine;

public class lookcamera : MonoBehaviour 
{
	private Transform Mycamera;
	void Start () 
	{
        Mycamera = PlayerController.GetInstance().m_CameraShake.transform;
	}

	void Update () 
	{
        float dis = Vector3.Distance(transform.position, Mycamera.position);
        if (dis < 300f && dis > 3f)
		{
            Vector3 forwardVal = Vector3.Normalize(Mycamera.position - transform.position);
            if (Vector3.Dot(forwardVal, transform.forward) <= Mathf.Cos(Mathf.PI / 40f))
            {
                transform.LookAt(Mycamera);
            }
		}
	}
}
