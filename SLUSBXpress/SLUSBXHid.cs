using UnityEngine;
using System.Collections;
using USBXpress_TestPanel;
using System.Text;
using System.Threading;

public class SLUSBXThreadClass
{
	public void Run()
	{
		do {
			SLUSBXHid.TimerTick();
			Thread.Sleep(20);
			if (SLUSBXHid.IsOnGameClose) {
				break;
			}
		} while (true);
		Debug.Log("exit the thread...");
	}
}

public class SLUSBXHid : MonoBehaviour {

	const int BytesReadRequest = 21;
	const int BytesWriteRequest = 18;
	static int BytesSucceed = 0;
	public static byte[] ReadBuf;
	public static byte[] WriteBuf;
	public static byte[] PcvrWriteByte;
	public static bool IsOpenDevice;
	public static bool IsFindDevice;
	static string DevicesName = "";
	public static bool IsCheckDevice;
	static byte LastWriteByte = 0x01;
	static float lastUpTime;
	public static bool IsOnGameClose;
	public static uint[] ReadInfo;
	public static bool IsFindDeviceDt;
	SLUSBXThreadClass SLUSBXThreadObj;
	Thread SLUSBXThread;
	public static byte[] AuthKey = {
		0x3e, 0x75, 0xbc, 0x60, 0x09, 0x3d, 0x17, 0xa2,
		0x62, 0x13, 0x24, 0xdc, 0xca, 0xbf, 0xcc, 0x4d
	};
	static byte[] WriteBuf_4 = {
		0xff, 0x01, 0x00, 0x00, 0x00, 0x00, 0x3e, 0x75, 0xbc,
		0x60, 0x09, 0x3d, 0x17, 0xa2, 0x02, 0x16, 0x00, 0x02
	};
	static byte[] WriteBuf_6 = {
		0xff, 0x01, 0x00, 0x00, 0x00, 0x00, 0x27, 0x8d, 0xd9,
		0xeb, 0xb5, 0x77, 0x26, 0xcf, 0x04, 0xb3, 0x00, 0x01
	};
	static byte[] ReadBuf_4 = {
		0x65, 0x3e, 0x75, 0xbc, 0x60, 0x09, 0x3d, 0x17, 0xa2,
		0x62, 0x13, 0x24, 0xdc, 0xca, 0xbf, 0xcc, 0x4d, 0x00, 0x02, 0x03, 0x02};
	static SLUSBXHid Instance;
	public static SLUSBXHid GetInstance()
	{
		if (Instance == null) {
			GameObject obj = new GameObject("_SLUSBXHid");
			//DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<SLUSBXHid>();
		}
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		EncryptHelper.InitTripleInfo();
		//TestDESDT();

		Instance = this;
		if (!pcvr.bIsHardWare) {
			return;
		}
		Invoke("DelayOpenSLUSBXLoop", 5f);
	}

	/*void FixedUpdate()
	{
		TimerTick();
	}*/
	
	float TimeLastUpdate;
	void Update()
	{
		if (Time.realtimeSinceStartup - TimeLastUpdate < 0.005f) {
			return;
		}
		TimeLastUpdate = Time.realtimeSinceStartup;
		TimerTick();
	}

	void DelayOpenSLUSBXLoop()
	{
		OpenDevice();
		/*SLUSBXThreadObj = new SLUSBXThreadClass();
		SLUSBXThread = new Thread(new ThreadStart(SLUSBXThreadObj.Run));
		SLUSBXThread.Start();*/
		//StartCoroutine(SLUSBXLoop());
	}

	IEnumerator SLUSBXLoop()
	{
		do {
			TimerTick();
			yield return new WaitForSeconds(0.008f);
			if (IsOnGameClose) {
				yield break;
			}
		} while (true);
	}

	void TestDESDT()
	{	
		//		byte[] TestDt = {0x65, 0x3e, 0x75, 0xbc, 0x60, 0x09, 0x3d, 0x17, 0xa2,
		//			0x62, 0x13, 0x24, 0xdc, 0xca, 0xbf, 0xcc, 0x4d, 0x00, 0x02, 0x03, 0x02};
		byte[] TestDt = {0x09, 0x9d, 0xf0, 0xfe, 0xe8, 0x75, 0x17, 0x1c, 0x99};
		byte[] readByte = EncryptHelper.TripleDesDecryptIOData(TestDt);
		string readStr = "解密前: ";
		for (int i = 0; i < TestDt.Length; i++) {
			readStr += TestDt[i].ToString("X2");
			readStr += " ";
		}
		Debug.Log(readStr);
		
		readStr = "解密后: ";
		for (int i = 0; i < readByte.Length; i++) {
			readStr += readByte[i].ToString("X2");
			readStr += " ";
		}
		Debug.Log(readStr);
	}
	
