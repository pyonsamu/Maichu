using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    #region//インスペクターで設定
    [Header("移動加速度")] public float accel;
    [Header("移動上限速度")] public float speedLimit;
    [Header("ジャンプ速度")] public float jumpSpeed;
    [Header("ロールキックX速度")] public float rollKickSpeedX;
    [Header("ロールキックY速度")] public float rollKickSpeedY;
    [Header("ロールキックX割合")] public float rollKickSpeedXRate;
    [Header("ロールキックY割合")] public float rollKickSpeedYRate;
    [Header("重力")] public float gravity;
    [Header("ジャンプする高さ")] public float jumpHeight;
    [Header("ジャンプする制限時間")] public float jumpLimitTime;
    [Header("ロールする制限時間")] public float rollLimitTime;
    [Header("踏みつけ判定の高さの割合")] public float stepOnRate;
    [Header("最後にやられた敵のタグ")] public string damagedTag;
    [Header("接地判定")] public GroundCheck ground;
    [Header("頭ぶつける判定")] public GroundCheck head;
    [Header("壁に右がくっついている判定")] public GroundCheck wall_r;
    [Header("壁に左がくっついている判定")] public GroundCheck wall_l;
    [Header("ダッシュの速さ表現")] public AnimationCurve dashCurve;
    [Header("ジャンプの速さ表現")] public AnimationCurve jumpCurve;

    [Header("ジャンプSE")] public AudioClip jumpSE;
    [Header("ロールSE")] public AudioClip rollSE;
    [Header("ロールキックSE")] public AudioClip rollKickSE;
    [Header("ダメージSE")] public AudioClip damageSE;
    #endregion


    #region//プライベート変数
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private CapsuleCollider2D capcol = null;
    private SpriteRenderer sr = null;
    private bool isGround = false;
    private bool isHead = false;
    private bool isJump = false;
    private bool isDash = false;
    private bool isDown = false;
    private bool isOtherJump = false;
    private bool isRoll = false;
    private bool canRoll = false;
    private bool isWallR = false;
    private bool isWallL = false;
    private bool isContinue = false;
    private bool isDamage = false;
    private bool isDead = false;
    private float continueTime = 0.0f;
    private float blinkTime = 0.0f;
    private float xSpeed = 0.0f;
    private float ySpeed = 0.0f;
    private float jumpPos = 0.0f;
    private float otherJumpHeight = 0.0f;
    private float jumpTime = 0.0f;
    private float dashTime = 0.0f;
    private float rollTime = 0.0f;
    private float beforeKey = 0.0f;
    private string enemyTag = "Enemy";
    private string bulletTag = "Bullet";
    private string spikeTag = "Spike";
    private string sickleTag = "Sickle";
    private string bossTag = "Boss";
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capcol = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isContinue || isDamage)
        {
            //明滅　ついてる
            if(blinkTime >= 0.2f)
            {
                sr.enabled = true;
                blinkTime = 0.0f;
            }
            //明滅　消えてる
            else if (blinkTime >= 0.1f)
            {
                sr.enabled = false;
            }
            //明滅　ついてる
            else
            {
                sr.enabled = true;
            }

            //1秒たったら終わり
            if(continueTime > 2)
            {
                isContinue = false;
                isDamage = false;
                isDown = false;
                blinkTime = 0.0f;
                continueTime = 0.0f;
                sr.enabled = true;
            }
            else
            {
                blinkTime += Time.deltaTime;
                continueTime += Time.deltaTime;
            }
        }
    }


        // Update is called once per frame
        void FixedUpdate()
    {
        if (!isDown || isContinue || isDamage)
        {
            //接地判定を得る
            isGround = ground.IsGround();
            isHead = head.IsGround();
            isWallR = wall_r.IsGround();
            isWallL = wall_l.IsGround();

            //ロールの処理
            doRoll();

            //各座標軸の速度を決める
            float xSpeed = GetXSpeed();
            float ySpeed = GetYSpeed();

            //アニメーションを適用
            SetAnimation();

            //移動速度を設定
            rb.velocity = new Vector2(xSpeed, ySpeed);
        }
        else
        {
            //移動速度を設定
            rb.velocity = new Vector2(0, -gravity);
        }
        
    }

    /// <summary>
    /// Y成分で必要な計算を行い，値を返す
    /// </summary>
    /// <returns>Y軸の速度</returns>
    private float GetYSpeed()
    {
        float verticalKey = Input.GetAxisRaw("Vertical");
        float horizontalKey = Input.GetAxisRaw("Horizontal");
        float yAccel = -gravity;
        bool isWall = isWallR || isWallL;

        if (isRoll && isWall)
        {
            GManager.instance.PlaySE(rollKickSE);
            if (isWallR)
            {
                //右に壁があるとき
                if (horizontalKey > 0)
                {
                    ySpeed = rollKickSpeedY;
                }
                else
                {
                    ySpeed = rollKickSpeedY * (rollKickSpeedYRate /100);
                }
            }
            else
            {
                //左に壁があるとき
                if (horizontalKey < 0)
                {
                    ySpeed = rollKickSpeedY;
                }
                else
                {
                    ySpeed = rollKickSpeedY * (rollKickSpeedYRate / 100);
                }
            }
            canRoll = true;
            isJump = false;
        }
        else
        {
            //跳ねる処理
            if (isOtherJump)
            {
                //Debug.Log("otherjump");
                //ジャンプ高さ制限を超えていないか
                //bool canHeight = jumpPos + otherJumpHeight > transform.position.y;
                //ジャンプ時間制限を超えていないか
                //bool canTime = jumpLimitTime > jumpTime;

                if (!isHead)
                {
                    ySpeed = rollKickSpeedY;
                    canRoll = true;
                    isOtherJump = false;
                    //jumpTime += Time.deltaTime;
                }
                else
                {
                    isOtherJump = false;
                    //jumpTime = 0.0f;
                }
            }
            //地面に接地している時
            else if (isGround)
            {
                if (verticalKey > 0)
                {
                    if (!isJump)
                    {
                        GManager.instance.PlaySE(jumpSE);
                    }
                    ySpeed = jumpSpeed;
                    jumpPos = transform.position.y; //ジャンプした位置を記録
                    isJump = true;
                    jumpTime = 0.0f;
                }
                else
                {
                    isJump = false;
                }
            }
            //ジャンプしているとき
            else if (isJump)
            {
                //上方向キーが押されているか
                //bool pushUpKey = verticalKey > 0;
                //ジャンプ高さ制限を超えていないか
                //bool canHeight = jumpPos + jumpHeight > transform.position.y;
                //ジャンプ時間制限を超えていないか
                //bool canTime = jumpLimitTime > jumpTime;

               /* if (pushUpKey && canHeight && canTime && !isHead)
                {
                    //ySpeed = jumpSpeed;
                    //jumpTime += Time.deltaTime;
                }
                else
                {
                    isJump = false;
                    jumpTime = 0.0f;
                }*/
            }

            //スピードを設定
            if ((ySpeed >= -speedLimit && ySpeed <= speedLimit) || (ySpeed < -speedLimit && yAccel > 0) || (ySpeed > speedLimit && yAccel < 0))
            {
                ySpeed += yAccel; //xSpeed = xSpeed + xAccel
            }

            //アニメーションカーブを反映
            if (isJump || isOtherJump)
            {
                //ySpeed *= jumpCurve.Evaluate(jumpTime);
            }
        }

        return ySpeed;
    }

    public bool IsRoll()
    {
        return isRoll;
    }

    /// <summary>
    /// X成分で必要な計算を行い，値を返す
    /// </summary>
    /// <returns>X軸の速度</returns>
    private float GetXSpeed()
    {
        float horizontalKey = Input.GetAxisRaw("Horizontal");
        float xAccel = 0.0f;
        bool isWall = isWallR || isWallL;

        //横移動
        if (isRoll && isWall)
        {
            if (isWallR)
            {
                //右に壁があるとき
                if (horizontalKey > 0)
                {
                    xSpeed = -rollKickSpeedX * (rollKickSpeedXRate / 100);
                }
                else
                {
                    xSpeed = -rollKickSpeedX;
                }
            }
            else
            {
                //左に壁があるとき
                if (horizontalKey < 0)
                {
                    xSpeed = rollKickSpeedX * (rollKickSpeedXRate / 100);
                }
                else
                {
                    xSpeed = rollKickSpeedX;
                }
            }
            canRoll = true;
        }
        else
        {
            //加速度を設定
            if (horizontalKey > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                wall_r.transform.localScale = new Vector3(1, 1, 1);
                wall_l.transform.localScale = new Vector3(1, 1, 1);
                isDash = true;
                dashTime += Time.deltaTime;
                xAccel = accel;
            }
            else if (horizontalKey < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                wall_r.transform.localScale = new Vector3(-1, 1, 1);
                wall_l.transform.localScale = new Vector3(-1, 1, 1);
                isDash = true;
                dashTime += Time.deltaTime;
                xAccel = -accel;
            }
            else
            {
                //isDash = false;
                dashTime = 0.0f;
                if(xSpeed != 0)
                {
                    xAccel = accel * 0.5f * -(xSpeed / Mathf.Abs(xSpeed));
                }
                else
                {
                    isDash = false;
                }

            }

            //スピードを設定
            if((xSpeed >= -speedLimit && xSpeed <= speedLimit) || (xSpeed < -speedLimit && xAccel > 0) || (xSpeed > speedLimit && xAccel < 0))
            {
                xSpeed += xAccel; //xSpeed = xSpeed + xAccel
            }

            /*if (!isDash)
            {
                xSpeed = 0.0f;
            }*/
            

            //前回の入力からダッシュの反転を判断し，時間をリセット
            if (horizontalKey < 0 && beforeKey > 0)
            {
                dashTime = 0.0f;
            }
            else if (horizontalKey > 0 && beforeKey < 0)
            {
                dashTime = 0.0f;
            }
            beforeKey = horizontalKey;

            //アニメーションカーブを反映
            //xSpeed *= dashCurve.Evaluate(dashTime);
        }
        
        

        

        return xSpeed;
    }

    private void doRoll()
    {
        float rollKey = Input.GetAxis("Roll");
        if (!isGround)
        {

            if(!isRoll && rollKey > 0 && canRoll)
            {
                GManager.instance.PlaySE(rollSE);
                isRoll = true;
                rollTime = 0.0f;
                canRoll = false;
            }
            else if (isRoll)
            {
                bool canTime = rollLimitTime - rollTime > 0;
                if (canTime)
                {
                    rollTime += Time.deltaTime;
                }
                else
                {
                    isRoll = false;
                    rollTime = 0.0f;
                }
                
            }
        }
        else
        {
            isRoll = false;
            canRoll = true;
        }
    }

    /// <summary>
    /// アニメーションの設定を行う
    /// </summary>
    private void SetAnimation()
    {
        anim.SetBool("jump", isJump || isOtherJump);
        anim.SetBool("ground", isGround);
        anim.SetBool("dash", isDash);
        anim.SetBool("roll", isRoll);
    }



    /// <summary>
    ///コンティニュー処理を行う
    /// </summary>
    public void ContinuePlayer()
    {
        
        anim.Play("hero_stand");
        isDash = false;
        isRoll = false;
        isOtherJump = false;
        isJump = false;

        isContinue = true;
    }

    public bool IsContinueWaiting()
    {
        if(!isDead)
        {
            return IsDownAnimEnd();
        }
        return false;
    }

    public bool IsDeadWaiting()
    {
        if (isDead)
        {
            return true;
        }
        return false;
    }

    private bool IsDownAnimEnd()
    {
        if(isDown && anim != null)
        {
            AnimatorStateInfo animstate = anim.GetCurrentAnimatorStateInfo(0);
            if (animstate.IsName("hero_down"))
            {
                if(animstate.normalizedTime >= 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// やられたときに実行
    /// </summary>
    private void PlayerDown()
    {
        anim.Play("hero_down");
        GManager.instance.PlaySE(damageSE);
        isDown = true;
        isDamage = true;
        if (GManager.instance != null)
        {
            if (GManager.instance.life >= 1)
            {
                GManager.instance.life -= 1;
            }
            else
            {
                //ゲームオーバー画面遷移
                isDead = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        ////敵，球，針と接触
        if (collision.collider.tag == enemyTag || collision.collider.tag == bulletTag || collision.collider.tag == spikeTag || collision.collider.tag == bossTag) 
        {

            foreach(ContactPoint2D p in collision.contacts)
            {
                if(isRoll)
                {
                    //もう一度踏んづける
                    ObjectCollision o = collision.gameObject.GetComponent<ObjectCollision>();
                    if(o != null)
                    {
                        o.playerStepOn = true; //踏んづけたものに踏んづけたことを知らせる
                        o.stepOnTag = tag;
                    }
                    else
                    {
                        Debug.Log("ObjectCollisionがついてない");
                    }
                    isOtherJump = true;
                    isJump = false;
                    jumpTime = 0.0f;
                    GManager.instance.PlaySE(rollKickSE);
                }
                else if(!isDown)
                {
                    //ダウンする
                    damagedTag = collision.collider.tag;
                    PlayerDown();
                    break;
                }
            }
            
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //鎌との接触
        if (collision.gameObject.tag == sickleTag)
        {
            GameObject obj = collision.gameObject;
            for (int i = 0; i < 3; i++)
            {
                ObjectCollision o = obj.GetComponent<ObjectCollision>();
                if (o != null)
                {
                    break;
                }
                else
                {
                    Debug.Log("ObjectCollisionがついてない");
                    obj = obj.transform.parent.gameObject;
                }

            }

            if (isRoll)
            {
                if(obj.GetComponent<Sickle>().owner == Sickle.SickleOwner.Boss)
                {
                    ObjectCollision o = obj.GetComponent<ObjectCollision>();
                    //もう一度踏んづける
                    o.playerStepOn = true; //踏んづけたものに踏んづけたことを知らせる
                    o.stepOnTag = tag;
                    isOtherJump = true;
                    isJump = false;
                    jumpTime = 0.0f;
                    GManager.instance.PlaySE(rollKickSE);
                }
                

            }
            else if (!isDown)
            {
                if(obj.GetComponent<Sickle>().owner == Sickle.SickleOwner.Boss)
                {
                    //ダウンする
                    damagedTag = collision.tag;
                    PlayerDown();
                }
                else if (obj.GetComponent<Sickle>().owner == Sickle.SickleOwner.Nan)
                {
                    ObjectCollision o = obj.GetComponent<ObjectCollision>();
                    o.playerStepOn = true; //踏んづけたものに踏んづけたことを知らせる
                    o.stepOnTag = tag;
                    o.stepOnObject = gameObject;
                    
                }
            }

        }
    }
}
