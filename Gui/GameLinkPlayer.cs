using UnityEngine;

public class GameLinkPlayer : MonoBehaviour
{
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
    public void Init()
    {
        SetAcitveStartBt(false);
        SetPlayerUITexture(0);
    }

    void Update()
    {
        if (Time.frameCount % 30 == 0)
        {
            if (!StartBtObj.activeSelf && !IsClickStartBt)
            {
                if (NetworkServerNet.GetInstance().mRequestMasterServer.GetMovieMasterServerNum() == 1
                    && NetworkServerNet.GetInstance().mRequestMasterServer.ServerIp == Network.player.ipAddress)
                {
                    SetAcitveStartBt(true);
                }
            }
        }
    }

    void SetAcitveStartBt(bool isActive)
    {
        StartBtObj.SetActive(isActive);
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
    /// indexVal [0, 7].
    /// </summary>
    public void SetPlayerUITexture(int indexVal)
    {
        for (int i = 0; i < 8; i++)
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