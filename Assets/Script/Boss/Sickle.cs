using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sickle : MonoBehaviour
{
    public enum SickleOwner
    {
        Boss,
        Player,
        Nan
    }

    [HideInInspector]public SickleOwner owner = SickleOwner.Boss; //Boss, Player, Nan
    [Header("鎌のcolオブジェクト")] public GameObject arm_col;
    [Header("Xスピード")] public float speedX;
    [Header("Yスピード")] public float speedY;
    [Header("重力")] public float gravity;


    private ObjectCollision oc = null;
    private bool isFlying = false;
    private float posX,posY,vecX,vecY = 0.0f;
    private GameObject player = null;
    private bool isPlayerAttack = false;
    private string bossTag = "Boss";
    private string sickleTag = "Sickle";
    private string playerSickleTag = "PlayerSickle";



    // Start is called before the first frame update
    void Start()
    {
        oc = GetComponent<ObjectCollision>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (owner)
        {
            case SickleOwner.Boss:
                if (oc.playerStepOn)
                {
                    if(oc.stepOnTag == "Player")
                    {
                        arm_col.SetActive(false);
                        oc.playerStepOn = false;
                        oc.stepOnTag = null;
                        isFlying = true;
                        owner = SickleOwner.Nan;
                        transform.parent = null;
                        posY = transform.position.y;
                        vecY = speedY;
                        
                    }
                }
                break;
            case SickleOwner.Nan:
                if (isFlying)
                {
                    Debug.Log("fly");
                    this.transform.localEulerAngles = new Vector3(0.0f,0.0f, this.transform.localEulerAngles.z-30.0f);
                    vecY -= gravity;
                    transform.position = new Vector3(transform.position.x+speedX,transform.position.y+vecY,0.0f);
                    if(transform.position.y < posY)
                    {
                        transform.position = new Vector3(transform.position.x,posY+1.1f,transform.position.z);
                        transform.localEulerAngles = new Vector3(0.0f,0.0f,25.0f);
                        isFlying = false;
                    }
                }
                else
                {
                    arm_col.SetActive(true);
                    if (oc.playerStepOn)
                    {
                        if(oc.stepOnTag == "Player")
                        {
                            player = oc.stepOnObject;
                            owner = SickleOwner.Player;
                            transform.localScale = new Vector3(-1*player.transform.localScale.x, 1.0f, 1.0f);
                            transform.parent = oc.stepOnObject.transform;
                            transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                            transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                            tag = playerSickleTag;
                        }
                    }
                    
                }
                break;
            case SickleOwner.Player:
                if (player.GetComponent<Hero>().IsRoll())
                {
                    isPlayerAttack = true;
                    transform.GetChild(0).gameObject.SetActive(true);
                    GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    isPlayerAttack = false;
                    transform.GetChild(0).gameObject.SetActive(false);
                    GetComponent<SpriteRenderer>().enabled = false;
                }

                if (isPlayerAttack)
                {
                    transform.localEulerAngles = new Vector3(0.0f,0.0f,transform.localEulerAngles.z-30);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                }

                if (oc.playerStepOn) //Bossに切りかかった
                {
                    if (oc.stepOnTag == bossTag)
                    {
                        oc.stepOnObject.GetComponent<ObjectCollision>().playerStepOn = true;
                        oc.stepOnObject.GetComponent<ObjectCollision>().stepOnTag = tag;
                        oc.stepOnObject.GetComponent<ObjectCollision>().stepOnObject = gameObject;
                        
                        tag = sickleTag;
                        transform.GetChild(0).gameObject.SetActive(false);
                        GetComponent<SpriteRenderer>().enabled = false;
                        owner = SickleOwner.Boss;
                        transform.parent = oc.stepOnObject.transform;
                        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                        transform.localScale = new Vector3(oc.stepOnObject.transform.localScale.x, 1.0f, 1.0f);
                    }
                }
                break;



        }
        
        
    }

 
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("aaa");
        //衝突判定
        if (collision.tag == bossTag) //敵と接触
        {
            GameObject obj = collision.gameObject;
            ObjectCollision o = obj.GetComponent<ObjectCollision>();
            for (int i = 0; i < 3; i++)
            {
                if (o != null)
                {
                    break;
                }
                else
                {
                    Debug.Log("ObjectCollisionがついてない");
                    obj = obj.transform.parent.gameObject;
                    o = obj.GetComponent<ObjectCollision>();
                }

            }

            //もう一度踏んづける
            if (o != null)
            {
                Debug.Log("aaa");
                o.playerStepOn = true; //踏んづけたものに踏んづけたことを知らせる
                o.stepOnTag = tag;
            }
            else
            {
                Debug.Log("ObjectCollisionがついてない");
            }


        }
    }*/

}
