using UnityEngine;

/// <summary>
/// 产生循环动画网络通信.
/// </summary>
public class NetworkRpcMsgSpawn : MonoBehaviour
{
    public GameObject NetworkRpcObjPrefab;
    /// <summary>
    /// 创建NetworkRpcMsgCtrl组件(只在游戏的循环动画里进行).
    /// </summary>
    public void CreateNetworkRpc()
    {
        if (NetworkRpcMsgCtrl.GetInstance() != null)
        {
            return;
        }

        if (NetworkRpcObjPrefab == null)
        {
            return;
        }

        //GameObject obj = (GameObject)Network.Instantiate(NetworkRpcObjPrefab, Vector3.zero, Quaternion.identity, GlobalData.NetWorkGroup);
        //NetworkRpcMsgCtrl netRpc = obj.GetComponent<NetworkRpcMsgCtrl>();
        //netRpc.Init();
    }
}