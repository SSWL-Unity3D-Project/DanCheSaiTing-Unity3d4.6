using UnityEngine;

/// <summary>
/// pcvr通信数据管理.
/// </summary>
public class pcvrTXManage : MonoBehaviour
{
    byte ReadHead_1 = 0x53;
    byte ReadHead_2 = 0x57;
    byte EndRead_1 = 0x0d;
    byte EndRead_2 = 0x0a;
    byte WriteHead_1 = 0x09;
    byte WriteHead_2 = 0x05;
    byte WriteEnd_1 = 0x0d;
    byte WriteEnd_2 = 0x0a;
    /// <summary>
    /// 继电器命令.
    /// </summary>
    public enum JiDianQiCmd
    {
        Close,
        Open,
    }
    /// <summary>
    /// 继电器开关命令.
    /// </summary>
    [HideInInspector]
    public JiDianQiCmd[] JiDianQiCmdArray = new JiDianQiCmd[2];
    /// <summary>
    /// 彩票打印状态.
    /// </summary>
    public enum CaiPiaoPrintState
    {
        Null = -1,
        WuXiao = 0x00,
        Succeed = 0x55,
        Failed = 0xaa,
    }
    /// <summary>
    /// 彩票机编号.
    /// </summary>
    public enum CaiPiaoJi
    {
        Null = -1,
        Num01 = 0,
        Num02 = 1,
    }
    /// <summary>
    /// 彩票打印命令.
    /// </summary>
    public enum CaiPiaoPrintCmd
    {
        WuXiao = 0x00,
        QuanPiaoPrint = 0x55,
        BanPiaoPrint = 0x66,
        StopPrint = 0xaa,
    }
    CaiPiaoPrintCmd[] CaiPiaoPrintCmdVal = new CaiPiaoPrintCmd[2];
    /// <summary>
    /// 彩票打印数量.
    /// </summary>
    [HideInInspector]
    public int[] CaiPiaoCountPrint = new int[2];
    /// <summary>
    /// 彩票打印失败次数.
    /// </summary>
    [HideInInspector]
    public byte[] CaiPiaoPrintFailedCount = new byte[2];
    CaiPiaoPrintState[] CaiPiaoJiPrintStArray = new CaiPiaoPrintState[2];
    /// <summary>
    /// 是否清除hid币值.
    /// </summary>
    bool[] IsCleanHidCoinArray = new bool[4];

    /// <summary>
    /// 游戏开启了几局.
    /// </summary>
    [HideInInspector]
    public int GamePlayCount;
    /// <summary>
    /// 检测加密芯片的最小游戏局数.
    /// </summary>
    [HideInInspector]
    public int JiaMiGamePlayCountMin;

    void Start()
    {
        InitJiaoYanMiMa();
    }

    void FixedUpdate()
    {
        if (!pcvr.bIsHardWare)
        {
            return;
        }
        SendMessage();
        GetMessage();
    }

