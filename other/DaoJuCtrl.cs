using UnityEngine;
using System.Collections;

public class DaoJuCtrl : MonoBehaviour
{
    /// <summary>
    /// JinBi 金币
    /// TangGuo 糖果
    /// BaoXiang 宝箱
    /// PenQiJiaSu 喷气加速器
    /// FeiXingYi 飞行翼
    /// JiaSuFengShan 加速风扇
    /// CiTie 磁铁
    /// DaoDan 导弹
    /// ZhangAiWu 障碍物
    /// </summary>
    public enum DaoJuType
    {
        Null,
        JinBi,
        TangGuo,
        BaoXiang,
        PenQiJiaSu,
        FeiXingYi,
        JiaSuFengShan,
        CiTie,
        DaoDan,
        ZhangAiWu,
    }
    public DaoJuType DaoJuState = DaoJuType.Null;
    bool IsDestroyThis = false;
    /// <summary>
    /// 粒子特效预置,需要挂上自销毁脚本.
    /// </summary>
    public GameObject LiZiPrefab;
    /// <summary>
    /// 道具积分.
    /// </summary>
    public int JiFenVal = 0;
    /// <summary>
    /// 道具积分预置,需要挂上自销毁脚本.
    /// </summary>
    public GameObject JiFenPrefab;
    /// <summary>
    /// 导弹要攻击的障碍物对象.
    /// </summary>
    public GameObject ZhangAiWuObj;
    /// <summary>
    /// 道具宝箱预置.
    /// </summary>
    public GameObject BaoXiangPrefab;
    /// <summary>
    /// 是否被玩家的磁铁吸附.
    /// </summary>
    bool IsMoveToPlayerByCiTie = false;
    void Update()
    {
        if (IsMoveToPlayerByCiTie)
        {
            Transform playerTr = PlayerController.GetInstance().transform;
            transform.position = Vector3.Lerp(transform.position, playerTr.position, Time.deltaTime * 10f);
            if (Vector3.Distance(transform.position, playerTr.position) <= 1.5f)
            {
                OnDestroyThis();
            }
            return;
        }

        switch (DaoJuState)
        {
            case DaoJuType.JinBi:
            case DaoJuType.TangGuo:
            case DaoJuType.BaoXiang:
                {
                    if (PlayerController.GetInstance().IsOpenCiTieDaoJu)
                    {
                        Transform playerTr = PlayerController.GetInstance().transform;
                        if (!IsMoveToPlayerByCiTie && Vector3.Distance(playerTr.position, transform.position) <= 10f)
                        {
                            IsMoveToPlayerByCiTie = true;
                        }
                    }
                    break;
                }

        }
    }

    public float TimeDestroyZhangAiWu = 0.4f;
    public void OnDestroyThis()
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

        if (PlayerController.GetInstance() != null)
        {
            if (JiFenVal > 0)
            {
                PlayerController.GetInstance().PlayerJiFen += JiFenVal;
                PlayerController.GetInstance().m_UIController.ShowJiFenInfo(PlayerController.GetInstance().PlayerJiFen);
            }

            if (JiFenPrefab != null)
            {
                PlayerController.GetInstance().SpawnDaoJuJiFen(JiFenPrefab);
            }
        }

        switch (DaoJuState)
        {
            case DaoJuType.PenQiJiaSu:
            case DaoJuType.FeiXingYi:
            case DaoJuType.JiaSuFengShan:
                {
                    PlayerController.GetInstance().OpenPlayerDaoJuAni(DaoJuState);
                    break;
                }
            case DaoJuType.ZhangAiWu:
                {
                    GameObject childObj = null;
                    DestroyThisTimed destroyCom = null;
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        childObj = transform.GetChild(i).gameObject;
                        destroyCom = childObj.AddComponent<DestroyThisTimed>();
                        destroyCom.InitInfo(LiZiPrefab, BaoXiangPrefab, i * 0.4f);
                    }
                    transform.DetachChildren(); //将子集从自身解除.
                    break;
                }
            case DaoJuType.CiTie:
                {
                    PlayerController.GetInstance().OpenPlayerCiTieDaoJu();
                    break;
                }
            case DaoJuType.DaoDan:
                {
                    PlayerController.GetInstance().OnPlayerHitDaoDanDaoJu(ZhangAiWuObj);
                    break;
                }
        }
        Destroy(gameObject);
    }
}