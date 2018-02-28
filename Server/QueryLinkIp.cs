using System.Collections.Generic;

public class QueryLinkIp
{
	public static List<string> IpList = new List<string>(){};
	static QueryLinkIp _Instance;
	public static QueryLinkIp GetInstance()
	{
		if(_Instance == null)
		{
			_Instance = new QueryLinkIp();
		}
		return _Instance;
	}

	public void CheckLinkIpArray()
	{
		string ip = "";
		for (int i = 1; i <= 256; i++) {
			ip = HandleJson.GetInstance().ReadFromFileXml("gameCheckIp.info", "IP_" + i.ToString());
			if(ip == null || ip == "")
			{
				break;
			}
			IpList.Add(ip);
			//UnityEngine.Debug.Log("ip_" + i + ": *** "+ ip);
		}
	}
}