    /// <summary>
    /// 发送串口数据信息.
    /// </summary>
    void SendMessage()
    {
        if (!MyCOMDevice.GetInstance().IsFindDeviceDt)
        {
            return;
        }

        int writeCount = MyCOMDevice.ComThreadClass.WriteCountLock;
        if (WriteMsgCount == writeCount)
        {
            return;
        }
        WriteMsgCount = writeCount;

        int bufLenWrite = MyCOMDevice.ComThreadClass.BufLenWrite;
        byte[] buffer = new byte[bufLenWrite];
        for (int i = 5; i < (bufLenWrite - 2); i++)
        {
            buffer[i] = (byte)Random.Range(0x00, 0xff);
        }
        buffer[0] = WriteHead_1;
        buffer[1] = WriteHead_2;
        //buffer[bufLenWrite - 2] = WriteEnd_1;
        //buffer[bufLenWrite - 1] = WriteEnd_2;
        buffer[bufLenWrite - 3] = WriteEnd_1;
        buffer[bufLenWrite - 2] = WriteEnd_2;

        //减币控制.
        if (!IsCleanHidCoinArray[0] && !IsCleanHidCoinArray[1] && !IsCleanHidCoinArray[2] && !IsCleanHidCoinArray[3])
        {
            buffer[4] = 0x00;
        }
        else
        {
            if (IsCleanHidCoinArray[0] || IsCleanHidCoinArray[1])
            {
                buffer[4] = 0x04;
                if (IsCleanHidCoinArray[0] && IsCleanHidCoinArray[1])
                {
                    buffer[2] = 0x11;
                }
                else if (IsCleanHidCoinArray[0])
                {
                    buffer[2] = 0x01;
                }
                else if (IsCleanHidCoinArray[1])
                {
                    buffer[2] = 0x10;
                }
            }

            if (IsCleanHidCoinArray[2] || IsCleanHidCoinArray[3])
            {
                buffer[4] = 0x04;
                if (IsCleanHidCoinArray[2] && IsCleanHidCoinArray[3])
                {
                    buffer[3] = 0x11;
                }
                else if (IsCleanHidCoinArray[2])
                {
                    buffer[3] = 0x01;
                }
                else if (IsCleanHidCoinArray[3])
                {
                    buffer[3] = 0x10;
                }
            }
        }

        //加密芯片校验控制.
        if (IsJiaoYanHid)
        {
            //校验允许1
            buffer[29] = 0x42;
            //校验允许2
            buffer[31] = 0x12;

            //密码指示---由第3、5、8位确定
            buffer[33] = mJiaMiPWDCmd[JiaMiPWDCmdIndex].Cmd;
            //密码1
            buffer[34] = JiaoYanMiMa[mJiaMiPWDCmd[JiaMiPWDCmdIndex].Index01];
            //密码2
            buffer[38] = JiaoYanMiMa[mJiaMiPWDCmd[JiaMiPWDCmdIndex].Index02];
            //密码3
            buffer[45] = JiaoYanMiMa[mJiaMiPWDCmd[JiaMiPWDCmdIndex].Index03];

            //加密运算标记（指示数据1、2、3）---位6、3、2
            buffer[37] = mJiaMiDtCmd[JiaMiDtCmdIndex].Cmd;
            //加密校验数据1
            buffer[40] = JiaoYanDt[mJiaMiDtCmd[JiaMiDtCmdIndex].Index01];
            //加密校验数据2
            buffer[41] = JiaoYanDt[mJiaMiDtCmd[JiaMiDtCmdIndex].Index02];
            //加密校验数据3
            buffer[35] = JiaoYanDt[mJiaMiDtCmd[JiaMiDtCmdIndex].Index03];
            if (IsJiaoYanJiaMiCoreDt)
            {
                Debug.Log("buffer_29 " + buffer[29].ToString("X2") + ", buffer_31 " + buffer[31].ToString("X2"));
                Debug.Log("miMa:: buffer_33 " + buffer[33].ToString("X2") + ", buffer_34 " + buffer[34].ToString("X2") + ", buffer_38 " + buffer[38].ToString("X2") + ", buffer_45 " + buffer[45].ToString("X2") + ", JiaMiPWDCmdIndex " + JiaMiPWDCmdIndex);
                Debug.Log("dtVal:: buffer_37 " + buffer[37].ToString("X2") + ", buffer_40 " + buffer[40].ToString("X2") + ", buffer_41 " + buffer[41].ToString("X2") + ", buffer_35 " + buffer[35].ToString("X2") + ", JiaMiDtCmdIndex " + JiaMiDtCmdIndex);
            }
        }
        else
        {
            //不进行加密芯片校验.
            buffer[29] = 0x00;
            buffer[31] = 0x00;
            buffer[33] = 0x00;
            buffer[37] = 0x00;
        }

        //彩票打印控制.
        buffer[19] = (byte)CaiPiaoPrintCmdVal[(int)CaiPiaoJi.Num01];
        buffer[20] = (byte)CaiPiaoPrintCmdVal[(int)CaiPiaoJi.Num02];

        //灯1控制
        LedData ledDt = new LedData(LedIndexEnum.Led01, buffer[13], buffer[12], 0x02, 0x04, 0x40, 0x08);
        SetLedInfo(out buffer[13], out buffer[12], ledDt);

        //灯2控制
        ledDt = new LedData(LedIndexEnum.Led02, buffer[27], buffer[15], 0x04, 0x10, 0x40, 0x08);
        SetLedInfo(out buffer[27], out buffer[15], ledDt);

        //灯3控制
        ledDt = new LedData(LedIndexEnum.Led03, buffer[14], buffer[16], 0x04, 0x10, 0x04, 0x10);
        SetLedInfo(out buffer[14], out buffer[16], ledDt);

        //灯4控制
        ledDt = new LedData(LedIndexEnum.Led04, buffer[6], buffer[17], 0x04, 0x10, 0x04, 0x10);
        SetLedInfo(out buffer[6], out buffer[17], ledDt);

        //灯5控制
        ledDt = new LedData(LedIndexEnum.Led05, buffer[11], buffer[7], 0x04, 0x40, 0x80, 0x04);
        SetLedInfo(out buffer[11], out buffer[7], ledDt);

        //灯6控制
        ledDt = new LedData(LedIndexEnum.Led06, buffer[24], buffer[10], 0x02, 0x10, 0x04, 0x40);
        SetLedInfo(out buffer[24], out buffer[10], ledDt);

        //灯7控制
        ledDt = new LedData(LedIndexEnum.Led07, buffer[30], buffer[8], 0x40, 0x10, 0x20, 0x10);
        SetLedInfo(out buffer[30], out buffer[8], ledDt);

        //灯8控制
        ledDt = new LedData(LedIndexEnum.Led08, buffer[9], buffer[5], 0x02, 0x20, 0x20, 0x10);
        SetLedInfo(out buffer[9], out buffer[5], ledDt);

        //灯9控制
        SetLedState(LedIndexEnum.Led09, buffer[21], out buffer[21]);

        //灯10控制
        SetLedState(LedIndexEnum.Led10, buffer[21], out buffer[21]);

        //灯11控制
        SetLedState(LedIndexEnum.Led11, buffer[21], out buffer[21]);

        //灯12控制
        SetLedState(LedIndexEnum.Led12, buffer[21], out buffer[21]);

        //灯13控制
        SetLedState(LedIndexEnum.Led13, buffer[21], out buffer[21]);

        //灯14控制
        SetLedState(LedIndexEnum.Led14, buffer[21], out buffer[21]);

        //灯15控制
        SetLedState(LedIndexEnum.Led15, buffer[21], out buffer[21]);

        //灯16控制
        SetLedState(LedIndexEnum.Led16, buffer[21], out buffer[21]);

        //灯17控制
        SetLedState(LedIndexEnum.Led17, buffer[22], out buffer[22]);

        //灯18控制
        SetLedState(LedIndexEnum.Led18, buffer[22], out buffer[22]);

        //灯19控制
        SetLedState(LedIndexEnum.Led19, buffer[22], out buffer[22]);

        //灯20控制
        SetLedState(LedIndexEnum.Led20, buffer[22], out buffer[22]);

        //灯21控制
        SetLedState(LedIndexEnum.Led21, buffer[22], out buffer[22]);

        //灯22控制
        SetLedState(LedIndexEnum.Led22, buffer[22], out buffer[22]);

        //灯23控制
        SetLedState(LedIndexEnum.Led23, buffer[22], out buffer[22]);

        //灯24控制
        SetLedState(LedIndexEnum.Led24, buffer[22], out buffer[22]);

        //灯25控制
        SetLedState(LedIndexEnum.Led25, buffer[26], out buffer[26]);

        //灯26控制
        SetLedState(LedIndexEnum.Led26, buffer[26], out buffer[26]);

        //灯27控制
        SetLedState(LedIndexEnum.Led27, buffer[26], out buffer[26]);

        //灯28控制
        SetLedState(LedIndexEnum.Led28, buffer[26], out buffer[26]);

        //灯29控制
        SetLedState(LedIndexEnum.Led29, buffer[26], out buffer[26]);

        //灯30控制
        SetLedState(LedIndexEnum.Led30, buffer[26], out buffer[26]);

        //灯31控制
        SetLedState(LedIndexEnum.Led31, buffer[26], out buffer[26]);

        //灯32控制
        SetLedState(LedIndexEnum.Led32, buffer[26], out buffer[26]);

        //Led灯总控.
        SetLedZongKongInfo(out buffer[18], buffer[18]);

        //继电器控制.
        buffer[23] = GetJiDianQiCmd();

        //数据校验位 26~44的异或校验、起始值为0x58（不包含自身） 第一步.
        buffer[32] = 0x58;
        for (int i = 26; i <= 44; i++)
        {
            if (i == 32)
            {
                continue;
            }
            buffer[32] ^= buffer[i];
        }

        //校验位，2~45的异或校验、起始值为0x85（不包含自身） 第二步.
        buffer[25] = 0x85;
        for (int i = 2; i <= 45; i++)
        {
            if (i == 25)
            {
                continue;
            }
            buffer[25] ^= buffer[i];
        }

        //数据校验 3~49的数据异或、异或初始值为0xba（不包含自身） 最后一步.
        buffer[46] = 0xba;
        for (int i = 3; i <= 49; i++)
        {
            if (i == 46)
            {
                continue;
            }
            buffer[46] ^= buffer[i];
        }

        buffer[bufLenWrite - 1] = (byte)(TestSendDt % 256); //test
        TestSendDt++; //test
        MyCOMDevice.ComThreadClass.WriteByteMsg = buffer;
    }
    byte TestSendDt = 0;

    enum LedIndexEnum
    {
        Null = 0,
        Led01 = 1,
        Led02 = 2,
        Led03 = 3,
        Led04 = 4,
        Led05 = 5,
        Led06 = 6,
        Led07 = 7,
        Led08 = 8,
        Led09 = 9,
        Led10 = 10,
        Led11 = 11,
        Led12 = 12,
        Led13 = 13,
        Led14 = 14,
        Led15 = 15,
        Led16 = 16,
        Led17 = 17,
        Led18 = 18,
        Led19 = 19,
        Led20 = 20,
        Led21 = 21,
        Led22 = 22,
        Led23 = 23,
        Led24 = 24,
        Led25 = 25,
        Led26 = 26,
        Led27 = 27,
        Led28 = 28,
        Led29 = 29,
        Led30 = 30,
        Led31 = 31,
        Led32 = 32,
    }
    class LedData
    {
        public LedIndexEnum IndexLed = LedIndexEnum.Null;
        /// <summary>
        /// 有效检验数据
        /// </summary>
        public byte LedKey = 0;
        /// <summary>
        /// 有效数据
        /// </summary>
        public byte LedVal = 0;
        public byte LedKey01 = 0; //有效检验数1
        public byte LedKey02 = 0; //有效检验数2
        public byte LedVal01 = 0; //有效数据1
        public byte LedVal02 = 0; //有效数据2
        public LedData(LedIndexEnum indexLed, byte ledKey, byte ledVal, byte ledKey01, byte ledKey02, byte ledVal01, byte ledVal02)
        {
            IndexLed = indexLed;
            LedKey = ledKey;
            LedVal = ledVal;
            LedKey01 = ledKey01;
            LedKey02 = ledKey02;
            LedVal01 = ledVal01;
            LedVal02 = ledVal02;
        }
    }

