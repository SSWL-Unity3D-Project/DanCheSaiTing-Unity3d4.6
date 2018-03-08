using System.Collections.Generic;
using UnityEngine;

public class DaoJuZhangAiWuData : MonoBehaviour
{
    public List<Transform> ZhangAiWuTrList = new List<Transform>();
    /// <summary>
    /// 添加障碍物.
    /// </summary>
    public int AddZhangAiWuTr(Transform tr)
    {
        if (!ZhangAiWuTrList.Contains(tr))
        {
            ZhangAiWuTrList.Add(tr);
            return ZhangAiWuTrList.Count - 1;
        }
        return -1;
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

    /// <summary>
    /// 通过障碍物与tr的距离来查找障碍.
    /// </summary>
    public GameObject FindZhangAiWu(Transform tr)
    {
        GameObject obj = null;
        float disVal = 0f;
        for (int i = 0; i < ZhangAiWuTrList.Count; i++)
        {
            if (ZhangAiWuTrList.Count > i && ZhangAiWuTrList[i] != null && tr != null)
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

    /// <summary>
    /// 通过障碍物的列表id来查找障碍.
    /// </summary>
    public GameObject FindZhangAiWu(int index)
    {
        if (index < ZhangAiWuTrList.Count)
        {
            if (ZhangAiWuTrList[index] != null)
            {
                return ZhangAiWuTrList[index].gameObject;
            }
        }
        return null;
    }
}