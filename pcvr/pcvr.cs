using UnityEngine;

public class pcvr : MonoBehaviour
{
    /// <summary>
    /// 是否是硬件版.
    /// </summary>
    static public bool bIsHardWare = false;
    /// <summary>
    /// 是否校验hid.
    /// </summary>
    public static bool IsJiaoYanHid;
    /// <summary>
    /// pcvr通信数据管理.
    /// </summary>
    [HideInInspector]
    public pcvrTXManage mPcvrTXManage;
    static private pcvr Instance = null;
    static public pcvr GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("_PCVR");
            DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<pcvr>();
            Instance.mPcvrTXManage = obj.AddComponent<pcvrTXManage>();
            ScreenLog.init();
            if (bIsHardWare)
            {
                MyCOMDevice.GetInstance();
            }
        }
        return Instance;
    }

    void FixedUpdate()
    {
        UpdatePcvrJiaoTaBanVal();
        UpdatePcvrPowerVal();
        UpdatePcvrSteerVal();
        UpdatePlayerCoinDt();
    }

    /// <summary>
    /// 更新玩家币值信息.
    /// </summary>
    void UpdatePlayerCoinDt()
    {
        if (bIsHardWare)
        {
            if (GlobalData.GetInstance().CoinCur != mPcvrTXManage.PlayerCoinArray[0])
            {
                GlobalData.GetInstance().CoinCur = mPcvrTXManage.PlayerCoinArray[0];
            }
        }
    }

    [HideInInspector]
    public float mGetJiaoTaBan = 0f;
    bool IsHitJiaoTaBan = false;
    /// <summary>
    /// 更新油门信息.
    /// </summary>
    void UpdatePcvrJiaoTaBanVal()
    {
        if (!bIsHardWare)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                IsHitJiaoTaBan = true;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                IsHitJiaoTaBan = false;
            }

            if (IsHitJiaoTaBan)
            {
                mGetJiaoTaBan = Mathf.Lerp(mGetJiaoTaBan, 1f, Time.deltaTime * 0.2f);
            }
            else
            {
                if (mGetJiaoTaBan >= 0.1f)
                {
                    mGetJiaoTaBan = Mathf.Lerp(mGetJiaoTaBan, 0f, Time.deltaTime * 10f);
                }
                else
                {
                    mGetJiaoTaBan = 0f;
                }
            }
        }
        else
        {

        }
    }

    [HideInInspector]
    public float mGetPower = 0f;
    /// <summary>
    /// 更新油门信息.
    /// </summary>
    void UpdatePcvrPowerVal()
    {
        if (!bIsHardWare)
        {
            mGetPower = Input.GetAxis("Vertical");
            return;
        }
    }

    enum SteerEnum
    {
        //Left = 0x55,
        //Right = 0xaa,
        Left = 0xaa,
        Right = 0x55,
        Center = 0x00,
    }

    [HideInInspector]
    public float mGetSteer = 0f;
    float TimeLastSteer = 0f;
    /// <summary>
    /// 更新转向信息.
    /// </summary>
	void UpdatePcvrSteerVal()
    {
        if (!bIsHardWare)
        {
            mGetSteer = Input.GetAxis("Horizontal");
            return;
        }

        SteerEnum steerState = (SteerEnum)(MyCOMDevice.ComThreadClass.ReadByteMsg[30]);
        switch (steerState)
        {
            case SteerEnum.Left:
                {
                    mGetSteer = -1f;
                    TimeLastSteer = Time.time;
                    break;
                }
            case SteerEnum.Center:
                {
                    if (Time.time - TimeLastSteer > 0.2f)
                    {
                        mGetSteer = 0f;
                    }
                    break;
                }
            case SteerEnum.Right:
                {
                    mGetSteer = 1f;
                    TimeLastSteer = Time.time;
                    break;
                }
        }
    }

    /// <summary>
    /// 闪光灯控制命令.
    /// </summary>
    [System.Serializable]
    public class ShanGuangDengCmd
    {
        public bool[] LedCmd = new bool[7];
    }

    /// <summary>
    /// 改变闪关灯.
    /// </summary>
    public void ChangeShanGuangDeng(ShanGuangDengCmd shanGuangDengCmd)
    {
        for (int i = 0; i < 7; i++)
        {
            mPcvrTXManage.LedState[i] = shanGuangDengCmd.LedCmd[i];
        }
    }

    public void CloseShanGuangDeng()
    {
        for (int i = 0; i < 7; i++)
        {
            mPcvrTXManage.LedState[i] = false;
        }
    }

    //void OnGUI()
    //{
    //    string info = "ledState:  ";
    //    for (int i = 0; i < 7; i++)
    //    {
    //        info += mPcvrTXManage.LedState[i] == true ? "1  " : "0  ";
    //    }
    //    GUI.Box(new Rect(10f, 100f, Screen.width - 20f, 30f), info);
    //}
}