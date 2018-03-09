using UnityEngine;
using System.Collections;

public class TriggerYanHua : MonoBehaviour
{
    public GameObject[] YanHuaPrefabArray;
    public Transform[] SpawnPointArray;
    int MaxYanHua = 1;
    float TimeYanHuaVal = 0f;
    //bool IsInitSpawnYanHua;
    public void InitSpawnYanHua()
    {
        MaxYanHua = SpawnPointArray.Length;
        if (MaxYanHua <= 0)
        {
            return;
        }

        if (Time.time - TimeYanHuaVal < 1f)
        {
            return;
        }
        TimeYanHuaVal = Time.time;
        //if (IsInitSpawnYanHua)
        //{
        //    return;
        //}
        //IsInitSpawnYanHua = true;
        StartCoroutine(LoopSpawnYanHua());
    }

    IEnumerator LoopSpawnYanHua()
    {
        int countNum = 0;
        int indexYanHua = 0;
        int indexYanHuaPoint = 0;
        bool isSpawnYanHua = true;
        do
        {
            indexYanHua = countNum % YanHuaPrefabArray.Length;
            indexYanHuaPoint = countNum % SpawnPointArray.Length;
            Instantiate(YanHuaPrefabArray[indexYanHua], SpawnPointArray[indexYanHuaPoint].position, SpawnPointArray[indexYanHuaPoint].rotation);
            countNum++;
            if (countNum >= MaxYanHua)
            {
                isSpawnYanHua = false;
                yield break;
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        }
        while (isSpawnYanHua);
    }
}