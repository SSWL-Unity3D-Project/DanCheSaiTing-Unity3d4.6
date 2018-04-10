using System.Collections.Generic;
using UnityEngine;

public class AiNpcDataManage : MonoBehaviour
{
    /// <summary>
    /// Player列表.
    /// </summary>
    public List<Transform> PlayerTrList = new List<Transform>();
    /// <summary>
    /// AiNpc和Player列表.
    /// </summary>
    public List<Transform> AiNpcTrList = new List<Transform>();
    /// <summary>
    /// 添加AiNpc和Player.
    /// </summary>
    public void AddAiNpcTr(Transform tr)
    {
        if (!AiNpcTrList.Contains(tr))
        {
            AiNpcTrList.Add(tr);
        }
    }

    /// <summary>
    /// 添加AiNpc和Player.
    /// </summary>
    public void AddPlayerTr(Transform tr)
    {
        if (!PlayerTrList.Contains(tr))
        {
            PlayerTrList.Add(tr);
        }
    }

    /// <summary>
    /// 玩家查找可攻击对象.
    /// </summary>
    public GameObject FindAiNpc(Transform tr, float disValMax, float disValMin = 3f)
    {
        GameObject obj = null;
        float disVal = 0f;
        float disValTmp = 10000f;
        for (int i = 0; i < AiNpcTrList.Count; i++)
        {
            if (AiNpcTrList.Count > i && AiNpcTrList[i] != null && tr != AiNpcTrList[i])
            {
                disVal = Vector3.Distance(tr.position, AiNpcTrList[i].position);
                if (disVal < disValMax && disVal > disValMin && Vector3.Dot(AiNpcTrList[i].forward, AiNpcTrList[i].position - tr.position) > 0f)
                {
                    if (disValTmp >= disVal)
                    {
                        //保留距离玩家最近的可攻击对象.
                        disValTmp = disVal;
                        obj = AiNpcTrList[i].gameObject;
                    }
                }
            }
        }
        return obj;
    }

    /// <summary>
    /// Npc查找可攻击对象.
    /// </summary>
    public GameObject FindPlayer(Transform tr, float disValMax, float disValMin)
    {
        GameObject obj = null;
        float disVal = 0f;
        float disValTmp = 10000f;
        for (int i = 0; i < PlayerTrList.Count; i++)
        {
            if (PlayerTrList.Count > i && PlayerTrList[i] != null && tr != PlayerTrList[i])
            {
                disVal = Vector3.Distance(tr.position, PlayerTrList[i].position);
                if (disVal < disValMax && disVal > disValMin && Vector3.Dot(tr.forward, PlayerTrList[i].position - tr.position) > 0f)
                {
                    if (disValTmp >= disVal)
                    {
                        //保留距离npc最近的可攻击对象.
                        disValTmp = disVal;
                        obj = PlayerTrList[i].gameObject;
                    }
                }
            }
        }
        return obj;
    }
}