    /// <summary>
    /// 设置Led(9-32)的状态.
    /// </summary>
    void SetLedState(LedIndexEnum indexLed, byte ledBuf, out byte outLedBuf)
    {
        bool isOpenLed = LedState[(int)indexLed - 1];
        int indexVal = ((int)indexLed - 1) % 8;
        int keyVal = (int)Mathf.Pow(2, indexVal);
        outLedBuf = (byte)(isOpenLed == true ? (ledBuf | keyVal) : (ledBuf & (~keyVal)));
    }

    /// <summary>
    /// 设置Led(1-8)的状态.
    /// </summary>
    void SetLedInfo(out byte ledKeyOut, out byte ledValOut, LedData ledDt)
    {
        byte indexLed = (byte)ledDt.IndexLed;
        indexLed -= 1;
        if (indexLed < 0 || indexLed > 7)
        {
            Debug.LogError("SetLedInfo -> indexLed was wrong! indexLed " + indexLed);
            ledKeyOut = ledDt.LedKey;
            ledValOut = ledDt.LedVal;
            return;
        }

        byte ledKey = ledDt.LedKey;
        byte ledVal = ledDt.LedVal;
        byte ledKey01 = ledDt.LedKey01; //有效检验数1
        byte ledKey02 = ledDt.LedKey02; //有效检验数2
        byte ledVal01 = ledDt.LedVal01; //有效数据1
        byte ledVal02 = ledDt.LedVal02; //有效数据2
        bool isOpenLed = LedState[indexLed];
        if (Random.Range(0, 100) % 2 == 0)
        {
            ledKey |= ledKey01;
            ledKey = (byte)(ledKey & (~(ledKey02)));
            if (isOpenLed)
            {
                ledVal = (byte)(ledVal & (~(ledVal01)));
            }
            else
            {
                ledVal |= ledVal01;
            }
        }
        else
        {
            ledKey |= ledKey02;
            ledKey = (byte)(ledKey & (~(ledKey01)));
            if (isOpenLed)
            {
                ledVal = (byte)(ledVal & (~(ledVal02)));
            }
            else
            {
                ledVal |= ledVal02;
            }
        }

        ledKeyOut = ledKey;
        ledValOut = ledVal;
    }

    /// <summary>
    /// Led状态.
    /// </summary>
    [HideInInspector]
    public bool[] LedState = new bool[32];
    /// <summary>
    /// 设置Led总控命令.
    /// </summary>
    void SetLedZongKongInfo(out byte ledZongKongNew, byte ledZongKongOld)
    {
        ledZongKongNew = ledZongKongOld;
        ledZongKongNew = (byte)(ledZongKongNew | 0x40);
        ledZongKongNew = (byte)(ledZongKongNew & 0xFB);
    }

    #region Pcvr_JiaMiJiaoYan
    int WriteMsgCount = -1;
    byte MiMaJiaoYanCount = 0;
    /// <summary>
    /// 校验hid加密芯片.
    /// </summary>
    void CheckHidJiaMiXinPian(byte[] buffer)
    {
        bool isFailedJiaoYan = true;
        if (buffer[47] == 0x00)
        {
            Debug.Log("CheckHidJiaMiXinPian weiJiaoYan -> buffer_47 was wrong! val " + buffer[47].ToString("X2"));
            return;
        }

        if (buffer[47] == 0xff)
        {
            Debug.Log("CheckHidJiaMiXinPian jiaoYanCuoWu -> buffer_47 was wrong! val " + buffer[47].ToString("X2"));
            //加密芯片校验失败.
            OnEndJiaoYanIO(JIAOYANENUM.FAILED);
            return;
        }
        MiMaJiaoYanCount++;
        Debug.Log("CheckHidJiaMiXinPian -> MiMaJiaoYanCount " + MiMaJiaoYanCount + ", time " + System.DateTime.Now.ToString("HH:mm:ss") + ", " + System.DateTime.Now.Millisecond + "ms");

        bool isCheckDt = false;
        byte[] jiaoYanDtArray = new byte[4];
        if ((buffer[47] & 0x04) == 0x04)
        {
            isCheckDt = true;
            jiaoYanDtArray[1] = buffer[49];
            jiaoYanDtArray[2] = buffer[48];
            jiaoYanDtArray[3] = buffer[54];
        }

        if ((buffer[47] & 0x20) == 0x20)
        {
            isCheckDt = true;
            jiaoYanDtArray[1] = buffer[54];
            jiaoYanDtArray[2] = buffer[49];
            jiaoYanDtArray[3] = buffer[48];
        }

        if (isCheckDt)
        {
            if (jiaoYanDtArray[1] == (JiaoYanDt[1] & 0xef)
                && jiaoYanDtArray[2] == (JiaoYanDt[2] & 0xde)
                && jiaoYanDtArray[3] == (JiaoYanDt[3] | 0x82))
            {
                Debug.Log("CheckHidJiaMiXinPian -> buffer_47 == " + buffer[47].ToString("X2"));
                for (int i = 1; i < jiaoYanDtArray.Length; i++)
                {
                    Debug.Log("CheckHidJiaMiXinPian -> JiaoYanDt_0" + i + " == " + JiaoYanDt[i].ToString("X2") + ", GetJiaoYanDt_0" + i + " == " + jiaoYanDtArray[i].ToString("X2"));
                }
                //加密芯片校验成功.
                OnEndJiaoYanIO(JIAOYANENUM.SUCCEED);
                isFailedJiaoYan = false;
                //MyCOMDevice.ComThreadClass.ReadCount = 0; //重置ReadCount.
                //IsCanCheckReadMsg = false;
            }
            else
            {
                Debug.Log("CheckHidJiaMiXinPian -> Get JiaMiDt was wrong! buffer_47 == " + buffer[47].ToString("X2"));
                for (int i = 1; i < jiaoYanDtArray.Length; i++)
                {
                    Debug.Log("CheckHidJiaMiXinPian -> JiaoYanDt_0" + i + " == " + JiaoYanDt[i].ToString("X2") + ", GetJiaoYanDt_0" + i + " == " + jiaoYanDtArray[i].ToString("X2"));
                }
            }
        }
        else
        {
            Debug.Log("CheckHidJiaMiXinPian -> buffer_47 was wrong! val " + buffer[47].ToString("X2"));
        }

        if (MiMaJiaoYanCount >= 3 && isFailedJiaoYan)
        {
            //收到的加密校验信息超过一定次数则认为失败.
            //加密芯片校验失败.
            OnEndJiaoYanIO(JIAOYANENUM.FAILED);
        }
    }

    /// <summary>
    /// 随机校验数据.
    /// </summary>
    void RandomJiaoYanDt()
    {
        for (int i = 1; i < 4; i++)
        {
            JiaoYanDt[i] = (byte)Random.Range(0x01, 0x7b);
        }
        JiaoYanDt[1] = 0xef;
        JiaoYanDt[2] = 0xde;
        JiaoYanDt[3] = 0x82;

        //JiaoYanDt[0] = 0x00;
        //for (int i = 1; i < 4; i++)
        //{
        //    JiaoYanDt[0] ^= JiaoYanDt[i];
        //}
        Debug.Log("RandomJiaoYanDt -> dt01 " + JiaoYanDt[1].ToString("X2") + ", dt02 " + JiaoYanDt[2].ToString("X2")
             + ", dt03 " + JiaoYanDt[3].ToString("X2"));

        JiaMiPWDCmdIndex = (byte)(Random.Range(0, 100) % mJiaMiPWDCmd.Length);
        JiaMiDtCmdIndex = (byte)(Random.Range(0, 100) % mJiaMiDtCmd.Length);
        JiaMiPWDCmdIndex = (byte)(0 % mJiaMiPWDCmd.Length); //test
        JiaMiDtCmdIndex = (byte)(1 % mJiaMiDtCmd.Length); //test
    }

