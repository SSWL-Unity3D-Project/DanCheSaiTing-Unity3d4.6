using UnityEngine;

/// <summary>
/// 游戏控制脚本,始终不销毁.
/// </summary>
public class SSGameCtrl : MonoBehaviour
{
    /// <summary>
    /// 游戏场景总控制脚本.
    /// </summary>
    public SSGameRoot mSSGameRoot;
    static SSGameCtrl _Instance;
    public static SSGameCtrl GetInstance()
    {
        if (_Instance == null)
        {
            GameObject obj = new GameObject("_SSGameCtrl");
            _Instance = obj.AddComponent<SSGameCtrl>();
            DontDestroyOnLoad(obj);
        }
        return _Instance;
    }
}