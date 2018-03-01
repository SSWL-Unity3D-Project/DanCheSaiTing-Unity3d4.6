using UnityEngine;

/// <summary>
/// ��Ϸ���ƽű�,ʼ�ղ�����.
/// </summary>
public class SSGameCtrl : MonoBehaviour
{
    /// <summary>
    /// ��Ϸ�����ܿ��ƽű�.
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