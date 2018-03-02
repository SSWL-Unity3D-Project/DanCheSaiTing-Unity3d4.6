//#define PRINT_IO_MSG
using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.IO.Ports;

public class MyCOMDevice : MonoBehaviour
{
    public class ComThreadClass
    {
        //string ThreadName;
        static SerialPort _SerialPort;
        //public static int BufLenRead = 60;
        //public static  int BufLenWrite = 50;
        public static int BufLenRead = 60 + 1; //test
        public static int BufLenWrite = 50 + 1; //test
        public static byte[] ReadByteMsg = new byte[BufLenRead];
        public static byte[] WriteByteMsg = new byte[BufLenWrite];
        int ReadTimeout = 0x07d0; //单位为毫秒.
        int WriteTimeout = 0x07d0; //单位为毫秒.
        public static string ComPortName = "COM1";
        public static int WriteCountLock;
        public static int ReadCountLock;

        public ComThreadClass(string name)
        {
            //ThreadName = name;
            OpenComPort();
        }

        /// <summary>
        /// 打开串口.
        /// </summary>
        public void OpenComPort()
        {
            if (_SerialPort != null)
            {
                return;
            }

            _SerialPort = new SerialPort(ComPortName, 57600, Parity.None, 8, StopBits.One);
            if (_SerialPort != null)
            {
                try
                {
                    if (_SerialPort.IsOpen)
                    {
                        _SerialPort.Close();
                        Debug.Log("Closing port, because it was already open!");
                    }
                    else
                    {
                        _SerialPort.ReadTimeout = ReadTimeout;
                        _SerialPort.WriteTimeout = WriteTimeout;
                        _SerialPort.Open();
                        if (_SerialPort.IsOpen)
                        {
                            if (_Instance != null)
                            {
                                _Instance.IsFindDeviceDt = true;
                            }
                            Debug.Log("COM open sucess");
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.Log("error:COM already opened by other PRG... " + exception);
                }
            }
            else
            {
                Debug.Log("Port == null");
            }
        }

        /// <summary>
        /// 线程运行函数.
        /// </summary>
		public void Run()
        {
            do
            {
                COMTxData();
                if (pcvr.IsJiaoYanHid)
                {
                    Thread.Sleep(500);
                }

                COMRxData();
                if (pcvr.IsJiaoYanHid)
                {
                    Thread.Sleep(500);
                }
                else
                {
                    Thread.Sleep(15);
                }
            }
            while (_SerialPort != null && _SerialPort.IsOpen);
            Debug.Log("Close run thead...");
        }

        /// <summary>
        /// 写串口数据.
        /// </summary>
        void COMTxData()
        {
            try
            {
#if PRINT_IO_MSG
                if (pcvr.IsJiaoYanHid)
                {
                    string writeInfo = "";
                    for (int i = 0; i < WriteByteMsg.Length; i++)
                    {
                        writeInfo += WriteByteMsg[i].ToString("X2") + " ";
                    }
                    Debug.Log("writMsg: " + writeInfo);
                }
#endif

                _SerialPort.Write(WriteByteMsg, 0, BufLenWrite);
                WriteCountLock++;

                if (pcvr.IsJiaoYanHid)
                {
                    byte[] miMaArray = new byte[4];
                    miMaArray[1] = WriteByteMsg[34]; //密码1
                    miMaArray[2] = WriteByteMsg[38]; //密码2
                    miMaArray[3] = WriteByteMsg[45]; //密码3
                    pcvrTXManage.CheckIsJiaoYanJiaMiCore(miMaArray);
                }
            }
            catch (Exception exception)
            {
                Debug.Log("Tx error:COM!!! " + exception);
            }
        }

        /// <summary>
        /// 读串口数据.
        /// </summary>
		void COMRxData()
        {
            try
            {
                long timeStart = DateTime.Now.Ticks;
                float timeOut = 1f;
                bool isTimeOut = false;

                bool isContinueRead = true;
                byte[] reagBuf = new byte[BufLenRead];
                int rvReadBytes = 0;
                int indexStart = 0;
                IsReadComDt = true;
                while (isContinueRead && !IsAppQuit)
                {
                    int len = BufLenRead - rvReadBytes;
                    byte[] reagBufTmp = new byte[len];
                    rvReadBytes += _SerialPort.Read(reagBufTmp, 0, len);
                    //Debug.Log("COMRxData -> rv " + rvReadBytes + ", len " + len);

                    for (int i = 0; i < rvReadBytes; i++)
                    {
                        if (i + indexStart < BufLenRead)
                        {
                            reagBuf[i + indexStart] = reagBufTmp[i];
                        }
                    }
                    indexStart = rvReadBytes;

                    float dTime = (DateTime.Now.Ticks - timeStart) / 10000000f;
                    if (dTime >= timeOut)
                    {
                        Debug.Log("COMRxData -> timeOut! dTime " + dTime + ", rvReadBytes " + rvReadBytes);
                        isContinueRead = false;
                        isTimeOut = true;
                        break;
                    }

                    if (rvReadBytes >= BufLenRead)
                    {
                        isContinueRead = false;
                        break;
                    }
                }
                ReadByteMsg = reagBuf;
                IsReadComDt = false;

#if PRINT_IO_MSG
                if (pcvr.IsJiaoYanHid)
                {
                    string readInfo = "";
                    for (int i = 0; i < rvReadBytes; i++)
                    {
                        readInfo += reagBuf[i].ToString("X2") + " ";
                    }
                    Debug.Log("readMsg: " + readInfo);
                }
#endif

                if (isTimeOut)
                {
                    return;
                }
                ReadCountLock++;
            }
            catch (Exception exception)
            {
                Debug.Log("Rx error:COM..." + exception);
                if (IsAppQuit)
                {
                    CloseComPort();
                }
            }
        }

        /// <summary>
        /// 关闭串口.
        /// </summary>
		public void CloseComPort()
        {
            if (_SerialPort == null || !_SerialPort.IsOpen)
            {
                return;
            }
            _SerialPort.Close();
            _SerialPort = null;
            Debug.Log("CloseComPort...");
        }
    }

    /// <summary>
    /// 串口线程控制.
    /// </summary>
    ComThreadClass mComThreadClass;
    /// <summary>
    /// 串口IO线程.
    /// </summary>
	Thread mComThreadIO;
    /// <summary>
    /// 是否找到串口设备.
    /// </summary>
    [HideInInspector]
    public bool IsFindDeviceDt;
    static bool IsAppQuit = false;
    static bool IsReadComDt = false;
    static MyCOMDevice _Instance;
    public static MyCOMDevice GetInstance()
    {
        if (_Instance == null)
        {
            GameObject obj = new GameObject("_MyCOMDevice");
            DontDestroyOnLoad(obj);
            _Instance = obj.AddComponent<MyCOMDevice>();
        }
        return _Instance;
    }

    // Use this for initialization
    void Start()
    {
        if (pcvr.bIsHardWare)
        {
            StartCoroutine(OpenComThread());
        }
    }

    /// <summary>
    /// 打开IO线程.
    /// </summary>
	IEnumerator OpenComThread()
    {
        if (mComThreadClass == null)
        {
            mComThreadClass = new ComThreadClass(ComThreadClass.ComPortName);
        }
        else
        {
            mComThreadClass.CloseComPort();
        }

        if (mComThreadIO != null)
        {
            CloseComThread();
        }
        yield return new WaitForSeconds(2f);

        if (mComThreadIO == null)
        {
            mComThreadIO = new Thread(new ThreadStart(mComThreadClass.Run));
            mComThreadIO.Start();
        }
    }

    /// <summary>
    /// 关闭IO线程.
    /// </summary>
	void CloseComThread()
    {
        try
        {
            if (mComThreadIO != null)
            {
                mComThreadIO.Abort();
                mComThreadIO = null;
            }
        }
        catch (Exception exception)
        {
            Debug.Log("CloseComThread -> ex: " + exception);
        }
    }

    /// <summary>
    /// 当程序关闭时.
    /// </summary>
	void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit...Com");
        try
        {
            IsAppQuit = true;
            CloseComThread();
            if (mComThreadClass != null && !IsReadComDt)
            {
                mComThreadClass.CloseComPort();
            }
        }
        catch (Exception exception)
        {
            Debug.Log("OnApplicationQuit -> ex: " + exception);
        }
    }
}