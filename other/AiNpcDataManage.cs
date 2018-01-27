using System.Collections.Generic;
using UnityEngine;

public class AiNpcDataManage : MonoBehaviour
{
    public List<Transform> AiNpcTrList = new List<Transform>();
    /// <summary>
    /// 添加障碍物.
    /// </summary>
    public void AddAiNpcTr(Transform tr)
    {
        if (!AiNpcTrList.Contains(tr))
        {
            AiNpcTrList.Add(tr);
        }
    }
    
    public GameObject FindAiNpc(Transform tr, float disValMax)
    {
        GameObject obj = null;
        float disVal = 0f;
        for (int i = 0; i < AiNpcTrList.Count; i++)
        {
            if (AiNpcTrList.Count > i && AiNpcTrList[i] != null)
            {
                disVal = Vector3.Distance(tr.position, AiNpcTrList[i].position);
                if (disVal < disValMax && disVal > 3f && Vector3.Dot(AiNpcTrList[i].forward, AiNpcTrList[i].position - tr.position) > 0f)
                {
                    obj = AiNpcTrList[i].gameObject;
                    break;
                }
            }
        }
        return obj;
    }
}