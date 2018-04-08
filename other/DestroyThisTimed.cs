using UnityEngine;
using System.Collections;

public class DestroyThisTimed : MonoBehaviour
{
	[Range(-1f, 100f)] public float TimeRemove = 5f;
    /// <summary>
    /// 爆炸粒子预置.
    /// </summary>
    GameObject LiZiPrefab;
    /// <summary>
    /// 道具宝箱预置.
    /// </summary>
    GameObject BaoXiangPrefab;
    bool IsDestroyThis = false;
    // Use this for initialization
    void Start()
	{
		//Debug.Log("DestroyThisTimed -> objName "+gameObject.name);
		//Destroy(gameObject, TimeRemove);
        //Invoke("DelayDestroyThis", TimeRemove);
        if (TimeRemove > 0f)
        {
            SSTimeUpCtrl timeUpCom = gameObject.AddComponent<SSTimeUpCtrl>();
            timeUpCom.Init(TimeRemove);
            timeUpCom.OnTimeUpOverEvent += OnTimeUpOverEvent;
        }
    }

    void OnTimeUpOverEvent()
    {
        DelayDestroyThis();
    }

    public void InitInfo(GameObject liZi, GameObject baoXiang, float timeVal)
    {
        LiZiPrefab = liZi;
        BaoXiangPrefab = baoXiang;
        TimeRemove = timeVal;
    }

    /// <summary>
    /// 动画删除对象事件触发.
    /// </summary>
    public void OnAniTriggerRemoveThis()
    {
        DelayDestroyThis();
    }

    void DelayDestroyThis()
    {
        if (IsDestroyThis)
        {
            return;
        }
        IsDestroyThis = true;

        if (LiZiPrefab != null)
        {
            Instantiate(LiZiPrefab, transform.position, transform.rotation);
        }

        if (BaoXiangPrefab != null)
        {
            Instantiate(BaoXiangPrefab, transform.position, transform.rotation);
        }

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
