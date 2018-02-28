using UnityEngine;
using System.Collections;

public class ServerLinkInfo : MonoBehaviour {

	UILabel InfoLabel;

	static ServerLinkInfo _Instance;
	public static ServerLinkInfo GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake () {
		_Instance = this;

		InfoLabel = GetComponent<UILabel>();
		gameObject.SetActive(false);
	}

	public void SetServerLinkInfo(string info)
	{
		if (info == null || info == "") {
			return;
		}
		InfoLabel.text = info;
		gameObject.SetActive(true);
	}

	public void HiddenServerLinkInfo()
	{
		if (!gameObject.activeSelf) {
			return;
		}
		gameObject.SetActive(false);
	}
}
