using UnityEngine;
using System.Collections;

public class XKGameTextCtrl : MonoBehaviour {
	public Texture TextureCH;
	public Texture TextureEN;
	public bool IsFixTexture;
	public Vector2 TextureVecCh;
	public Vector2 TextureVecEn;
	public UISpriteAnimation UISpAniCom;
	public string ChSpAni; //中文前缀.
	public string EnSpAni; //英文前缀.
	public UISprite UISpCom;
	public string ChSpName; //中文图名称.
	public string EnSpName; //英文图名称.
	public MeshRenderer MeshRenderCom;
	public Material Material_Ch;
	public Material Material_En;
	GameTextType GameTextVal = GlobalData.GameTextVal;
	// Use this for initialization
	void Start()
	{
		GameTextVal = GlobalData.GetGameTextMode();
		//GameTextVal = GameTextType.English; //test.
		//Debug.Log("GameTextVal "+GameTextVal);
		CheckGameUITexture();
		CheckUISpAniCom();
		CheckGameUISpCom();
		CheckMeshRenderCom();
	}

	void CheckGameUITexture()
	{
		if (TextureCH != null && TextureEN != null) {
			//改变UITexture的图片.
			UITexture uiTextureCom = GetComponent<UITexture>();
			switch (GameTextVal) {
			case GameTextType.Chinese:
				if (uiTextureCom != null) {
					uiTextureCom.mainTexture = TextureCH;
					if (IsFixTexture) {
						uiTextureCom.width = (int)TextureVecCh.x;
						uiTextureCom.height = (int)TextureVecCh.y;
					}
				}
				break;
				
			case GameTextType.English:
				if (uiTextureCom != null) {
					uiTextureCom.mainTexture = TextureEN;
					if (IsFixTexture) {
						uiTextureCom.width = (int)TextureVecEn.x;
						uiTextureCom.height = (int)TextureVecEn.y;
					}
				}
				break;
			}
		}
	}
	
	void CheckUISpAniCom()
	{
		if (UISpAniCom == null) {
			return;
		}
		
		switch (GameTextVal) {
		case GameTextType.Chinese:
			UISpAniCom.namePrefix = ChSpAni;
			break;
			
		case GameTextType.English:
			UISpAniCom.namePrefix = EnSpAni;
			break;
		}
	}
	
	void CheckGameUISpCom()
	{
		if (UISpCom == null) {
			return;
		}

		switch (GameTextVal) {
		case GameTextType.Chinese:
			UISpCom.spriteName = ChSpName;
			break;
			
		case GameTextType.English:
			UISpCom.spriteName = EnSpName;
			break;
		}
	}
	
	void CheckMeshRenderCom()
	{
		if (MeshRenderCom == null) {
			return;
		}
		
		switch (GameTextVal) {
		case GameTextType.Chinese:
			MeshRenderCom.material = Material_Ch;
			break;
			
		case GameTextType.English:
			MeshRenderCom.material = Material_En;
			break;
		}
	}
}