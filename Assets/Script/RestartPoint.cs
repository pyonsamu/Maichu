using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartPoint : MonoBehaviour
{
    [Header("リスタートNUM")] public int num;
    [Header("プレイヤー接触判定")] public CollisionCheck playerCheck;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerCheck.IsOn())
        {
            if (GManager.instance != null)
            {
                GManager.instance.continueNum = num;
                //Destroy(gameObject);
            }
        }
    }
}