    /// <summary>
    /// 游戏校验加密芯片(当游戏结束一局后调用该方法).
    /// </summary>
    public void GameJiaoYanJiaMiXinPian()
    {
        GamePlayCount++;
        if (GamePlayCount >= JiaMiGamePlayCountMin)
        {
            StartJiaoYanIO();
        }
    }

    /// <summary>
    /// 开始校验IO板加密芯片.
    /// </summary>
    public void StartJiaoYanIO()
    {
        if (IsJiaoYanHid)
        {
            return;
        }

        if (HardWareTest.GetInstance() != null)
        {
        }
        else
        {
            if (JiaoYanSucceedCount >= JiaoYanFailedMax)
            {
                //校验加密芯片成功后就不用再继续校验.
                return;
            }

            if ((JiaoYanState == JIAOYANENUM.FAILED && JiaoYanFailedCount >= JiaoYanFailedMax) || IsJiaMiJiaoYanFailed)
            {
                //加密芯片校验失败后,无需继续校验.
                return;
            }
        }

        RandomJiaoYanDt();
        IsJiaoYanHid = true;
        MiMaJiaoYanCount = 0;
        CancelInvoke("CloseJiaoYanIO");
        Invoke("CloseJiaoYanIO", 5f);
        Debug.Log("Start IO JiaMi JiaoYan...");
    }

    /// <summary>
    /// 延时自动关闭芯片校验.
    /// </summary>
    void CloseJiaoYanIO()
    {
        if (!IsJiaoYanHid)
        {
            return;
        }
        IsJiaoYanHid = false;
        //芯片校验失败.
        OnEndJiaoYanIO(JIAOYANENUM.FAILED);
    }

    void OnEndJiaoYanIO(JIAOYANENUM val)
    {
        IsJiaoYanHid = false;
        if (IsInvoking("CloseJiaoYanIO"))
        {
            CancelInvoke("CloseJiaoYanIO");
        }

        switch (val)
        {
            case JIAOYANENUM.FAILED:
                {
                    JiaoYanFailedCount++;
                    JiaoYanSucceedCount = 0;
                    if (HardWareTest.GetInstance() != null)
                    {
                        HardWareTest.GetInstance().JiaMiJiaoYanFailed();
                    }
                    break;
                }

            case JIAOYANENUM.SUCCEED:
                {
                    JiaoYanSucceedCount++;
                    JiaoYanFailedCount = 0;
                    if (HardWareTest.GetInstance() != null)
                    {
                        HardWareTest.GetInstance().JiaMiJiaoYanSucceed();
                    }
                    break;
                }
        }
        JiaoYanState = val;
        Debug.Log("OnEndJiaoYanIO -> JiaoYanState " + JiaoYanState);

        if (HardWareTest.GetInstance() != null)
        {
        }
        else
        {
            if (JiaoYanFailedCount >= JiaoYanFailedMax)
            {
                //加密校验失败.
                Debug.Log("JMXP JYSB...");
                IsJiaMiJiaoYanFailed = true;
            }
        }
        IsJiaoYanJiaMiCoreDt = false;
    }

    /// <summary>
    /// 加密校验是否失败.
    /// </summary>
    bool IsJiaMiJiaoYanFailed;
    public enum JIAOYANENUM
    {
        NULL,
        SUCCEED,
        FAILED,
    }
    JIAOYANENUM JiaoYanState = JIAOYANENUM.NULL;
    /// <summary>
    /// 加密芯片校验失败的最大次数.
    /// </summary>
    byte JiaoYanFailedMax = 0x03;
    /// <summary>
    /// 当前校验成功次数.
    /// </summary>
    byte JiaoYanSucceedCount;
    /// <summary>
    /// 当前校验失败次数.
    /// </summary>
    byte JiaoYanFailedCount;

    /// <summary>
    /// 加密芯片密码命令.
    /// </summary>
    class JiaMiPWDCmd
    {
        public byte Cmd = 0x00;
        /// <summary>
        /// 密码索引.
        /// </summary>
        public byte Index01 = 0x00;
        public byte Index02 = 0x00;
        public byte Index03 = 0x00;
        public JiaMiPWDCmd(byte cmd, byte index01, byte index02, byte index03)
        {
            Cmd = cmd;
            Index01 = index01;
            Index02 = index02;
            Index03 = index03;
        }
    }
    static JiaMiPWDCmd[] mJiaMiPWDCmd = new JiaMiPWDCmd[]{
        new JiaMiPWDCmd(0x84, 1, 2, 3),
        new JiaMiPWDCmd(0x80, 2, 3, 1),
        new JiaMiPWDCmd(0x10, 3, 2, 1),
        new JiaMiPWDCmd(0x14, 3, 1, 2),
        new JiaMiPWDCmd(0x90, 2, 1, 3),
    };
    /// <summary>
    /// 加密芯片密码命令索引.
    /// </summary>
    static byte JiaMiPWDCmdIndex = 0x00;

    /// <summary>
    /// 加密数据命令.
    /// </summary>
    class JiaMiDtCmd
    {
        public byte Cmd = 0x00;
        /// <summary>
        /// 数据索引.
        /// </summary>
        public byte Index01 = 0x00;
        public byte Index02 = 0x00;
        public byte Index03 = 0x00;
        public JiaMiDtCmd(byte cmd, byte index01, byte index02, byte index03)
        {
            Cmd = cmd;
            Index01 = index01;
            Index02 = index02;
            Index03 = index03;
        }
    }
    JiaMiDtCmd[] mJiaMiDtCmd = new JiaMiDtCmd[]{
        new JiaMiDtCmd(0x22, 3, 1, 2),
        new JiaMiDtCmd(0x06, 1, 2, 3),
        new JiaMiDtCmd(0x24, 1, 3, 2),
        new JiaMiDtCmd(0x04, 2, 3, 1),
        new JiaMiDtCmd(0x20, 2, 1, 3),
    };
    /// <summary>
    /// 加密数据命令索引.
    /// </summary>
    byte JiaMiDtCmdIndex = 0x00;
    /// <summary>
    /// 加密校验数据.
    /// </summary>
    byte[] JiaoYanDt = new byte[4];
    /// <summary>
    /// 加密校验密码.
    /// </summary>
    static byte[] JiaoYanMiMa = new byte[4];
    /// <summary>
    /// 是否检测加密芯片数据.
    /// </summary>
    static bool IsJiaoYanJiaMiCoreDt = false;
    /// <summary>
    /// 检测是否开始校验加密芯片数据.
    /// </summary>
    public static void CheckIsJiaoYanJiaMiCore(byte[] miMaArray)
    {
        if (JiaoYanMiMa[mJiaMiPWDCmd[JiaMiPWDCmdIndex].Index01] == miMaArray[1]
            && JiaoYanMiMa[mJiaMiPWDCmd[JiaMiPWDCmdIndex].Index02] == miMaArray[2]
            && JiaoYanMiMa[mJiaMiPWDCmd[JiaMiPWDCmdIndex].Index03] == miMaArray[3])
        {
            IsJiaoYanJiaMiCoreDt = true;
            Debug.Log("CheckIsJiaoYanJiaMiCore...");
        }
    }

    /// <summary>
    /// 初始化加密校验.
    /// </summary>
    void InitJiaoYanMiMa()
    {
        //#define First_pin		 	0x9a
        //#define Second_pin		0xb7
        //#define Third_pin		 	0xab
        JiaoYanMiMa[1] = 0x9a;
        JiaoYanMiMa[2] = 0xb7;
        JiaoYanMiMa[3] = 0xab;
        JiaoYanMiMa[0] = 0x00;
        for (int i = 1; i < 4; i++)
        {
            JiaoYanMiMa[0] ^= JiaoYanMiMa[i];
        }
        JiaMiGamePlayCountMin = Random.Range(5, 15);
    }

