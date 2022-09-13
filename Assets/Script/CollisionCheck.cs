using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    [Header("判定する対象のタグ")] public string colTag;

    private bool isCollision = false;
    private bool isCollisionEnter, isCollisionStay, isCollisionExit;
    
    //接地判定メソッド
    //物理判定の更新毎に呼ぶ必要がある
    public bool IsOn()
    {
        if (isCollisionEnter || isCollisionStay)
        {
            isCollision = true;
        }
        else if (isCollisionExit)
        {
            isCollision = false;
        }
        isCollisionEnter = false;
        isCollisionStay = false;
        isCollisionExit = false;
        return isCollision;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == colTag)
        {
            isCollisionEnter = true;
            //Debug.Log("地面enter");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == colTag)
        {
            isCollisionStay = true;
            // Debug.Log("地面stay");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == colTag)
        {
            isCollisionExit = true;
           // Debug.Log("地面exit");
        }
    }
}
