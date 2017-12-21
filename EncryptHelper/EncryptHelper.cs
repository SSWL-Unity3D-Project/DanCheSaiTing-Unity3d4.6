using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

public class EncryptHelper : MonoBehaviour
{
	static TripleDESCryptoServiceProvider tri_des;
	static ICryptoTransform ict;
	public static void InitTripleInfo()
	{
		tri_des = new TripleDESCryptoServiceProvider();
		tri_des.KeySize = 192;          // 3DES 用 168-bit 的 key，其它 24-bit 为校验位
		tri_des.Key = SLUSBXHid.AuthKey; // 这里用 AuthKey 的数据填写
		tri_des.Mode = CipherMode.ECB;  // 只有 8 个 byte，模式无所谓，就用最简单的 ECB
		tri_des.Padding = PaddingMode.None;  // 刚好 8 个 byte，无须补齐
		ict = tri_des.CreateDecryptor();  // 创建一个解密器
	}

	public static byte[] TripleDesDecryptIOData(byte[] iodata)
	{
		return ict.TransformFinalBlock(iodata, 0, 8); //解密后的数据
	}
}