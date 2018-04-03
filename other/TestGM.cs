using UnityEngine;

public class TestGM : MonoBehaviour
{
    public Animator mAnimator;
    int mIndexVal = 0;
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            //MoveAmmo();

            mAnimator.gameObject.SetActive(true);
            mAnimator.ResetTrigger("IsPlay");
            mAnimator.SetTrigger("IsPlay");
            mIndexVal++;
        }
    }

    public float mAmmoSpeed = 150f;
    void MoveAmmo()
    {
        float ammoSpeed = mAmmoSpeed;
        float lobHeight = 3f;
        Vector3 posHit = transform.position + transform.forward * 15f;
        float lobTime = Vector3.Distance(transform.position, posHit) / ammoSpeed;

        GameObject ammoCore = transform.GetChild(0).gameObject;
        iTween.MoveBy(ammoCore, iTween.Hash("y", lobHeight,
                                            "time", 0.5f * lobTime,
                                            "easeType", iTween.EaseType.easeOutQuad));
        iTween.MoveBy(ammoCore, iTween.Hash("y", -lobHeight,
                                            "time", 0.5f * lobTime,
                                            "delay", 0.5f * lobTime,
                                            "easeType", iTween.EaseType.easeInCubic));

        Vector3[] posArray = new Vector3[2];
        posArray[0] = transform.position;
        posArray[1] = posHit;
        iTween.MoveTo(gameObject, iTween.Hash("path", posArray,
                                           "time", lobTime,
                                           "orienttopath", true,
                                           "easeType", iTween.EaseType.linear,
                                           "oncomplete", "MoveAmmoOnCompelteITween"));
    }

    void MoveAmmoOnCompelteITween()
    {
        Debug.Log("***************");
    }
}