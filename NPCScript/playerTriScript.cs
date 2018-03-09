using UnityEngine;

public class playerTriScript : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
    {
        TriggerYanHua yanHua = other.GetComponent<TriggerYanHua>();
        if (yanHua != null)
        {
            yanHua.InitSpawnYanHua();
        }

        if (other.tag == "spawnNPCTri")
		{
			other.transform.collider.enabled = false;
			other.transform.GetComponent<spawnTriggerScript>().BeginSpawn();
			Destroy(other.gameObject);
		}
		else if (other.tag == "deleteNPCTri")
		{
			other.transform.collider.enabled = false;
			other.transform.GetComponent<deleteTriggerScript>().BeginDelete();
			Destroy(other.gameObject);
		}
		else if (other.tag == "openLiziTri")
		{
			other.transform.collider.enabled = false;
			other.transform.GetComponent<liziOpenTrigger>().openLizi();
			Destroy(other.gameObject);
		}
		else if (other.tag == "closeLiziTri")
		{
			other.transform.collider.enabled = false;
			other.transform.GetComponent<liziCloseTrigger>().closeLizi();
			Destroy(other.gameObject);
		}
	}
}
