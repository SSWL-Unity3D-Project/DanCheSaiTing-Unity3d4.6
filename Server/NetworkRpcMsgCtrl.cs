using UnityEngine;

/// <summary>
/// ѭ����������ͨ��.
/// </summary>
public class NetworkRpcMsgCtrl : MonoBehaviour
{
	NetworkView mNetViewCom;
	public void Init()
    {
		gameObject.name = "_NetworkRpcMsgCtrl";
		mNetViewCom = GetComponent<NetworkView>();
	}

    void Awake()
    {
        NetworkRootMovie.GetInstance().mNetworkRpcMsgScript = this;
    }

	public void RemoveSelf()
    {
        if (gameObject == null)
        {
            return;
        }

		Debug.Log("NetworkRpcMsgCtrl::RemoveSelf...");
        if (Network.peerType == NetworkPeerType.Client)
        {
            Destroy(gameObject);
        }
	}

    /// <summary>
    /// ���ͽ�Ҫ���ص���Ϸ������Ϣ.
    /// </summary>
    public void NetSendLoadLevel(int levelVal)
    {
        if (Network.peerType == NetworkPeerType.Server)
        {
            mNetViewCom.RPC("RpcSendLoadLevelMsg", RPCMode.AllBuffered, levelVal);
        }
    }

    [RPC]
    void RpcSendLoadLevelMsg(int levelVal)
    {
        NetworkEvent.GetInstance().OnRpcSendLoadLevelMsgTrigger(levelVal);
    }

    /// <summary>
    /// ���Ͳ�����ҵ�������Ϣ.
    /// </summary>
    public void NetSetSpawnPlayerIndex(NetworkPlayer playerNet, int index)
	{
		if (Network.peerType == NetworkPeerType.Server)
        {
			mNetViewCom.RPC("RpcSendSpawnPlayerIndex", RPCMode.Others, playerNet, index);
		}
	}
	
	[RPC]
	void RpcSendSpawnPlayerIndex(NetworkPlayer playerNet, int index)
	{
		if (playerNet != Network.player)
        {
			return;
		}
		NetworkServerNet.GetInstance().SetIndexSpawnPlayer(index);
	}
}