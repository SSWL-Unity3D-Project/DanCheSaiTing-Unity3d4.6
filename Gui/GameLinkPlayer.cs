using UnityEngine;

public class GameLinkPlayer : MonoBehaviour
{
    /// <summary>
    /// 正在创建服务器UI.
    /// </summary>
    public GameObject UICreateSever;
    /// <summary>
    /// 联机玩家昵称列表父级.
    /// </summary>
    public GameObject LinkNameParent;
    /// <summary>
    /// 开始按键UI.
    /// </summary>
    public GameObject StartBtObj;
    /// <summary>
    /// 联机玩家UI列表.
    /// </summary>
    public UITexture[] PlayerUITextureArray = new UITexture[8];
    public Texture[] PlayerTexureArray = new Texture[8];
    bool IsClickStartBt = false;
    /// <summary>
    /// 记录开始按键时间.
    /// </summary>
    float TimeLastStartBt = 0f;
    Loading mLoadingCom;
    public void Init(Loading loadingScript)
    {
        mLoadingCom = loadingScript;
        SetActiveLinkNameParent(false);
        SetAcitveStartBt(false);
        SetPlayerUITexture(0);
        SetActiveUICreateSever(true);
    }

    /// <summary>
    /// 玩家索引.
    /// </summary>
    int IndexPlayer = 0;
    /// <summary>
    /// 当前联机到服务器的玩家数.
    /// </summary>
    int PlayerLinkServerCount = 0;
    void Update()
    {
        if (Time.frameCount % 30 == 0)
        {
            if (!StartBtObj.activeSelf && !IsClickStartBt)
            {
                if (NetworkServerNet.GetInstance().mRequestMasterServer.GetMovieMasterServerNum() == 1
                    && NetworkServerNet.GetInstance().mRequestMasterServer.ServerIp == Network.player.ipAddress
                    && Network.peerType == NetworkPeerType.Server)
                {
                    TimeLastStartBt += (Time.deltaTime * 30);
                    if (TimeLastStartBt >= 3f)
                    {
                        //循环动画场景主服务器有且只有1个动画服务端时,才允许显示开始按键.
                        //SetAcitveStartBt(true);
                        //产生选择游戏场景UI.
                        mLoadingCom.SpawnLevelSelectUI();
                        //隐藏创建服务器的UI提示.
                        SetActiveUICreateSever(false);
                    }
                }
                else
                {
                    TimeLastStartBt = 0f;
                }
            }

            if (NetworkServerNet.GetInstance() != null)
            {
                if (IndexPlayer != NetworkServerNet.GetInstance().IndexSpawnPlayer)
                {
                    IndexPlayer = NetworkServerNet.GetInstance().IndexSpawnPlayer;
                    Debug.Log("GameLinkPlayer::update -> IndexPlayer == " + IndexPlayer);
                    SetActiveLinkNameParent(true);
                    ChangeUINameScale(IndexPlayer);
                }

                if (PlayerLinkServerCount != NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie)
                {
                    PlayerLinkServerCount = NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie;
                    Debug.Log("GameLinkPlayer::update -> PlayerLinkServerCount == " + PlayerLinkServerCount);
                    SetPlayerUITexture(PlayerLinkServerCount);
                }
            }
        }
    }

    public void SetAcitveStartBt(bool isActive)
    {
        StartBtObj.SetActive(isActive);
    }

    public void SetActiveLinkNameParent(bool isActive)
    {
        LinkNameParent.SetActive(isActive);
        if (isActive)
        {
            //隐藏正在创建服务器的UI提示.
            SetActiveUICreateSever(false);
        }
    }

    void SetActiveUICreateSever(bool isActive)
    {
        UICreateSever.SetActive(isActive);
    }

    /// <summary>
    /// 改变玩家昵称UI大小.
    /// </summary>
    public void ChangeUINameScale(int index)
    {
        for (int i = 0; i < PlayerUITextureArray.Length; i++)
        {
            if (index == i)
            {
                PlayerUITextureArray[i].transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            }
            else
            {
                PlayerUITextureArray[i].transform.localScale = Vector3.one;
            }
        }
    }

    /// <summary>
    /// 点击开始按键.
    /// </summary>
    public void OnClickStartBt()
    {
        if (!IsClickStartBt)
        {
            IsClickStartBt = true;
            SetAcitveStartBt(false);
            HiddenSelf();
        }
    }

    /// <summary>
    /// 设置联机玩家UI信息.
    /// indexVal [0, 4].
    /// </summary>
    public void SetPlayerUITexture(int indexVal)
    {
        for (int i = 0; i < PlayerUITextureArray.Length; i++)
        {
            if (i > indexVal)
            {
                break;
            }
            PlayerUITextureArray[i].mainTexture = PlayerTexureArray[i];
        }
    }

    public void HiddenSelf()
    {
        gameObject.SetActive(false);
    }
}