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
        /// <summary>
        /// 抛物线的高度值.
        /// </summary>
        public float HightVal;
    }

    /// <summary>
    /// PuTong 普通子弹.
    /// GenZongDan 跟踪子弹.
    /// YuLei 鱼雷子弹.
    /// TankPaoDan 坦克炮弹.
    /// DiLei 地雷.
    /// </summary>
    public enum AmmoType
    {
        PuTong,
        GenZongDan,
        YuLei,
        TankPaoDan,
        DiLei,
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
                    CheckAmmoOverlapSphereHit();
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
            case AmmoType.TankPaoDan:
                {
                    float lobHeight = ammoInfo.HightVal;
                    float lobTime = Vector3.Distance(transform.position, ammoInfo.PosHit) / AmmoSpeed;
                    GameObject ammoCore = transform.GetChild(0).gameObject;
                    iTween.MoveBy(ammoCore, iTween.Hash("y", lobHeight,
                                                        "time", lobTime / 2,
                                                        "easeType", iTween.EaseType.easeOutQuad));
                    iTween.MoveBy(ammoCore, iTween.Hash("y", -lobHeight,
                                                        "time", lobTime / 2,
                                                        "delay", lobTime / 2,
                                                        "easeType", iTween.EaseType.easeInCubic));
                    
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
            case AmmoType.DiLei:
                {
                    float lobHeight = ammoInfo.HightVal;
                    float lobTime = Vector3.Distance(transform.position, ammoInfo.PosHit) / AmmoSpeed;
                    GameObject ammoCore = transform.GetChild(0).gameObject;
                    iTween.MoveBy(ammoCore, iTween.Hash("y", lobHeight,
                                                        "time", lobTime / 2,
                                                        "easeType", iTween.EaseType.easeOutQuad));
                    iTween.MoveBy(ammoCore, iTween.Hash("y", -lobHeight,
                                                        "time", lobTime / 2,
                                                        "delay", lobTime / 2,
                                                        "easeType", iTween.EaseType.easeInCubic));

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
        switch (AmmoState)
        {
            case AmmoType.PuTong:
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
                    break;
                }
            case AmmoType.TankPaoDan:
                {
                    CheckAmmoOverlapSphereHit();
                    break;
                }
            case AmmoType.DiLei:
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
                    else
                    {
                        CheckAmmoOverlapSphereHit();
                    }
                    break;
                }
        }
        OnDestroyThis();
    }


    /// <summary>
    /// 检测子弹的范围碰撞.
    /// </summary>
    void CheckAmmoOverlapSphereHit()
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