	void OnApplicationQuit()
	{
		IsOnGameClose = true;
		//SLUSBXThread.Abort();
		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_Close(SLUSBXpressDLL.hUSBDevice);
		if (SLUSBXpressDLL.Status == SLUSBXpressDLL.SI_SUCCESS) {
			Debug.Log("Close device...");
		}
	}

	void OpenDevice()
	{
		int DevNum = 0;
		StringBuilder DevStr = new StringBuilder(SLUSBXpressDLL.SI_MAX_DEVICE_STRLEN);
		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_GetNumDevices(ref DevNum);
		if (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS || DevNum <= 0) {
			return;
		}
		
		int HidIndex = 0;
		bool isFoundHid = false;
		for (int i = 0; i < DevNum; i++) {
			SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_GetProductString(i, DevStr, SLUSBXpressDLL.SI_RETURN_SERIAL_NUMBER);
			if (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS) {
				continue;
			}
			
			if (DevicesName == "") {
				isFoundHid = true;
				HidIndex = i;
				DevicesName = DevStr.ToString();
				break;
			}
			else if (DevicesName == DevStr.ToString()) {
				HidIndex = i;
				isFoundHid = true;
				break;
			}
		}
		
		if (isFoundHid) {
			Debug.Log("DevicesName: "+DevStr);
		}
		else {
			Debug.Log("not find hid...");
		}
		
		SLUSBXpressDLL.SI_SetTimeouts(10, 10);
		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_Open(HidIndex, ref SLUSBXpressDLL.hUSBDevice);
		if (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS) {
			Debug.Log("Error opening device: " + DevicesName + ". Application is aborting. Reset hardware and try again.");
			Application.Quit();
			return;
		}
		else {
			Debug.Log("open device...");
			IsOpenDevice = true;
			ReadInfo = new uint[BytesReadRequest];
			ReadBuf = new byte[BytesReadRequest];
			WriteBuf = new byte[BytesWriteRequest];
			PcvrWriteByte = new byte[BytesWriteRequest];
			WriteBuf[0] = 0xff;
			WriteBuf[1] = 0x01;
			WriteBuf[17] = 0x01;

			BytesSucceed = 0;
			// Send output data out to the board
			SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_Write(SLUSBXpressDLL.hUSBDevice, ref WriteBuf[0], BytesWriteRequest, ref BytesSucceed, 0);
			if ((BytesSucceed != BytesWriteRequest) || (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS)) {
				if (SLUSBXpressDLL.Status == SLUSBXpressDLL.SI_WRITE_TIMED_OUT) {
					return;
				}
				Debug.Log("Error writing to USB. Wrote " + BytesSucceed.ToString() + " of " + BytesWriteRequest.ToString() + " bytes. Application is aborting. Reset hardware and try again.");
				Debug.Log("Status " + SLUSBXpressDLL.Status);
				IsOpenDevice = false;
				Application.Quit();
				return;
			}
		}
	}

	static void FindDevice()
	{
		if (!IsOpenDevice) {
			return;
		}
		//clear out bytessucceed for the next read
		BytesSucceed = 0;
		//read data from the board
		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_Read(SLUSBXpressDLL.hUSBDevice, ref ReadBuf[0], BytesReadRequest, ref BytesSucceed, 0);
		if ((BytesSucceed != BytesReadRequest) || (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS)) {
			if (SLUSBXpressDLL.Status == SLUSBXpressDLL.SI_READ_TIMED_OUT) {
				return;
			}
			Debug.Log("Error reading to USB. Read " + BytesSucceed.ToString() + " of " + BytesReadRequest.ToString() + " bytes. Application is aborting. Reset hardware and try again.");
			Debug.Log("Status " + SLUSBXpressDLL.Status);
			IsOpenDevice = false;
			Application.Quit();
			return;
		}

		if (ReadBuf[0] == 0x65 && ReadBuf[20] == 0x01) {
			IsFindDevice = true;
			string readStr = "ReadBuf: ";
			for (int i = 0; i < BytesReadRequest; i++) {
				readStr += ReadBuf[i].ToString("X2");
				readStr += " ";
			}
			//Debug.Log(readStr);
			Debug.Log("find device...");
		}

		BytesSucceed = 0;
		// Send output data out to the board
		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_Write(SLUSBXpressDLL.hUSBDevice, ref WriteBuf[0], BytesWriteRequest, ref BytesSucceed, 0);
		if ((BytesSucceed != BytesWriteRequest) || (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS)) {
			if (SLUSBXpressDLL.Status == SLUSBXpressDLL.SI_WRITE_TIMED_OUT) {
				return;
			}
			Debug.Log("Error writing to USB. Wrote " + BytesSucceed.ToString() + " of " + BytesWriteRequest.ToString() + " bytes. Application is aborting. Reset hardware and try again.");
			Debug.Log("Status " + SLUSBXpressDLL.Status);
			IsOpenDevice = false;
			Application.Quit();
			return;
		}
	}

