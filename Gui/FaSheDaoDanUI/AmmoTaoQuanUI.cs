using UnityEngine;

public class AmmoTaoQuanUI : MonoBehaviour
{
    /// <summary>
    /// 提示圈是否为3d.
    /// </summary>
    public bool IsTiShiQuan3D = false;
    /// <summary>
    /// 瞄准对象.
    /// </summary>
    [HideInInspector]
    public Transform mAimTr;
    /// <summary>
    /// 产生套圈瞄准UI的玩家.
    /// </summary>
    Transform mPlayerTr;
    float mDisMinVal = 0f;
    float mDisMaxVal = 0f;
    bool IsDestroyThis = false;
    bool IsInit = false;
    public void Init(Transform trAim, Transform trPlayer, float minDis, float maxDis)
    {
        mAimTr = trAim;
        mPlayerTr = trPlayer;
        mDisMinVal = minDis;
        mDisMaxVal = maxDis;
        IsInit = true;
        if (IsTiShiQuan3D)
        {
            transform.parent = mAimTr;
            NpcController npcCtrl = trAim.GetComponent<NpcController>();
            if (npcCtrl != null && npcCtrl.TiShiQuanTr != null)
            {
                transform.parent = npcCtrl.TiShiQuanTr;
            }
            transform.localPosition = Vector3.zero;
        }
    }

    public void DestroySelf()
    {
        if (!IsDestroyThis)
        {
            IsDestroyThis = true;
            Destroy(gameObject);
        }
    }
    
	// Update is called once per frame
	void Update ()
    {
        if (!IsInit || IsDestroyThis || Camera.main == null)
        {
            return;
        }

        float disVal = Vector3.Distance(mAimTr.position, mPlayerTr.position);
        if (disVal < mDisMinVal || disVal > mDisMaxVal)
        {
            DestroySelf();
            return;
        }

        if (!IsTiShiQuan3D)
        {
            Vector3 aimPos = Camera.main.WorldToScreenPoint(mAimTr.position);
            aimPos.z = 500f;
            aimPos.x = (1360f * aimPos.x) / Screen.width;
            aimPos.y = (768f * aimPos.y) / Screen.height;
            transform.localPosition = aimPos;
        }
    }
}