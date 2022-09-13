using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_mob2 : MonoBehaviour
{
    #region//インスペクターで設定
    [Header("画面外でも行動するか")] public bool nonVisible;
    [HideInInspector] public bool rightTleftF = false;
    [Header("速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("射程範囲Y")] public float attackRangeY;
    [Header("攻撃硬直時間")] public float attackFreezTime;
    [Header("攻撃クールタイム")] public float attackCoolTime;
    [Header("接触判定")] public EnemyCollisionCheck checkCollision;
    [Header("攻撃範囲判定")] public CollisionCheck range;
    [Header("球オブジェクト")] public GameObject bullet;

    [Header("攻撃1SE")] public AudioClip attack1SE;
    [Header("攻撃2SE")] public AudioClip attack2SE;
    [Header("ダメージSE")] public AudioClip damageSE;
    #endregion

    #region//プライベート変数
    private Rigidbody2D rb = null;
    private SpriteRenderer sr = null;
    private Animator anim = null;
    private ObjectCollision oc = null;
    private BoxCollider2D col = null;
    private bool isAttack = false;
    private bool isAttackOne = false;
    private bool isAttackEnd = false;
    private bool isDead = false;
    private bool isOnAttackRange = false;
    private float attackTime = 0.0f;
    private string playerKickBulletTag = "PlayerKickBullet";
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        oc = GetComponent<ObjectCollision>();
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        isOnAttackRange = range.IsOn();

        if (!oc.playerStepOn)
        {
            if (sr.isVisible || nonVisible)
            {
                int xVector = -1;
                if (!isAttack)
                {
                    //攻撃範囲判定処理
                    if (isOnAttackRange)
                    {
                        isAttack = true;
                        isAttackOne = true;
                    }

                    if (checkCollision.isOn)
                    {
                        rightTleftF = !rightTleftF;
                    }
                    
                    if (rightTleftF)
                    {
                        xVector = 1;
                        transform.localScale = new Vector3(-1, 1, 1);

                    }
                    else
                    {
                        transform.localScale = new Vector3(1, 1, 1);
                    }
                    
                }
                else //攻撃中
                {
                    Attack();
                    xVector = 0;
                    attackTime += Time.deltaTime;
                }

                //Debug.Log("見えている");
                rb.velocity = new Vector2(xVector * speed, -gravity);
            }
            else
            {
                rb.Sleep();
            }
        }
        else if(oc.stepOnTag == playerKickBulletTag)
        {
            if (!isDead)
            {
                anim.Play("enemy2_death");
                GManager.instance.PlaySE(damageSE);
                rb.velocity = new Vector2(0, -gravity);
                isDead = true;
                col.enabled = false;
                Destroy(gameObject, 3f);
            }
            else
            {
                transform.Rotate(new Vector3(0, 0, 5));
            }
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            oc.playerStepOn = false;
        }


        //アニメーションを適用
        SetAnimation();

    }

    /// <summary>
    /// アニメーションの設定を行う
    /// </summary>
    private void SetAnimation()
    {
        anim.SetBool("attack", isAttack);
    }

    public void Attack()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        float animTime = state.normalizedTime;

        if (isAttackOne)
        {
            anim.Play("enemy2_attack");
            GManager.instance.PlaySE(attack1SE);
            isAttackOne = false;
        }

        Debug.Log("attack");
        if (animTime >= 1)
        {
            Debug.Log("end");
            isAttack = false;
            isAttackEnd = false;
            anim.Play("enemy2_walk");
            attackTime = 0.0f;
        }
    }

    public void ShootBullet()
    {
        //球を発射する処理
        //Debug.Log("発射");
        GManager.instance.PlaySE(attack2SE);
        GameObject g = Instantiate(bullet);
        //g.transform.SetParent(transform);
        g.transform.position = bullet.transform.position;
        g.transform.rotation = bullet.transform.rotation;
        g.SetActive(true);

    }

    public void AttackEnd()
    {
        Debug.Log("end2");
        isAttackEnd = true;
        
    }
}