	static void CheckDevice()
	{
		if (IsCheckDevice) {
			return;
		}

		WriteBuf = WriteBuf_4;
		BytesSucceed = 0;
		// Send output data out to the board
		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_Write(SLUSBXpressDLL.hUSBDevice, ref WriteBuf[0], BytesWriteRequest, ref BytesSucceed, 0);
		if ((BytesSucceed != BytesWriteRequest) || (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS)) {
			if (SLUSBXpressDLL.Status == SLUSBXpressDLL.SI_WRITE_TIMED_OUT) {
				return;
			}
			Debug.Log("Error writing to USB. Wrote " + BytesSucceed.ToString() + " of " + BytesWriteRequest.ToString() + " bytes. Application is aborting. Reset hardware and try again.");
			Debug.Log("Status " + SLUSBXpressDLL.Status);
			IsOpenDevice = false;
			Application.Quit();
			return;
		}

		//clear out bytessucceed for the next read
		BytesSucceed = 0;
		//read data from the board
		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_Read(SLUSBXpressDLL.hUSBDevice, ref ReadBuf[0], BytesReadRequest, ref BytesSucceed, 0);
		if ((BytesSucceed != BytesReadRequest) || (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS)) {
			if (SLUSBXpressDLL.Status == SLUSBXpressDLL.SI_READ_TIMED_OUT) {
				return;
			}
			Debug.Log("Error reading to USB. Read " + BytesSucceed.ToString() + " of " + BytesReadRequest.ToString() + " bytes. Application is aborting. Reset hardware and try again.");
			Debug.Log("Status " + SLUSBXpressDLL.Status);
			IsOpenDevice = false;
			Application.Quit();
			return;
		}

		bool isFindDevice = true;
		for (int i = 0; i < BytesReadRequest; i++) {
			if (ReadBuf[i] != ReadBuf_4[i]) {
				isFindDevice = false;
				break;
			}
		}

		if (isFindDevice) {
			IsCheckDevice = true;
			Debug.Log("check device over...");
		}
		
		/*SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_FlushBuffers(SLUSBXpressDLL.hUSBDevice, 1, 1);
		if (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS) {
			Debug.Log("SI_FlushBuffers was wrong!");
			IsOpenDevice = false;
			Application.Quit();
			return;
		}*/
	}

	static void FindDeviceDt()
	{
		if (IsFindDeviceDt) {
			return;
		}
		
		WriteBuf = WriteBuf_6;
		BytesSucceed = 0;
		// Send output data out to the board
		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_Write(SLUSBXpressDLL.hUSBDevice, ref WriteBuf[0], BytesWriteRequest, ref BytesSucceed, 0);
		if ((BytesSucceed != BytesWriteRequest) || (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS)) {
			if (SLUSBXpressDLL.Status == SLUSBXpressDLL.SI_WRITE_TIMED_OUT) {
				return;
			}
			Debug.Log("Error writing to USB. Wrote " + BytesSucceed.ToString() + " of " + BytesWriteRequest.ToString() + " bytes. Application is aborting. Reset hardware and try again.");
			Debug.Log("Status " + SLUSBXpressDLL.Status);
			IsOpenDevice = false;
			Application.Quit();
			return;
		}
		
		//clear out bytessucceed for the next read
		BytesSucceed = 0;
		//read data from the board
		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_Read(SLUSBXpressDLL.hUSBDevice, ref ReadBuf[0], BytesReadRequest, ref BytesSucceed, 0);
		if ((BytesSucceed != BytesReadRequest) || (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS)) {
			if (SLUSBXpressDLL.Status == SLUSBXpressDLL.SI_READ_TIMED_OUT) {
				return;
			}
			Debug.Log("Error reading to USB. Read " + BytesSucceed.ToString() + " of " + BytesReadRequest.ToString() + " bytes. Application is aborting. Reset hardware and try again.");
			Debug.Log("Status " + SLUSBXpressDLL.Status);
			IsOpenDevice = false;
			Application.Quit();
			return;
		}

		if (ReadBuf[0] == 0x65) {
			IsFindDeviceDt = true;
			Debug.Log("FindDeviceDt over...");
		}
		
		/*SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_FlushBuffers(SLUSBXpressDLL.hUSBDevice, 1, 1);
		if (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS) {
			Debug.Log("SI_FlushBuffers was wrong!");
			IsOpenDevice = false;
			Application.Quit();
			return;
		}*/
	}

