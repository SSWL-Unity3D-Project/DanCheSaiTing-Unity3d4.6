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
            Instance.Init();
        }
        return Instance;
    }

    void Init()
    {
        InitYouMenInfo();
    }

    void FixedUpdate()
    {
        byte[] readBuf = MyCOMDevice.ComThreadClass.ReadByteMsg;
        if (bIsHardWare)
        {
            UpdatePcvrJiaoTaBanVal();
        }

        int youMenVal = ((readBuf[2] & 0x0f) << 8) + readBuf[3];
        UpdatePcvrPowerVal(youMenVal);
        UpdatePcvrSteerVal();
        UpdatePlayerCoinDt();
    }

    void Update()
    {
        if (!bIsHardWare)
        {
            UpdatePcvrJiaoTaBanVal();
        }
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
    /// 油门最大值.
    /// </summary>
    int YouMenMaxVal = 0x0fff;
    /// <summary>
    /// 油门最小值.
    /// </summary>
    int YouMenMinVal = 0x00;
    /// <summary>
    /// 油门当前值.
    /// </summary>
    int YouMenCurVal = 0x00;
    /// <summary>
    /// 油门检测计数.
    /// </summary>
    int CountYouMen = 100;
    float TimeYouMen;

    /// <summary>
    /// 初始化油门校验.
    /// </summary>
    public void InitCheckYouMenValInfo()
    {
        YouMenMinVal = 0;
        CountYouMen = 0;
    }

    /// <summary>
    /// 检测油门最小值信息.
    /// </summary>
    void CheckYouMenValInfo()
    {
        if (!bIsHardWare)
        {
            return;
        }

        if (CountYouMen >= 10)
        {
            return;
        }

        if (Time.realtimeSinceStartup - TimeYouMen < 0.1f)
        {
            return;
        }
        TimeYouMen = Time.realtimeSinceStartup;

        CountYouMen++;
        YouMenMinVal += YouMenCurVal;
        if (CountYouMen >= 10)
        {
            string fileName = ReadGameInfo.GetInstance().mFileName;
            YouMenMinVal = (int)(YouMenMinVal / 10f);
            HandleJson.GetInstance().WriteToFileXml(fileName, "YouMenMinVal", YouMenMinVal.ToString());
        }
    }

    /// <summary>
    /// 初始化油门信息.
    /// </summary>
    void InitYouMenInfo()
    {
        string strVal = "";
        string fileName = ReadGameInfo.GetInstance().mFileName;
        strVal = HandleJson.GetInstance().ReadFromFileXml(fileName, "YouMenMaxVal");
        if (strVal == null || strVal == "")
        {
            strVal = "4095";
            HandleJson.GetInstance().WriteToFileXml(fileName, "YouMenMaxVal", strVal);
        }
        YouMenMaxVal = System.Convert.ToInt32(strVal);
        
        strVal = HandleJson.GetInstance().ReadFromFileXml(fileName, "YouMenMinVal");
        if (strVal == null || strVal == "")
        {
            strVal = "0";
            HandleJson.GetInstance().WriteToFileXml(fileName, "YouMenMinVal", strVal);
        }
        YouMenMinVal = System.Convert.ToInt32(strVal);
    }

    /// <summary>
    /// 保存油门校准结果.
    /// </summary>
    public void SaveYouMenVal()
    {
        string fileName = ReadGameInfo.GetInstance().mFileName;
        YouMenMaxVal = YouMenCurVal;
        HandleJson.GetInstance().WriteToFileXml(fileName, "YouMenMaxVal", YouMenCurVal.ToString());
    }

    /// <summary>
    /// 更新油门信息.
    /// </summary>
    void UpdatePcvrPowerVal(int youMenVal)
    {
        if (!bIsHardWare)
        {
            mGetPower = Input.GetAxis("Vertical");
            return;
        }
        
        float youMenInput = 0f;
        YouMenCurVal = youMenVal;
        if (YouMenMinVal < YouMenMaxVal)
        {
            //油门正接.
            if (youMenVal < YouMenMinVal)
            {
                youMenVal = YouMenMinVal;
            }
            youMenInput = ((float)youMenVal - YouMenMinVal) / (YouMenMaxVal - YouMenMinVal);
        }
        else
        {
            //油门反接.
            if (youMenVal > YouMenMinVal)
            {
                youMenVal = YouMenMinVal;
            }
            youMenInput = ((float)YouMenMinVal - youMenVal) / (YouMenMinVal - YouMenMaxVal);
        }
        youMenInput = Mathf.Clamp01(youMenInput);
        mGetPower = youMenInput >= 0.05f ? 1f : 0f;

        CheckYouMenValInfo();
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

        //方向校准.
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