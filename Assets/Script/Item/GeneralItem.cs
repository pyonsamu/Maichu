using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralItem : MonoBehaviour
{
    [Header("加算するスコア")] public int score;
    [Header("加算するライフ")] public int life;
    [Header("プレイヤー接触判定")] public CollisionCheck playerCheck;

    [Header("アイテムSE")] public AudioClip itemSE;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCheck.IsOn())
        {
            if(GManager.instance != null)
            {
                GManager.instance.PlaySE(itemSE);
                GManager.instance.score += score;
                GManager.instance.life += life;
                Destroy(gameObject);
            }
        }
    }
}
