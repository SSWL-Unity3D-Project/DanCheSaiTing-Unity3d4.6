using UnityEngine;

public class TestGM : MonoBehaviour
{
    public Animator mAnimator;
    int mIndexVal = 0;
    public TerrainData m_TerrainData;
    public Terrain m_Terrain;
    public Vector3 m_TestPos;
    [Range(-100f, 100f)]
    public float m_AddHeight = 0f;
    float mRecordHeightVal = 0f;
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

        if (m_Terrain != null && m_TerrainData != null && mRecordHeightVal != m_AddHeight)
        {
            mRecordHeightVal = m_AddHeight;
            Vector3 terrainLocalPos = m_TestPos + m_Terrain.transform.position;
            Vector2 controlPos = new Vector2(terrainLocalPos.x / m_TerrainData.size.x * m_TerrainData.heightmapWidth, terrainLocalPos.z / m_TerrainData.size.z * m_TerrainData.heightmapHeight);
            float oldHeight = m_TerrainData.GetHeight((int)controlPos.x, (int)controlPos.y);
            float[,] newHeightData = new float[16, 16]; //new float[1, 1] { { (oldHeight + addHeight) / m_TerrainData.heightmapScale.y } };  
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    newHeightData[i, j] = (oldHeight + m_AddHeight) / m_TerrainData.heightmapScale.y;
                }
            }
            m_TerrainData.SetHeights((int)controlPos.x, (int)controlPos.y, newHeightData);
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