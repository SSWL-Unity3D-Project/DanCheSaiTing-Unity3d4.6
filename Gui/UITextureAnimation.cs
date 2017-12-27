using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class UITextureAnimation : MonoBehaviour
{
    public Texture mTexture;
    UITexture mUITexture;
    void Start()
    {
        mUITexture = GetComponent<UITexture>();
    }

    void Update()
    {
        if (mUITexture.mainTexture != mTexture)
        {
            mUITexture.mainTexture = mTexture;
        }
    }

    /// <summary>
    /// 动画事件回调.
    /// </summary>
    public void OnAnimationTrigger(int index)
    {
        //Debug.Log("OnAnimationTrigger -> index is " + index);
        //广播消息.
        SendMessage("OnAnimationEnvent", index);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (mUITexture == null)
        {
            mUITexture = GetComponent<UITexture>();
        }

        if (mUITexture.mainTexture != mTexture)
        {
            mUITexture.mainTexture = mTexture;
        }
    }
#endif
}