	public static void TimerTick()
	{
		if (!IsOpenDevice) {
			return;
		}

		if (!IsFindDeviceDt) {
			if (!IsFindDevice) {
				FindDevice();
				return;
			}
			
			if (!IsCheckDevice) {
				CheckDevice();
				return;
			}
			FindDeviceDt();
			return;
		}

		WriteBuf = PcvrWriteByte;
		WriteBuf[17] = (byte)(LastWriteByte == 0x01 ? 0x02 : 0x01);
		if (WriteBuf[17] == 0x02) {
			WriteBuf[14] = 0x02;
			WriteBuf[15] = 0;
			for (int i = 6; i < 14; i++) {
				WriteBuf[15] ^= WriteBuf[i];
			}
		}
		else {
			WriteBuf[17] = 0x01;
		}

		BytesSucceed = 0;
		// Send output data out to the board
		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_Write(SLUSBXpressDLL.hUSBDevice, ref WriteBuf[0], BytesWriteRequest, ref BytesSucceed, 0);
		if ((BytesSucceed != BytesWriteRequest) || (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS)) {
			if (SLUSBXpressDLL.Status == SLUSBXpressDLL.SI_WRITE_TIMED_OUT) {
				return;
			}
			Debug.Log("Error writing to USB. Wrote " + BytesSucceed.ToString() + " of " + BytesWriteRequest.ToString() + " bytes. Application is aborting. Reset hardware and try again.");
			Debug.Log("Status " + SLUSBXpressDLL.Status);
			IsOpenDevice = false;
			Application.Quit();
			return;
		}
		LastWriteByte = WriteBuf[17];

		//clear out bytessucceed for the next read
		BytesSucceed = 0;
		//read data from the board
		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_Read(SLUSBXpressDLL.hUSBDevice, ref ReadBuf[0], BytesReadRequest, ref BytesSucceed, 0);
		if ((BytesSucceed != BytesReadRequest) || (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS)) {
			if (SLUSBXpressDLL.Status == SLUSBXpressDLL.SI_READ_TIMED_OUT) {
				return;
			}
			Debug.Log("Error reading to USB. Read " + BytesSucceed.ToString() + " of " + BytesReadRequest.ToString() + " bytes. Application is aborting. Reset hardware and try again.");
			Debug.Log("Status " + SLUSBXpressDLL.Status);
			IsOpenDevice = false;
			Application.Quit();
			return;
		}
		
		if (ReadBuf[0] == 0x65) {
			/*string testReadBt = "解码前: ";
			for (int i = 0; i < 21; i++) {
				testReadBt += ReadBuf[i].ToString("X2");
				testReadBt += " ";
			}*/
			//StrTest2 = testReadBt;
			//Debug.Log(testReadBt);
			
			byte[] getDt = new byte[8];
			for (int i = 0; i < 8; i++) {
				getDt[i] = ReadBuf[i+1];
			}
			
			byte[] byteTmp = EncryptHelper.TripleDesDecryptIOData(getDt);
			byte[] byteReadTmp = ReadBuf;
			for (int i = 0; i < 8; i++) {
				byteReadTmp[i+1] = byteTmp[i];
			}
			
			//testReadBt = "解码后: ";
			for (int i = 0; i < 21; i++) {
				ReadInfo[i] = (uint)byteReadTmp[i];
				//testReadBt += ReadInfo[i].ToString("X2");
				//testReadBt += " ";
			}
			//Debug.Log("ReadInfo[0] "+ReadInfo[0]);
			//StrTest3 = testReadBt;
			//Debug.Log(testReadBt);

			//uint SteerValCur = ((ReadInfo[4]&0x03) << 8) + ReadInfo[3];
			//StrTest1 = "方向: "+SteerValCur+", byte[3] "+ReadInfo[3].ToString("X2")+", byte[4] "+ReadInfo[4].ToString("X2");
			//Debug.Log(StrTest1);
		}

		SLUSBXpressDLL.Status = SLUSBXpressDLL.SI_FlushBuffers(SLUSBXpressDLL.hUSBDevice, 1, 1);
		if (SLUSBXpressDLL.Status != SLUSBXpressDLL.SI_SUCCESS) {
			Debug.Log("SI_FlushBuffers was wrong!");
			IsOpenDevice = false;
			Application.Quit();
			return;
		}
	}

	/*static string StrTest1;
	static string StrTest2;
	static string StrTest3;
	void OnGUI()
	{
		GUI.Box(new Rect(0f, 0f, 700f, 30f), StrTest1);
		//GUI.Box(new Rect(0f, 30f, 700f, 30f), StrTest2);
		//GUI.Box(new Rect(0f, 60f, 700f, 30f), StrTest3);
	}*/
}