    bool _IsJiaoYanHid = false;
    /// <summary>
    /// 是否校验hid.
    /// </summary>
    [HideInInspector]
    public bool IsJiaoYanHid
    {
        set
        {
            _IsJiaoYanHid = value;
            pcvr.IsJiaoYanHid = _IsJiaoYanHid;
        }
        get
        {
            return _IsJiaoYanHid;
        }
    }
    #endregion

    /// <summary>
    /// 获取IO板的信息.
    /// </summary>
    void GetMessage()
    {
        if (CheckReadCountIsLock())
        {
            return;
        }

        byte[] buffer = MyCOMDevice.ComThreadClass.ReadByteMsg;
        if (CheckGetMsgInfoIsError(buffer))
        {
            return;
        }

        if (IsJiaoYanHid && IsJiaoYanJiaMiCoreDt)
        {
            CheckHidJiaMiXinPian(buffer);
        }
        CheckIsPlayerActivePcvr();
        KeyProcess(buffer);

        if (HardWareTest.GetInstance() != null)
        {
            HardWareTest.GetInstance().CheckReadComMsg(buffer);
        }
    }

    /// <summary>
    /// 循环检测收到IO板的信息.
    /// </summary>
    void KeyProcess(byte[] buffer)
    {
        //game coinInfo
        PlayerCoinHidArray[0] = buffer[18] & 0x0f;
        PlayerCoinHidArray[1] = (buffer[18] & 0xf0) >> 4;
        PlayerCoinHidArray[2] = buffer[19] & 0x0f;
        PlayerCoinHidArray[3] = (buffer[19] & 0xf0) >> 4;
        CheckPlayerCoinInfo(PlayerCoinEnum.player01);
        CheckPlayerCoinInfo(PlayerCoinEnum.player02);
        CheckPlayerCoinInfo(PlayerCoinEnum.player03);
        CheckPlayerCoinInfo(PlayerCoinEnum.player04);

        UpdateDianWeiQiDt(buffer);

        if (!CheckAnJianInfoIsError(buffer[41]))
        {
            UpdateAnJianLbDt(buffer);
            CheckKaiFangAnJianInfo(buffer[33]);
        }

        CaiPiaoPrintState caiPiaoPrintSt01 = (CaiPiaoPrintState)buffer[44];
        //CaiPiaoPrintState caiPiaoPrintSt02 = (CaiPiaoPrintState)buffer[44];
        OnReceiveCaiPiaoJiPrintState(caiPiaoPrintSt01, CaiPiaoJi.Num01);
        //CheckCaiPiaoJiPrintState(caiPiaoPrintSt02, CaiPiaoJi.Num02);
    }

    /// <summary>
    /// 玩家是否激活游戏.
    /// </summary>
    [HideInInspector]
    public bool IsPlayerActivePcvr = true;
    float TimeLastActivePcvr;
    /// <summary>
    /// 检测硬件是否被激活.
    /// 动态调整串口通信速度.
    /// </summary>
	void CheckIsPlayerActivePcvr()
    {
        if (Application.loadedLevel >= 1)
        {
            IsPlayerActivePcvr = true;
            return;
        }

        if (!IsPlayerActivePcvr)
        {
            return;
        }

        if (Time.realtimeSinceStartup - TimeLastActivePcvr > 60f)
        {
            IsPlayerActivePcvr = false;
        }
    }

    /// <summary>
    /// 激活pcvr.
    /// </summary>
    public void SetIsPlayerActivePcvr()
    {
        if (!pcvr.bIsHardWare)
        {
            return;
        }
        IsPlayerActivePcvr = true;
        TimeLastActivePcvr = Time.realtimeSinceStartup;
    }

    /// <summary>
    /// 获取是否可以继续打印彩票.
    /// </summary>
    public bool GetIsCanPrintCaiPiao(CaiPiaoJi indexCaiPiaoJi)
    {
        if (CaiPiaoCountPrint[(int)indexCaiPiaoJi] <= 0)
        {
            //目前需要打印彩票为0时,可以继续开启打印彩票.
            return true;
        }

        if (CaiPiaoPrintFailedCount[(int)indexCaiPiaoJi] >= 3)
        {
            //目前打印彩票失败次数大于3时,可以继续开启打印彩票(认为彩票机没有彩票了).
            return true;
        }
        return false;
    }

    /// <summary>
    /// 设置彩票机打印命令.
    /// </summary>
    public void SetCaiPiaoPrintCmd(CaiPiaoPrintCmd printCmd, CaiPiaoJi indexCaiPiaoJi, int caiPiaoCount)
    {
        Debug.Log("SetCaiPiaoPrintState -> printCmd " + printCmd + ", indexCaiPiaoJi " + indexCaiPiaoJi + ", caiPiaoCount " + caiPiaoCount);
        CaiPiaoPrintCmdVal[(int)indexCaiPiaoJi] = printCmd;
        if (printCmd == CaiPiaoPrintCmd.QuanPiaoPrint || printCmd == CaiPiaoPrintCmd.BanPiaoPrint)
        {
            CaiPiaoCountPrint[(int)indexCaiPiaoJi] = caiPiaoCount;
            CaiPiaoJiPrintStArray[(int)indexCaiPiaoJi] = CaiPiaoPrintState.Null;
        }
    }

    /// <summary>
    /// 收到彩票打印状态信息.
    /// </summary>
    void OnReceiveCaiPiaoJiPrintState(CaiPiaoPrintState printSt, CaiPiaoJi indexCaiPiaoJi)
    {
        switch (printSt)
        {
            case CaiPiaoPrintState.WuXiao:
                {
                    if (CaiPiaoJiPrintStArray[(int)indexCaiPiaoJi] != CaiPiaoPrintState.WuXiao)
                    {
                        Debug.Log("CaiPiaoJi_" + indexCaiPiaoJi + " -> print wuXiao!");
                    }

                    if (CaiPiaoCountPrint[(int)indexCaiPiaoJi] > 0 && CaiPiaoPrintFailedCount[(int)indexCaiPiaoJi] <= 3)
                    {
                        SetCaiPiaoPrintCmd(CaiPiaoPrintCmd.QuanPiaoPrint, indexCaiPiaoJi, CaiPiaoCountPrint[(int)indexCaiPiaoJi]);
                    }
                    break;
                }
            case CaiPiaoPrintState.Succeed:
                {
                    Debug.Log("CaiPiaoJi_" + indexCaiPiaoJi + " -> print succeed!");
                    SetCaiPiaoPrintCmd(CaiPiaoPrintCmd.StopPrint, indexCaiPiaoJi, 0);
                    if (CaiPiaoJiPrintStArray[(int)indexCaiPiaoJi] != CaiPiaoPrintState.Succeed)
                    {
                        CaiPiaoCountPrint[(int)indexCaiPiaoJi] -= 1;
                        InputEventCtrl.GetInstance().OnCaiPiaJiChuPiao(indexCaiPiaoJi);
                    }
                    CaiPiaoPrintFailedCount[(int)indexCaiPiaoJi] = 0;
                    break;
                }
            case CaiPiaoPrintState.Failed:
                {
                    Debug.Log("CaiPiaoJi_" + indexCaiPiaoJi + " -> print failed! failedCount " + CaiPiaoPrintFailedCount[(int)indexCaiPiaoJi]);
                    SetCaiPiaoPrintCmd(CaiPiaoPrintCmd.StopPrint, indexCaiPiaoJi, 0);
                    CaiPiaoPrintFailedCount[(int)indexCaiPiaoJi]++;
                    if (CaiPiaoPrintFailedCount[(int)indexCaiPiaoJi] > 3)
                    {
                        //彩票机无票了.
                        InputEventCtrl.GetInstance().OnCaiPiaJiWuPiao(indexCaiPiaoJi);
                    }
                    break;
                }
        }
        CaiPiaoJiPrintStArray[(int)indexCaiPiaoJi] = printSt;
    }

