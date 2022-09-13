using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectCollision : MonoBehaviour
{

    [Header("これを踏んづけた時の跳ねる高さ")] public float boundHeight;
    
    /// <summary>
    /// このオブジェクトをプレイヤーが踏んだかどうか
    /// </summary>
    [HideInInspector] public bool playerStepOn;
    [HideInInspector] public string stepOnTag;
    [HideInInspector] public GameObject stepOnObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
