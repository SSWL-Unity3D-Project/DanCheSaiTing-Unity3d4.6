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
        InitSteerInfo();
        InitJiaoTaBanInfo();
    }

    void FixedUpdate()
    {
        byte[] readBuf = MyCOMDevice.ComThreadClass.ReadByteMsg;
        if (bIsHardWare)
        {
            int jiaTaBan = ((readBuf[30] & 0x0f) << 8) + readBuf[31];
            UpdatePcvrJiaoTaBanVal(jiaTaBan);
        }

        int fangXiangVal = ((readBuf[2] & 0x0f) << 8) + readBuf[3];
        UpdatePcvrSteerVal(fangXiangVal);

        int youMenVal = ((readBuf[4] & 0x0f) << 8) + readBuf[5];
        UpdatePcvrPowerVal(youMenVal);
        UpdatePlayerCoinDt();
    }

    void Update()
    {
        if (!bIsHardWare)
        {
            UpdatePcvrJiaoTaBanVal(0);
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
    /// 编码器(脚踏板)信息.
    /// </summary>
    public int BianMaQiCurVal = 0x00;
    /// <summary>
    /// 编码器(脚踏板)信息最大值.
    /// </summary>
    int BianMaQiMaxVal = 0x0fff;
    /// <summary>
    /// 编码器(脚踏板)信息最小值.
    /// </summary>
    int BianMaQiMinVal = 0x00;
    /// <summary>
    /// 初始化脚踏板信息.
    /// </summary>
    void InitJiaoTaBanInfo()
    {
        string fileName = ReadGameInfo.GetInstance().mFileName;
        string strVal = HandleJson.GetInstance().ReadFromFileXml(fileName, "BianMaQiMaxVal");
        if (strVal == null || strVal == "")
        {
            strVal = "31000";
            HandleJson.GetInstance().WriteToFileXml(fileName, "BianMaQiMaxVal", strVal);
        }
        BianMaQiMaxVal = System.Convert.ToInt32(strVal);
        BianMaQiMinVal = 30000;
    }

    bool IsInitJiaoZhunJiaoTaBan = false;
    public void InitJiaoZhunJiaoTaBan()
    {
        IsInitJiaoZhunJiaoTaBan = true;
    }

    public void EndJiaoZhunJiaoTaBan()
    {
        IsInitJiaoZhunJiaoTaBan = false;
    }

    /// <summary>
    /// 保存脚踏板信息.
    /// </summary>
    public void SaveJiaoTaBanVal()
    {
        if (!bIsHardWare)
        {
            return;
        }
        string fileName = ReadGameInfo.GetInstance().mFileName;
        BianMaQiMaxVal = BianMaQiCurVal;
        HandleJson.GetInstance().WriteToFileXml(fileName, "BianMaQiMaxVal", BianMaQiCurVal.ToString());
    }
    
    /// <summary>
    /// 更新脚踏板信息.
    /// </summary>
    void UpdatePcvrJiaoTaBanVal(int bianMaQiVal)
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
            BianMaQiCurVal = bianMaQiVal;
            if (!IsInitJiaoZhunJiaoTaBan)
            {
                float bianMaQiInput = 0f;
                if (BianMaQiMinVal < BianMaQiMaxVal)
                {
                    //编码器(脚踏板)正接.
                    if (bianMaQiVal < BianMaQiMinVal)
                    {
                        bianMaQiVal = BianMaQiMinVal;
                    }
                    bianMaQiInput = ((float)bianMaQiVal - BianMaQiMinVal) / (BianMaQiMaxVal - BianMaQiMinVal);
                }
                else
                {
                    //编码器(脚踏板)反接.
                    if (bianMaQiVal > BianMaQiMinVal)
                    {
                        bianMaQiVal = BianMaQiMinVal;
                    }
                    bianMaQiInput = ((float)BianMaQiMinVal - bianMaQiVal) / (BianMaQiMinVal - BianMaQiMaxVal);
                }
                bianMaQiInput = Mathf.Clamp01(bianMaQiInput);
                bianMaQiInput = bianMaQiInput < 0.01f ? 0f : bianMaQiInput;
                mGetJiaoTaBan = bianMaQiInput;
            }
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
    bool IsInitJiaoZhunYouMen = false;

    /// <summary>
    /// 初始化油门校验.
    /// </summary>
    public void InitJiaoZhunYouMenValInfo()
    {
        if (IsInitJiaoZhunYouMen)
        {
            return;
        }
        IsInitJiaoZhunYouMen = true;
        YouMenMinVal = 0;
        CountYouMen = 0;
    }

    public void EndJiaoZhunYouMen()
    {
        IsInitJiaoZhunYouMen = false;
    }

    /// <summary>
    /// 检测油门最小值信息.
    /// </summary>
    void CheckYouMenMinValInfo()
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
        
        YouMenCurVal = youMenVal;
        if (!IsInitJiaoZhunYouMen)
        {
            float youMenInput = 0f;
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
        }
        else
        {
            CheckYouMenMinValInfo();
        }
    }

    [HideInInspector]
    public float mGetSteer = 0f;
    /// <summary>
    /// 方向最大值.
    /// </summary>
    int SteerValMaxAy = 0x0fff;
    /// <summary>
    /// 方向中间值.
    /// </summary>
    int SteerValCenAy = 0x07ff;
    /// <summary>
    /// 方向最小值.
    /// </summary>
    int SteerValMinAy = 0x00;
    /// <summary>
    /// 方向当前值.
    /// </summary>
    int SteerValCurAy = 0x00;
    /// <summary>
    /// 是否校准方向.
    /// </summary>
    bool IsJiaoZhunSteer = false;

    /// <summary>
    /// 初始化方向信息.
    /// </summary>
    void InitSteerInfo()
    {
        string fileName = ReadGameInfo.GetInstance().mFileName;
        string strVal = HandleJson.GetInstance().ReadFromFileXml(fileName, "SteerValMax");
        if (strVal == null || strVal == "")
        {
            strVal = "4095";
            HandleJson.GetInstance().WriteToFileXml(fileName, "SteerValMax", strVal);
        }
        SteerValMaxAy = System.Convert.ToInt32(strVal);

        strVal = HandleJson.GetInstance().ReadFromFileXml(fileName, "SteerValCen");
        if (strVal == null || strVal == "")
        {
            strVal = "2047";
            HandleJson.GetInstance().WriteToFileXml(fileName, "SteerValCen", strVal);
        }
        SteerValCenAy = System.Convert.ToInt32(strVal);
        
        strVal = HandleJson.GetInstance().ReadFromFileXml(fileName, "SteerValMin");
        if (strVal == null || strVal == "")
        {
            strVal = "0";
            HandleJson.GetInstance().WriteToFileXml(fileName, "SteerValMin", strVal);
        }
        SteerValMinAy = System.Convert.ToInt32(strVal);
    }

    public enum PcvrValState
    {
        ValMin,
        ValCenter,
        ValMax,
    }

    /// <summary>
    /// 初始化方向校准.
    /// </summary>
    public void InitJiaoZhunSteer()
    {
        IsJiaoZhunSteer = true;
    }

    /// <summary>
    /// 结束方向校准.
    /// </summary>
    public void EndJiaoZhunSteer()
    {
        IsJiaoZhunSteer = false;
    }

    /// <summary>
    /// 保存方向信息.
    /// </summary>
    public void SaveSteerVal(PcvrValState key)
    {
        string fileName = ReadGameInfo.GetInstance().mFileName;
        switch (key)
        {
            case PcvrValState.ValMin:
                {
                    SteerValMinAy = SteerValCurAy;
                    HandleJson.GetInstance().WriteToFileXml(fileName, "SteerValMin", SteerValCurAy.ToString());
                    break;
                }
            case PcvrValState.ValCenter:
                {
                    SteerValCenAy = SteerValCurAy;
                    HandleJson.GetInstance().WriteToFileXml(fileName, "SteerValCen", SteerValCurAy.ToString());
                    break;
                }
            case PcvrValState.ValMax:
                {
                    SteerValMaxAy = SteerValCurAy;
                    HandleJson.GetInstance().WriteToFileXml(fileName, "SteerValMax", SteerValCurAy.ToString());
                    break;
                }
        }
    }

    /// <summary>
    /// 更新转向信息.
    /// </summary>
	void UpdatePcvrSteerVal(int fangXiangVal)
    {
        if (!bIsHardWare)
        {
            mGetSteer = Input.GetAxis("Horizontal");
            return;
        }

        SteerValCurAy = fangXiangVal;
        if (IsJiaoZhunSteer)
        {
            return;
        }

        int steerMaxVal = SteerValMaxAy;
        int steerMinVal = SteerValMinAy;
        int steerCenVal = SteerValCenAy;
        float fangXiangValTmp = 0f;
        if (fangXiangVal >= steerCenVal)
        {
            if (steerMaxVal > steerMinVal)
            {
                //正接方向.
                fangXiangValTmp = ((float)fangXiangVal - steerCenVal) / (steerMaxVal - steerCenVal);
            }
            else
            {
                //反接方向.
                fangXiangValTmp = ((float)steerCenVal - fangXiangVal) / (steerMinVal - steerCenVal);
            }
        }
        else
        {
            if (steerMaxVal > steerMinVal)
            {
                //正接方向.
                fangXiangValTmp = ((float)fangXiangVal - steerCenVal) / (steerCenVal - steerMinVal);
            }
            else
            {
                //反接方向.
                fangXiangValTmp = ((float)steerCenVal - fangXiangVal) / (steerCenVal - steerMaxVal);
            }
        }

        fangXiangValTmp = Mathf.Clamp(fangXiangValTmp, -1f, 1f);
        fangXiangValTmp = Mathf.Abs(fangXiangValTmp) <= 0.15f ? 0f : fangXiangValTmp;
        mGetSteer = fangXiangValTmp;
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