    /// <summary>
    /// 玩家币值索引.
    /// </summary>
    public enum PlayerCoinEnum
    {
        player01 = 0,
        player02 = 1,
        player03 = 2,
        player04 = 3,
    }

    /// <summary>
    /// hid币值信息.
    /// </summary>
    int[] PlayerCoinHidArray = new int[4];
    /// <summary>
    /// 玩家币值信息.
    /// </summary>
    [HideInInspector]
    public int[] PlayerCoinArray = new int[4];
    /// <summary>
    /// 减币.
    /// </summary>
    public void SubPlayerCoin(int subNum, PlayerCoinEnum indexPlayer)
    {
        int indexVal = (int)indexPlayer;
        if (PlayerCoinArray[indexVal] >= subNum)
        {
            PlayerCoinArray[indexVal] -= subNum;
        }
    }

    /// <summary>
    /// 检测玩家的投币信息.
    /// </summary>
    void CheckPlayerCoinInfo(PlayerCoinEnum indexPlayer)
    {
        int indexVal = (int)indexPlayer;
        if (PlayerCoinHidArray[indexVal] > 0)
        {
            if (!IsCleanHidCoinArray[indexVal])
            {
                IsCleanHidCoinArray[indexVal] = true;
                PlayerCoinArray[indexVal] += PlayerCoinHidArray[indexVal];
                Debug.Log(indexPlayer + " insert coin, coinNum -> " + PlayerCoinArray[indexVal]);
            }
        }
        else
        {
            IsCleanHidCoinArray[indexVal] = false;
        }
    }

