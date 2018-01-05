using UnityEngine;
using System.Collections;

/// <summary>
/// 子弹运动控制.
/// </summary>
public class AmmoMoveCtrl : MonoBehaviour
{
    /// <summary>
    /// 子弹数据信息.
    /// </summary>
    public class AmmoDt
    {
        public AmmoType AmmoState = AmmoType.PuTong;
        /// <summary>
        /// 子弹要击中的目标.
        /// </summary>
        public Transform AimTr;
        /// <summary>
        /// 子弹击中的坐标.
        /// </summary>
        public Vector3 PosHit;
    }

    /// <summary>
    /// PuTong 普通子弹.
    /// GenZongDan 跟踪子弹.
    /// YuLei 鱼雷子弹.
    /// </summary>
    public enum AmmoType
    {
        PuTong,
        GenZongDan,
        YuLei,
    }
    AmmoType AmmoState = AmmoType.PuTong;
    /// <summary>
    /// 跟踪弹跟随的目标.
    /// </summary>
    Transform GenZongTr;
    /// <summary>
    /// 子弹速度.
    /// </summary>
    public float AmmoSpeed;
    /// <summary>
    /// 子弹杀伤范围.
    /// </summary>
    public float ShaShangDis;
    /// <summary>
    /// 子弹检测碰撞的信息.
    /// </summary>
    public LayerMask AmmoHitLayer;
    bool IsDestroyThis = false;
    /// <summary>
    /// 子弹爆炸粒子特效,需要挂上自销毁脚本.
    /// </summary>
    public GameObject LiZiPrefab;
    AmmoDt mAmmoInfo;
    
    // Update is called once per frame
    void Update()
    {
        switch (AmmoState)
        {
            case AmmoType.GenZongDan:
                {
                    float mvDis = AmmoSpeed * Time.deltaTime;
                    if (Vector3.Distance(GenZongTr.position, transform.position) <= mvDis)
                    {
                        //子弹击中目标.
                        NpcController npcCom = GenZongTr.GetComponent<NpcController>();
                        if (npcCom != null)
                        {
                            npcCom.OnDaoDanHit(transform.position);
                        }
                        OnDestroyThis();
                        return;
                    }
                    Vector3 vecForward = Vector3.Normalize(GenZongTr.position - transform.position);
                    transform.position += vecForward * mvDis;
                    transform.forward = Vector3.Lerp(transform.forward, vecForward, 15f * Time.deltaTime);
                    break;
                }
            case AmmoType.YuLei:
                {
                    CheckYuLeiAmmoOverlapSphereHit();
                    break;
                }
        }
	}

    void OnDestroyThis()
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
        Destroy(gameObject);
    }

    public void InitMoveAmmo(AmmoDt ammoInfo)
    {
        mAmmoInfo = ammoInfo;
        switch (ammoInfo.AmmoState)
        {
            case AmmoType.PuTong:
                {
                    Vector3[] posArray = new Vector3[2];
                    posArray[0] = transform.position;
                    posArray[1] = ammoInfo.PosHit;
                    iTween.MoveTo(gameObject, iTween.Hash("path", posArray,
                                                       "speed", AmmoSpeed,
                                                       "orienttopath", true,
                                                       "easeType", iTween.EaseType.linear,
                                                       "oncomplete", "MoveAmmoOnCompelteITween"));
                    break;
                }
            case AmmoType.GenZongDan:
                {
                    GenZongTr = ammoInfo.AimTr;
                    break;
                }
            case AmmoType.YuLei:
                {
                    Vector3[] posArray = new Vector3[2];
                    posArray[0] = transform.position;
                    posArray[1] = ammoInfo.PosHit;
                    iTween.MoveTo(gameObject, iTween.Hash("path", posArray,
                                                       "speed", AmmoSpeed,
                                                       "orienttopath", true,
                                                       "easeType", iTween.EaseType.linear,
                                                       "oncomplete", "MoveAmmoOnCompelteITween"));
                    break;
                }
        }
        AmmoState = ammoInfo.AmmoState;
    }

    void MoveAmmoOnCompelteITween()
    {
        if (AmmoType.PuTong == AmmoState)
        {
            if (mAmmoInfo.AimTr != null)
            {
                DaoJuCtrl daoJuCom = mAmmoInfo.AimTr.GetComponent<DaoJuCtrl>();
                if (daoJuCom.DaoJuState == DaoJuCtrl.DaoJuType.ZhangAiWu)
                {
                    //障碍物爆炸. 
                    daoJuCom.OnDestroyThis();
                }
            }
        }
        OnDestroyThis();
    }


    /// <summary>
    /// 检测鱼雷子弹的碰撞.
    /// </summary>
    void CheckYuLeiAmmoOverlapSphereHit()
    {
        bool isDestroyAmmo = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, ShaShangDis, AmmoHitLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            // Don't collide with triggers
            if (hits[i].isTrigger)
            {
                continue;
            }

            DaoJuCtrl daoJuCom = hits[i].GetComponent<DaoJuCtrl>();
            if (daoJuCom.DaoJuState == DaoJuCtrl.DaoJuType.ZhangAiWu)
            {
                daoJuCom.OnDestroyThis();
                isDestroyAmmo = true;
            }

            NpcController npcCom = hits[i].GetComponent<NpcController>();
            if (npcCom != null)
            {
                npcCom.OnDaoDanHit(transform.position);
                isDestroyAmmo = true;
            }

            npcScript npcSp = hits[i].GetComponent<npcScript>();
            if (npcSp != null)
            {
                npcSp.OnDestroyThis();
                isDestroyAmmo = true;
            }
        }

        if (isDestroyAmmo)
        {
            OnDestroyThis();
        }
    }
}