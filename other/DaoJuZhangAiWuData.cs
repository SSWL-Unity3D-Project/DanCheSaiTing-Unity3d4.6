using System.Collections.Generic;
using UnityEngine;

public class DaoJuZhangAiWuData : MonoBehaviour
{
    public List<Transform> ZhangAiWuTrList = new List<Transform>();
    /// <summary>
    /// 添加障碍物.
    /// </summary>
    public void AddZhangAiWuTr(Transform tr)
    {
        if (!ZhangAiWuTrList.Contains(tr))
        {
            ZhangAiWuTrList.Add(tr);
        }
    }

    /// <summary>
    /// 移除障碍物.
    /// </summary>
    public void RemoveZhangAiWuTr(Transform tr)
    {
        if (ZhangAiWuTrList.Contains(tr))
        {
            ZhangAiWuTrList.Remove(tr);
        }
    }

    public GameObject FindZhangAiWu(Transform tr)
    {
        GameObject obj = null;
        float disVal = 0f;
        for (int i = 0; i < ZhangAiWuTrList.Count; i++)
        {
            if (ZhangAiWuTrList.Count > i && ZhangAiWuTrList[i] != null)
            {
                disVal = Vector3.Distance(tr.position, ZhangAiWuTrList[i].position);
                if (disVal < 120f && disVal > 5f)
                {
                    obj = ZhangAiWuTrList[i].gameObject;
                    break;
                }
            }
        }
        return obj;
    }
}