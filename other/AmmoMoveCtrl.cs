using UnityEngine;
using System.Collections;

/// <summary>
/// 子弹运动控制.
/// </summary>
public class AmmoMoveCtrl : MonoBehaviour
{
    /// <summary>
    /// 是否是网络控制端(只有网络控制端才有主动控制权限).
    /// </summary>
    [HideInInspector]
    public bool IsNetControlPort = false;
    /// <summary>
    /// 网络信息同步组件.
    /// </summary>
    public NetworkSynchronizeGame mNetSynGame;
    /// <summary>
    /// 销毁子弹时需要隐藏的对象.
    /// </summary>
    public GameObject[] HiddenObjArray;
    /// <summary>
    /// 网络消息控制器.
    /// </summary>
    NetworkView mNetViewCom;
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
    /// <summary>
    /// 抛物线运动子弹的第一个子集.
    /// </summary>
    Transform AmmoCoreTr;
    void Awake()
    {
        if (transform.childCount > 0)
        {
            AmmoCoreTr = transform.GetChild(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsNetControlPort)
        {
            return;
        }

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
                    CheckAmmoOverlapSphereHit(AmmoCoreTr);
                    break;
                }
            case AmmoType.DiLei:
                {
                    if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
                    {
                        if (AmmoCoreTr != null)
                        {
                            mNetViewCom.RPC("RpcNetSynAmmoCorePosition", RPCMode.Others, AmmoCoreTr.position);
                        }
                    }
                    break;
                }
        }
	}
    
    /// <summary>
    /// 同步子弹实际物体的坐标.
    /// </summary>
    [RPC]
    void RpcNetSynAmmoCorePosition(Vector3 pos)
    {
        if (AmmoCoreTr != null)
        {
            AmmoCoreTr.position = pos;
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

        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                //只允许主控制端调用.
                HandleHiddenAmmo();
                mNetViewCom.RPC("RpcSpawnAmmoExplosionLiZi", RPCMode.Others);
                StartCoroutine(DelayRemoveNetAmmo());
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void HandleHiddenAmmo()
    {
        if (HiddenObjArray != null && HiddenObjArray.Length > 0)
        {
            for (int i = 0; i < HiddenObjArray.Length; i++)
            {
                HiddenObjArray[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 延迟删除网络子弹.
    /// </summary>
    IEnumerator DelayRemoveNetAmmo()
    {
        yield return new WaitForSeconds(1f);
        Network.Destroy(gameObject);
    }

    [RPC]
    void RpcSpawnAmmoExplosionLiZi()
    {
        if (LiZiPrefab != null)
        {
            Instantiate(LiZiPrefab, transform.position, transform.rotation);
        }
        HandleHiddenAmmo();
        CheckAmmoOverlapSphereHit();
    }

    public void InitMoveAmmo(AmmoDt ammoInfo)
    {
        if (transform.childCount > 0)
        {
            AmmoCoreTr = transform.GetChild(0);
        }

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
                    float lobTimePosY = 0.5f * lobTime;
                    GameObject ammoCore = transform.GetChild(0).gameObject;
                    iTween.MoveBy(ammoCore, iTween.Hash("y", lobHeight,
                                                        "time", lobTimePosY,
                                                        "easeType", iTween.EaseType.easeOutQuad));
                    iTween.MoveBy(ammoCore, iTween.Hash("y", -lobHeight,
                                                        "time", lobTimePosY,
                                                        "delay", lobTimePosY,
                                                        "easeType", iTween.EaseType.easeInCubic));
                    
                    Vector3[] posArray = new Vector3[2];
                    posArray[0] = transform.position;
                    posArray[1] = ammoInfo.PosHit;
                    iTween.MoveTo(gameObject, iTween.Hash("path", posArray,
                                                       "time", lobTime,
                                                       "orienttopath", true,
                                                       "easeType", iTween.EaseType.linear,
                                                       "oncomplete", "MoveAmmoOnCompelteITween"));
                    break;
                }
            case AmmoType.DiLei:
                {
                    float lobHeight = ammoInfo.HightVal;
                    float lobTime = Vector3.Distance(transform.position, ammoInfo.PosHit) / AmmoSpeed;
                    float lobTimePosY = 0.5f * lobTime;
                    GameObject ammoCore = transform.GetChild(0).gameObject;
                    iTween.MoveBy(ammoCore, iTween.Hash("y", lobHeight,
                                                        "time", lobTimePosY,
                                                        "easeType", iTween.EaseType.easeOutQuad));
                    iTween.MoveBy(ammoCore, iTween.Hash("y", -lobHeight,
                                                        "time", lobTimePosY,
                                                        "delay", lobTimePosY,
                                                        "easeType", iTween.EaseType.easeInCubic));

                    Vector3[] posArray = new Vector3[2];
                    posArray[0] = transform.position;
                    posArray[1] = ammoInfo.PosHit;
                    iTween.MoveTo(gameObject, iTween.Hash("path", posArray,
                                                       "time", lobTime,
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
                        if (daoJuCom != null && daoJuCom.DaoJuState == DaoJuCtrl.DaoJuType.ZhangAiWu)
                        {
                            //障碍物爆炸.
                            if (PlayerController.GetInstance() != null)
                            {
                                PlayerController.GetInstance().NetSendAmmoHitZhangAiWuInfo(daoJuCom);
                            }
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
                        if (daoJuCom != null && daoJuCom.DaoJuState == DaoJuCtrl.DaoJuType.ZhangAiWu)
                        {
                            //障碍物爆炸.
                            if (PlayerController.GetInstance() != null)
                            {
                                PlayerController.GetInstance().NetSendAmmoHitZhangAiWuInfo(daoJuCom);
                            }
                            daoJuCom.OnDestroyThis();
                        }

                        NpcController npcCom = mAmmoInfo.AimTr.GetComponent<NpcController>();
                        if (npcCom != null)
                        {
                            npcCom.OnDaoDanHit(transform.position);
                        }
                        
                        PlayerController playerCom = mAmmoInfo.AimTr.GetComponent<PlayerController>();
                        if (playerCom != null && IsNetControlPort)
                        {
                            playerCom.OnAmmoHitPlayer();
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
    void CheckAmmoOverlapSphereHit(Transform posTr = null)
    {
        bool isDestroyAmmo = false;
        Vector3 pos = posTr == null ? transform.position : posTr.position;
        Collider[] hits = Physics.OverlapSphere(pos, ShaShangDis, AmmoHitLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            // Don't collide with triggers
            if (hits[i].isTrigger)
            {
                continue;
            }

            DaoJuCtrl daoJuCom = hits[i].GetComponent<DaoJuCtrl>();
			if (daoJuCom != null && daoJuCom.DaoJuState == DaoJuCtrl.DaoJuType.ZhangAiWu)
            {
                if (PlayerController.GetInstance() != null)
                {
                    PlayerController.GetInstance().NetSendAmmoHitZhangAiWuInfo(daoJuCom);
                }
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

            PlayerController playerScript = hits[i].GetComponent<PlayerController>();
            if (playerScript != null && IsNetControlPort)
            {
                playerScript.OnAmmoHitPlayer();
            }
        }
        
        if (AmmoState == AmmoType.DiLei || AmmoState == AmmoType.TankPaoDan)
        {
            //地雷和坦克炮弹强制删除.
            isDestroyAmmo = true;
        }

        if (!IsNetControlPort)
        {
            //非主控制端时,由主控制端来删除网络子弹.
            isDestroyAmmo = false;
        }

        if (isDestroyAmmo)
        {
            OnDestroyThis();
        }
    }
    
    public void SetIsNetControlPort(bool isNetControl)
    {
        IsNetControlPort = isNetControl;
        if (IsNetControlPort)
        {
            mNetViewCom = GetComponent<NetworkView>();
            mNetSynGame.Init(mNetViewCom);
            if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
            {
            }
            else
            {
                mNetViewCom.enabled = false;
            }
        }
    }
}