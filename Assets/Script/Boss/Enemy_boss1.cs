using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_boss1 : MonoBehaviour
{
    private enum BossState
    {
        Default,
        Swing,
        Pyon,
        Fall,
        Run,
        Start,
        Clear
    }
    private BossState nowState = BossState.Default;
    private bool isAttack = false;

    #region//Swing
    [Header("鎌オブジェクト")] public GameObject sickle;
    [Header("攻撃スピード")] public float rotSpeed;
    [Header("画面外でも行動するか")] public bool nonVisible;
    [Header("速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("接地判定")] public GroundCheck ground;
    [Header("接地判定")] public EnemyCollisionCheck checkCollision;
    [Header("体力")] public int hp;
    [Header("Pyonの高さ")] public float pyonHeight = 20;
    [Header("Pyonの速さ")] public float pyonSpeedX = 3;
    [Header("Pyonの高さ")] public float defaultPyonHeight = 10;
    [Header("ダメージSE")] public AudioClip damageSE;

    private bool firstflag = true;
    private bool visible = true;
    private int countAttack = 0;
    private float rotation = 0.0f;
    private float attackStartRot = -90.0f;
    private float attackEndRot = 0.0f;
    private float swingTime = 0.0f;
    private float time = 0.0f;
    private float fallScaleX, fallScaleY = 0.0f;
    private float coolTime = 0.0f;
    private Vector2 bossStartPos;
    private Rigidbody2D rb = null;
    private SpriteRenderer sr = null;
    private Animator anim = null;
    private ObjectCollision oc = null;
    private BoxCollider2D col = null;
    private bool rightTleftF = false;
    private bool isGround = false;
    private bool isDead = false;
    private bool isFallUp = false;
    private bool isFallDown = false;
    private string playerTag = "Player";
    private string playerKickBulletTag = "PlayerKickBullet";
    private string playerSickleTag = "PlayerSickle";

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        nowState = RandomBossState();
        nowState = BossState.Swing;
        isAttack = true;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        oc = GetComponent<ObjectCollision>();
        col = GetComponent<BoxCollider2D>();
        bossStartPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGround = ground.IsGround();
        if (checkCollision.isOn)
        {
            Debug.Log("change");
            rightTleftF = !rightTleftF;
        }
        int xVector = -1;
        if (rightTleftF)
        {
            xVector = 1;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * - 1, transform.localScale.y*1, transform.localScale.z*1);

        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * 1, transform.localScale.y * 1, transform.localScale.z * 1);
        }



        if (isAttack)
        {
            switch(nowState)
            {
                case BossState.Swing:
                    if (firstflag)
                    {
                        swingTime = 5;
                        rotation = attackStartRot;
                        firstflag = false;
                        sickle.transform.GetChild(0).gameObject.SetActive(true);
                        sickle.GetComponent<SpriteRenderer>().enabled = true;
                    }
                    if(sickle.GetComponent<Sickle>().owner== Sickle.SickleOwner.Boss)
                    {
                        if (rotation < attackEndRot)
                        {
                            rotation += rotSpeed;
                        }
                        else
                        {
                            rotation = attackEndRot;
                        }
                        sickle.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotation);
                        swingTime -= Time.deltaTime;
                        if (swingTime < 0)
                        {
                            nowState = BossState.Default;
                            sickle.transform.GetChild(0).gameObject.SetActive(false);
                            sickle.GetComponent<SpriteRenderer>().enabled = false;
                            coolTime = 2;
                        }
                    }else if(sickle.GetComponent<Sickle>().owner == Sickle.SickleOwner.Nan)
                    {
                        nowState = BossState.Run;
                        firstflag = true;
                        break;
                    }
                    break;
                case BossState.Pyon:
                    if (firstflag)
                    {
                        countAttack = 8;
                        firstflag = false;
                        
                    }
                    if (isGround)
                    {
                        Debug.Log("pyon");
                        rb.velocity = new Vector3(rb.velocity.x, pyonHeight,0.0f); //跳ねる処理
                        countAttack -= 1;
                    }
                    rb.velocity = new Vector3(pyonSpeedX * xVector, rb.velocity.y, 0.0f); //横移動処理

                    if (countAttack < 0)
                    {
                        nowState = BossState.Default;
                        coolTime = 2;
                    }
                    
                    break;
                case BossState.Fall:
                    if (firstflag)
                    {
                        countAttack = 5;
                        isFallUp = true;
                        firstflag = false;
                    }
                    if (isFallUp)
                    {
                        rb.Sleep();
                        transform.position = new Vector2(transform.position.x,transform.position.y+1);
                        Debug.Log("up");
                        if(transform.position.y > 100)
                        {
                            Debug.Log("aaa");
                            isFallUp = false;
                            isFallDown = true;
                            float random = Random.Range(-20f,0f);
                            transform.position = new Vector2(bossStartPos.x+random,transform.position.y);
                            rb.WakeUp();
                        }
                    }
                    else if(isFallDown)
                    {
                        rb.velocity = new Vector2(0.0f,-200.0f);
                        Debug.Log("down");
                        if (transform.position.y < bossStartPos.y)
                        {
                            rb.velocity = new Vector2(0.0f, 0.0f);
                            transform.position = new Vector2(transform.position.x,bossStartPos.y);
                            isFallUp = true;
                            isFallDown = false;
                            countAttack -= 1;
                        }
                    }
                    if(countAttack < 0)
                    {
                        nowState = BossState.Default;
                        coolTime = 2;
                    }
                    break;
                case BossState.Default:
                    firstflag = true;
                    coolTime -= Time.deltaTime;
                    if (isGround)
                    {
                        if (coolTime < 0)
                        {
                            nowState = RandomBossState();
                            break;
                        }
                        rb.velocity = new Vector3(rb.velocity.x, defaultPyonHeight, 0.0f); //跳ねる処理
                        countAttack -= 1;
                    }
                    break;
                case BossState.Run:
                    if (firstflag)
                    {
                        transform.localScale = new Vector3(0.3f * xVector, 0.3f, 0.3f);
                        countAttack = 8;
                        firstflag = false;
                    }
                    GetComponent<BoxCollider2D>().enabled = false;
                    GetComponent<CircleCollider2D>().enabled = true;
                    if (isGround)
                    {
                        Debug.Log("pyon");
                        rb.velocity = new Vector3(rb.velocity.x, defaultPyonHeight, 0.0f); //跳ねる処理
                    }
                    rb.velocity = new Vector3(pyonSpeedX * 2 * xVector, rb.velocity.y, 0.0f); //横移動処理

                    if (sickle.GetComponent<Sickle>().owner == Sickle.SickleOwner.Boss)
                    {
                        transform.position = new Vector3(transform.position.x,transform.position.y+5,transform.position.z);
                        transform.localScale = new Vector3(1.0f * xVector, 1.0f, 1.0f);
                        GetComponent<BoxCollider2D>().enabled = true;
                        GetComponent<CircleCollider2D>().enabled = false;
                        nowState = BossState.Default;
                        coolTime = 2;
                    }
                    break;

                default:
                    break;

            }
        }

        if (!oc.playerStepOn) //踏まれていない
        {
            
        }
        else if (oc.stepOnTag == playerSickleTag) //踏まれたとき
        {
            Debug.Log("damaged");
            if (!isDead)
            {
                Damaged();
            }
            else
            {
                //transform.Rotate(new Vector3(0, 0, 5));
            }
        }
        else
        {
            oc.playerStepOn = false;
        }
    }

    private BossState RandomBossState()
    {
        BossState state = BossState.Default;
        switch(Random.Range(0, 3))
        {
            case 0:
                state = BossState.Swing;
                break;
            case 1:
                state = BossState.Pyon;
                break;
            case 2:
                state = BossState.Fall;
                break;
            default:
                break;
        }
        return state;
    }

    private void Damaged()
    {
        hp -= 1;
        col.enabled = false;
        //anim.Play("enemy_death");
        //GManager.instance.PlaySE(damageSE);
        //rb.velocity = new Vector2(0, -gravity);Damaged();
        if (hp <= 0)
        {
            isDead = true;
        }
        //Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
                if(obj.transform.parent != null)
                {
                    obj = obj.transform.parent.gameObject;
                    o = obj.GetComponent<ObjectCollision>();
                }
                else
                {
                    break;
                }
                
            }

        }

        //衝突判定
        if (obj.tag == playerSickleTag) //敵と接触
        {
            //もう一度踏んづける
            if (o != null)
            {
                Debug.Log("sickle");
                o.playerStepOn = true; //踏んづけたものに踏んづけたことを知らせる
                o.stepOnTag = tag;
                o.stepOnObject = gameObject;
                //Damaged();
            }
            else
            {
                Debug.Log("ObjectCollisionがついてない");
            }


        }
    }

}
