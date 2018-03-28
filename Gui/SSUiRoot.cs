using UnityEngine;

/// <summary>
/// UI脚本总父级,完成通用函数的封装.
/// </summary>
public class SSUiRoot : MonoBehaviour
{
    /// <summary>
    /// 产生UI预制.
    /// </summary>
    public Object Instantiate(GameObject prefab, Transform parent)
    {
        GameObject obj = (GameObject)Instantiate(prefab);
        if (parent != null)
        {
            obj.transform.SetParent(parent);
            obj.transform.localScale = prefab.transform.localScale;
            obj.transform.localPosition = prefab.transform.localPosition;
        }
        return obj;
    }
}