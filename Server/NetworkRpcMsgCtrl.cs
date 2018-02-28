using UnityEngine;

/// <summary>
/// 循环动画网络通信.
/// </summary>
public class NetworkRpcMsgCtrl : MonoBehaviour
{
	NetworkView mNetViewCom;
	public static int MaxLinkServerCount = 100;
	public static int NoLinkClientCount = 10;

	static NetworkRpcMsgCtrl _Instance;
	public static NetworkRpcMsgCtrl GetInstance()
	{
		return _Instance;
	}

	public void Init()
    {
		if (_Instance != null)
        {
			_Instance.RemoveSelf();
		}

		_Instance = this;
		DontDestroyOnLoad(gameObject);
		gameObject.name = "_NetworkRpcMsgCtrl";
		mNetViewCom = GetComponent<NetworkView>();
	}

	public void RemoveSelf()
	{
		if (gameObject == null)
        {
			return;
		}
		_Instance = null;
		Network.Destroy(gameObject);
		Debug.Log("NetworkRpcMsgCtrl::RemoveSelf...");
	}

	public void SendLoadLevel(int levelVal)
	{
		//if (levelVal == (int)GameLeve.None || levelVal == (int)GameLeve.SetPanel || levelVal == (int)GameLeve.Waterwheel)
  //      {
		//	return;
		//}

		//if(GlobalData.GetInstance().gameLeve == (GameLeve)levelVal)
		//{
		//	return;
		//}
		MaxLinkServerCount = Network.connections.Length;
		RequestMasterServer.GetInstance().ResetIsClickConnect();
		//GlobalData.GetInstance().gameLeve = (GameLeve)levelVal;
		//Debug.Log("SendLoadLevel -> levelVal = " + (GameLeve)levelVal);

		if (Network.peerType != NetworkPeerType.Disconnected)
        {
			mNetViewCom.RPC("SendLoadLevelMsgToOthers", RPCMode.OthersBuffered, levelVal);
		}
	}

	[RPC]
	void SendLoadLevelMsgToOthers(int levelVal)
	{
		//if(GlobalData.GetInstance().gameLeve == (GameLeve)levelVal)
		//{
		//	return;
		//}
		//Debug.Log("SendLoadLevelMsgToOthers *********** levelVal " + (GameLeve)levelVal);
		MaxLinkServerCount = Network.connections.Length;
		RequestMasterServer.GetInstance().ResetIsClickConnect();
		//Toubi.GetInstance().MakeGameIntoWaterwheelNet();

		//GlobalData.GetInstance().gameLeve = (GameLeve)levelVal;
	}

	public void SetSpawnClientIndex(NetworkPlayer playerNet, int val)
	{
		//LinkPlayerCtrl.GetInstance().DisplayLinkPlayerName(val);
		if (Network.peerType != NetworkPeerType.Disconnected)
        {
			mNetViewCom.RPC("SendSpawnClientIndex", RPCMode.OthersBuffered, playerNet, val);
		}
	}
	
	[RPC]
	void SendSpawnClientIndex(NetworkPlayer playerNet, int val)
	{
		//LinkPlayerCtrl.GetInstance().DisplayLinkPlayerName(val);
		if (playerNet != Network.player)
        {
			return;
		}
		NetworkServerNet.GetInstance().SetIndexSpawnClient(val);
	}
}