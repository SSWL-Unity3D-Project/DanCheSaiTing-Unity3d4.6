using UnityEngine;

/// <summary>
/// ��Ϸ���ƽű�,ʼ�ղ�����.
/// </summary>
public class SSGameCtrl : MonoBehaviour
{
    /// <summary>
    /// ��Ϸ�����ܿ��ƽű�.
    /// </summary>
    [HideInInspector]
    public SSGameRoot mSSGameRoot;
    /// <summary>
    /// ���ѡ�����Ϸģʽ.
    /// </summary>
    [HideInInspector]
    public NetworkRootMovie.GameMode eGameMode = NetworkRootMovie.GameMode.Null;
    /// <summary>
    /// ��Ҕ�������.
    /// </summary>
    [HideInInspector]
    public PlayerDataManage mPlayerDataManage;
    static SSGameCtrl _Instance;
    public static SSGameCtrl GetInstance()
    {
        if (_Instance == null)
        {
            GameObject obj = new GameObject("_SSGameCtrl");
            _Instance = obj.AddComponent<SSGameCtrl>();
            _Instance.mPlayerDataManage = new PlayerDataManage();
            DontDestroyOnLoad(obj);
        }
        return _Instance;
    }
}