    /// <summary>
    /// 电位器数据列表.
    /// </summary>
    [HideInInspector]
    public uint[] DianWeiQiDtArray = new uint[8];
    /// <summary>
    /// 检测ADKey是否错误.
    /// </summary>
    public bool CheckADKeyIsError(byte buffer)
    {
        //AD信息无效标记 2、5、8位依次位010
        if ((buffer & 0x02) == 0x02
            || (buffer & 0x10) != 0x10
            || (buffer & 0x80) == 0x80)
        {
            Debug.LogWarning("UpdateDianWeiQiDt -> ADKey was wrong! buffer_46 " + buffer.ToString("X2"));
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取电位器数据信息.
    /// </summary>
    public uint GetDianWeiQiDt(byte gaoWei, byte diWei)
    {
        return (((uint)gaoWei & 0x0f) << 8) + diWei;
    }

    /// <summary>
    /// 更新电位器数据信息.
    /// </summary>
    void UpdateDianWeiQiDt(byte[] buffer)
    {
        if (CheckADKeyIsError(buffer[46]))
        {
            return;
        }

        DianWeiQiDtArray[0] = GetDianWeiQiDt(buffer[2], buffer[3]);
        DianWeiQiDtArray[1] = GetDianWeiQiDt(buffer[4], buffer[5]);
        DianWeiQiDtArray[2] = GetDianWeiQiDt(buffer[6], buffer[7]);
        DianWeiQiDtArray[3] = GetDianWeiQiDt(buffer[8], buffer[9]);
        DianWeiQiDtArray[4] = GetDianWeiQiDt(buffer[10], buffer[11]);
        DianWeiQiDtArray[5] = GetDianWeiQiDt(buffer[12], buffer[13]);
        DianWeiQiDtArray[6] = GetDianWeiQiDt(buffer[14], buffer[15]);
        DianWeiQiDtArray[7] = GetDianWeiQiDt(buffer[16], buffer[17]);
    }
    
    int ReadCountLock = 0;
    /// <summary>
    /// 检测ReadCountLock.
    /// </summary>
    /// <returns></returns>
    bool CheckReadCountIsLock()
    {
        int readCount = MyCOMDevice.ComThreadClass.ReadCountLock;
        if (readCount == ReadCountLock)
        {
            return true;
        }
        ReadCountLock = readCount;
        //Debug.Log("CheckReadCountIsLock -> readCount " + readCount);
        return false;
    }
    
    /// <summary>
    /// 检测获取的IO信息是否错误.
    /// </summary>
    public bool CheckGetMsgInfoIsError(byte[] buffer)
    {
        if (!MyCOMDevice.GetInstance().IsFindDeviceDt)
        {
            return true;
        }

        bool isErrorMsg = false;
        if (buffer[0] != ReadHead_1)
        {
            isErrorMsg = true;
            Debug.LogWarning("CheckGetMsgInfo -> readHead01_buffer_00 was wrong! head01 " + buffer[0].ToString("X2"));
        }

        if (buffer[1] != ReadHead_2)
        {
            isErrorMsg = true;
            Debug.LogWarning("CheckGetMsgInfo -> readHead02_buffer_01 was wrong! head02 " + buffer[1].ToString("X2"));
        }

        if (buffer[58] != EndRead_1)
        {
            isErrorMsg = true;
            Debug.LogWarning("CheckGetMsgInfo -> readEnd01_buffer_58 was wrong! end01 " + buffer[58].ToString("X2"));
        }

        if (buffer[59] != EndRead_2)
        {
            isErrorMsg = true;
            Debug.LogWarning("CheckGetMsgInfo -> readEnd02_buffer_59 was wrong! end02 " + buffer[59].ToString("X2"));
        }

        if (IsJiaoYanHid)
        {
            if (buffer[45] == 0xff || buffer[45] == 0x00 || (buffer[45] & 0x10) != 0x10)
            {
                isErrorMsg = true;
                Debug.LogWarning("CheckGetMsgInfo -> buffer_45 was wrong! val " + buffer[45].ToString("X2"));
            }
        }

        //校验位1 位号6~55的疑惑校验值、初始校验异或值为0x38，不包含53自身
        byte jiaoYanVal = 0x38;
        for (int i = 6; i <= 51; i++)
        {
            if (i != 53)
            {
                jiaoYanVal ^= buffer[i];
            }
        }

        if (jiaoYanVal != buffer[53])
        {
            isErrorMsg = true;
            Debug.LogWarning("CheckGetMsgInfo -> jiaoYanVal01_buffer_53 was wrong! key " + buffer[53].ToString("X2"));
        }

        //数据校验位2	数据位5~49的异或值、初始异或值为0x95，不包23自身
        jiaoYanVal = 0x95;
        for (int i = 5; i <= 49; i++)
        {
            if (i != 23)
            {
                jiaoYanVal ^= buffer[i];
            }
        }

        if (jiaoYanVal != buffer[23])
        {
            isErrorMsg = true;
            Debug.LogWarning("CheckGetMsgInfo -> jiaoYanVal02_buffer_23 was wrong! key " + buffer[23].ToString("X2"));
        }

        //全包校验	异或初值0x36、0~59都包含, 不包55自身
        jiaoYanVal = 0x36;
        for (int i = 0; i <= 59; i++)
        {
            if (i != 55)
            {
                jiaoYanVal ^= buffer[i];
            }
        }

        if (jiaoYanVal != buffer[55])
        {
            isErrorMsg = true;
            Debug.LogWarning("CheckGetMsgInfo -> jiaoYanValQuanBao_buffer_55 was wrong! key " + buffer[55].ToString("X2"));
        }

        if (isErrorMsg)
        {
            string readInfo = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                readInfo += buffer[i].ToString("X2") + " ";
            }
            Debug.LogWarning("readMsg: " + readInfo);
        }
        return isErrorMsg;
    }

    /// <summary>
    /// 设置继电器的工作命令.
    /// </summary>
    public void SetJiDianQiCmd(byte indexVal, JiDianQiCmd cmd)
    {
        pcvr.GetInstance().mPcvrTXManage.JiDianQiCmdArray[indexVal] = cmd;
    }

    /// <summary>
    /// 获取继电器控制命令.
    /// </summary>
    byte GetJiDianQiCmd()
    {
        byte jiDianQiCmd = 0x00;
        switch (JiDianQiCmdArray[0])
        {
            case JiDianQiCmd.Close:
                {
                    jiDianQiCmd |= 0xa0;
                    break;
                }
            case JiDianQiCmd.Open:
                {
                    jiDianQiCmd |= 0x50;
                    break;
                }
        }

        switch (JiDianQiCmdArray[1])
        {
            case JiDianQiCmd.Close:
                {
                    jiDianQiCmd |= 0x0a;
                    break;
                }
            case JiDianQiCmd.Open:
                {
                    jiDianQiCmd |= 0x05;
                    break;
                }
        }
        return jiDianQiCmd;
    }

    #region PCVR_BT_EVENT
    public enum AnJianIndex
    {
        Null = 0,
        bt01 = 1, //按键1
        bt02 = 2,
        bt03 = 3,
        bt04 = 4,
        bt05 = 5,
        bt06 = 6,
        bt07 = 7,
        bt08 = 8,
        bt09 = 9,
        bt10 = 10,
        bt11 = 11,
        bt12 = 12,
        bt13 = 13,
        bt14 = 14,
        bt15 = 15, //按键15
    }

    class AnJianDt
    {
        /// <summary>
        /// 按键索引
        /// </summary>
        public AnJianIndex IndexAnJian = AnJianIndex.Null; //按键索引.
        /// <summary>
        /// 有效数据
        /// </summary>
        public byte YouXiaoDt = 21; //有效数据.
        /// <summary>
        /// 按键数据
        /// </summary>
        public byte AnJianVal = 20; //按键数据.
        /// <summary>
        /// 有效数据检测01
        /// </summary>
        public byte YouXiao_01 = 0x10; //有效按键数据检测01
        /// <summary>
        /// 有效数据检测02
        /// </summary>
        public byte YouXiao_02 = 0x40; //有效按键数据检测02
        /// <summary>
        /// 按键检测数据
        /// </summary>
        public byte AnJianKey_01 = 0x00; //按键检测数据01.
        /// <summary>
        /// 按键检测数据
        /// </summary>
        public byte AnJianKey_02 = 0x00; //按键检测数据02.
        /// <summary>
        /// 按键数据文本索引
        /// </summary>
        public byte IndexAnJianTx = 0; //按键数据文本索引.
        public AnJianDt(AnJianIndex indexAnJian, byte youXiaoDt, byte anJianVal, byte youXiao_01, byte youXiao_02, byte anJianKey_01, byte anJianKey_02)
        {
            IndexAnJian = indexAnJian;
            YouXiaoDt = youXiaoDt;
            AnJianVal = anJianVal;
            YouXiao_01 = youXiao_01;
            YouXiao_02 = youXiao_02;
            AnJianKey_01 = anJianKey_01;
            AnJianKey_02 = anJianKey_02;
        }
    }
    /// <summary>
    /// 按键状态.
    /// </summary>
    byte[] AnJianState = new byte[15];
    /// <summary>
    /// 检测按键状态.
    /// </summary>
    /// <param name="anJianDtVal"></param>
    void CheckAnJianDt(AnJianDt anJianDtVal)
    {
        //test
        //if (anJianDtVal.IndexAnJian != AnJianIndex.bt03)
        //{
        //    return;
        //}
        //test

        byte indexVal = (byte)anJianDtVal.IndexAnJian;
        indexVal -= 1;
        if ((anJianDtVal.YouXiaoDt & anJianDtVal.YouXiao_01) == anJianDtVal.YouXiao_01 && (anJianDtVal.YouXiaoDt & anJianDtVal.YouXiao_02) != anJianDtVal.YouXiao_02)
        {
            //按键有效位01.
            if ((anJianDtVal.AnJianVal & anJianDtVal.AnJianKey_01) == anJianDtVal.AnJianKey_01 && AnJianState[indexVal] == 0)
            {
                AnJianState[indexVal] = 1;
                OnClickPcvrBtEvent(anJianDtVal.IndexAnJian, InputEventCtrl.ButtonState.UP);
                Debug.Log(anJianDtVal.IndexAnJian + "-UP: YouXiaoDt " + anJianDtVal.YouXiaoDt.ToString("X2") + ", AnJianVal " + anJianDtVal.AnJianVal.ToString("X2") + ", YouXiao_01 " + anJianDtVal.YouXiao_01.ToString("X2") + ", AnJianKey_01 " + anJianDtVal.AnJianKey_01.ToString("X2"));
            }
            else if ((anJianDtVal.AnJianVal & anJianDtVal.AnJianKey_01) == 0x00 && AnJianState[indexVal] == 1)
            {
                AnJianState[indexVal] = 0;
                OnClickPcvrBtEvent(anJianDtVal.IndexAnJian, InputEventCtrl.ButtonState.DOWN);
                Debug.Log(anJianDtVal.IndexAnJian + "-DOWN: YouXiaoDt " + anJianDtVal.YouXiaoDt.ToString("X2") + ", AnJianVal " + anJianDtVal.AnJianVal.ToString("X2") + ", YouXiao_01 " + anJianDtVal.YouXiao_01.ToString("X2") + ", AnJianKey_01 " + anJianDtVal.AnJianKey_01.ToString("X2"));
            }
        }

        if ((anJianDtVal.YouXiaoDt & anJianDtVal.YouXiao_01) != anJianDtVal.YouXiao_01 && (anJianDtVal.YouXiaoDt & anJianDtVal.YouXiao_02) == anJianDtVal.YouXiao_02)
        {
            //按键有效位02.
            if ((anJianDtVal.AnJianVal & anJianDtVal.AnJianKey_02) == anJianDtVal.AnJianKey_02 && AnJianState[indexVal] == 0)
            {
                AnJianState[indexVal] = 1;
                OnClickPcvrBtEvent(anJianDtVal.IndexAnJian, InputEventCtrl.ButtonState.UP);
                Debug.Log(anJianDtVal.IndexAnJian + "-UP: YouXiaoDt " + anJianDtVal.YouXiaoDt.ToString("X2") + ", AnJianVal " + anJianDtVal.AnJianVal.ToString("X2") + ", YouXiao_02 " + anJianDtVal.YouXiao_02.ToString("X2") + ", AnJianKey_02 " + anJianDtVal.AnJianKey_02.ToString("X2"));
            }
            else if ((anJianDtVal.AnJianVal & anJianDtVal.AnJianKey_02) == 0x00 && AnJianState[indexVal] == 1)
            {
                AnJianState[indexVal] = 0;
                OnClickPcvrBtEvent(anJianDtVal.IndexAnJian, InputEventCtrl.ButtonState.DOWN);
                Debug.Log(anJianDtVal.IndexAnJian + "-DOWN: YouXiaoDt " + anJianDtVal.YouXiaoDt.ToString("X2") + ", AnJianVal " + anJianDtVal.AnJianVal.ToString("X2") + ", YouXiao_02 " + anJianDtVal.YouXiao_02.ToString("X2") + ", AnJianKey_02 " + anJianDtVal.AnJianKey_02.ToString("X2"));
            }
        }
    }

    /// <summary>
    /// 检测按键有效值数据是否错误.
    /// </summary>
    public bool CheckAnJianInfoIsError(byte buffer)
    {
        //键值有效位 2、3、5、7分别是1101
        if ((buffer & 0x02) != 0x02
            || (buffer & 0x04) != 0x04
            || (buffer & 0x10) == 0x10
            || (buffer & 0x40) != 0x40)
        {
            Debug.LogWarning("UpdateAnJianLbDt -> btKey was wrong! buffer_41 is " + buffer.ToString("X2"));
            return true;
        }
        return false;
    }

    /// <summary>
    /// 检测开放按键状态.
    /// </summary>
    void CheckKaiFangAnJianInfo(byte buffer)
    {
        //按键11（彩票3）
        if ((buffer & 0x01) == 0x01 && AnJianState[10] == 0)
        {
            AnJianState[10] = 1;
            OnClickPcvrBtEvent(AnJianIndex.bt11, InputEventCtrl.ButtonState.UP);
        }
        else if ((buffer & 0x01) == 0x00 && AnJianState[10] == 1)
        {
            AnJianState[10] = 0;
            OnClickPcvrBtEvent(AnJianIndex.bt11, InputEventCtrl.ButtonState.DOWN);
        }

        //按键12（彩票4）
        if ((buffer & 0x02) == 0x02 && AnJianState[11] == 0)
        {
            AnJianState[11] = 1;
            OnClickPcvrBtEvent(AnJianIndex.bt12, InputEventCtrl.ButtonState.UP);
        }
        else if ((buffer & 0x02) == 0x00 && AnJianState[11] == 1)
        {
            AnJianState[11] = 0;
            OnClickPcvrBtEvent(AnJianIndex.bt12, InputEventCtrl.ButtonState.DOWN);
        }

        //按键13（编码A）
        if ((buffer & 0x04) == 0x04 && AnJianState[12] == 0)
        {
            AnJianState[12] = 1;
            OnClickPcvrBtEvent(AnJianIndex.bt13, InputEventCtrl.ButtonState.UP);
        }
        else if ((buffer & 0x04) == 0x00 && AnJianState[12] == 1)
        {
            AnJianState[12] = 0;
            OnClickPcvrBtEvent(AnJianIndex.bt13, InputEventCtrl.ButtonState.DOWN);
        }

        //按键14（编码B）
        if ((buffer & 0x08) == 0x08 && AnJianState[13] == 0)
        {
            AnJianState[13] = 1;
            OnClickPcvrBtEvent(AnJianIndex.bt14, InputEventCtrl.ButtonState.UP);
        }
        else if ((buffer & 0x08) == 0x00 && AnJianState[13] == 1)
        {
            AnJianState[13] = 0;
            OnClickPcvrBtEvent(AnJianIndex.bt14, InputEventCtrl.ButtonState.DOWN);
        }

        //按键15（投币2）
        if ((buffer & 0x10) == 0x10 && AnJianState[14] == 0)
        {
            AnJianState[14] = 1;
            OnClickPcvrBtEvent(AnJianIndex.bt15, InputEventCtrl.ButtonState.UP);
        }
        else if ((buffer & 0x10) == 0x00 && AnJianState[14] == 1)
        {
            AnJianState[14] = 0;
            OnClickPcvrBtEvent(AnJianIndex.bt15, InputEventCtrl.ButtonState.DOWN);
        }
    }

    /// <summary>
    /// 当按键状态变化时.
    /// </summary>
    void OnClickPcvrBtEvent(AnJianIndex indexAnJian, InputEventCtrl.ButtonState btState)
    {
        switch (indexAnJian)
        {
            case AnJianIndex.bt01:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt01(btState);
                    break;
                }
            case AnJianIndex.bt02:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt02(btState);
                    break;
                }
            case AnJianIndex.bt03:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt03(btState);
                    break;
                }
            case AnJianIndex.bt04:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt04(btState);
                    break;
                }
            case AnJianIndex.bt05:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt05(btState);
                    break;
                }
            case AnJianIndex.bt06:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt06(btState);
                    break;
                }
            case AnJianIndex.bt07:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt07(btState);
                    break;
                }
            case AnJianIndex.bt08:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt08(btState);
                    break;
                }
            case AnJianIndex.bt09:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt09(btState);
                    break;
                }
            case AnJianIndex.bt10:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt10(btState);
                    break;
                }
            case AnJianIndex.bt11:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt11(btState);
                    break;
                }
            case AnJianIndex.bt12:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt12(btState);
                    break;
                }
            case AnJianIndex.bt13:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt13(btState);
                    break;
                }
            case AnJianIndex.bt14:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt14(btState);
                    break;
                }
            case AnJianIndex.bt15:
                {
                    InputEventCtrl.GetInstance().ClickPcvrBt15(btState);
                    break;
                }
        }
    }

    /// <summary>
    /// 更新按键数据状态.
    /// </summary>
    void UpdateAnJianLbDt(byte[] buffer)
    {
        //按键1（投币3）
        AnJianDt anJianDtVal = new AnJianDt(AnJianIndex.bt01, buffer[21], buffer[20], 0x10, 0x40, 0x04, 0x10);
        CheckAnJianDt(anJianDtVal);

        //按键2（投币4）
        anJianDtVal = new AnJianDt(AnJianIndex.bt02, buffer[22], buffer[24], 0x10, 0x40, 0x20, 0x80);
        CheckAnJianDt(anJianDtVal);

        //按键3（开始1）
        anJianDtVal = new AnJianDt(AnJianIndex.bt03, buffer[52], buffer[35], 0x10, 0x40, 0x20, 0x80);
        CheckAnJianDt(anJianDtVal);

        //按键4（开始2）
        anJianDtVal = new AnJianDt(AnJianIndex.bt04, buffer[51], buffer[38], 0x04, 0x10, 0x04, 0x10);
        CheckAnJianDt(anJianDtVal);

        //按键5（开始3）
        anJianDtVal = new AnJianDt(AnJianIndex.bt05, buffer[37], buffer[42], 0x02, 0x20, 0x08, 0x04);
        CheckAnJianDt(anJianDtVal);

        //按键6（开始4）
        anJianDtVal = new AnJianDt(AnJianIndex.bt06, buffer[39], buffer[43], 0x02, 0x80, 0x01, 0x02);
        CheckAnJianDt(anJianDtVal);

        //按键7（设置）
        anJianDtVal = new AnJianDt(AnJianIndex.bt07, buffer[36], buffer[40], 0x04, 0x10, 0x04, 0x10);
        CheckAnJianDt(anJianDtVal);

        //按键8（移动）
        anJianDtVal = new AnJianDt(AnJianIndex.bt08, buffer[25], buffer[27], 0x10, 0x40, 0x02, 0x10);
        CheckAnJianDt(anJianDtVal);

        //按键9（彩票1）
        anJianDtVal = new AnJianDt(AnJianIndex.bt09, buffer[28], buffer[32], 0x01, 0x80, 0x04, 0x20);
        CheckAnJianDt(anJianDtVal);

        //按键10（彩票2）
        anJianDtVal = new AnJianDt(AnJianIndex.bt10, buffer[34], buffer[29], 0x01, 0x80, 0x01, 0x08);
        CheckAnJianDt(anJianDtVal);
    }
    #endregion

    float TimeLastJiaoYanFailed = 0f;
    string InfoJiaoYanFailed = "";
    void OnGUI()
    {
        //IsJiaMiJiaoYanFailed = true; //test
        if (IsJiaMiJiaoYanFailed)
        {
            if (Time.time - TimeLastJiaoYanFailed > 0.05f)
            {
                InfoJiaoYanFailed = "io-pcvr-jmjysb::";
                int length = (Screen.width * Screen.height) / 300;
                for (int i = 0; i < length; i++)
                {
                    InfoJiaoYanFailed += Random.Range(0x00, 0xff).ToString("X2") + " ";
                }
                TimeLastJiaoYanFailed = Time.time;
            }
            //加密芯片校验失败后的提示信息.
            GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), InfoJiaoYanFailed);
        }
    }
}