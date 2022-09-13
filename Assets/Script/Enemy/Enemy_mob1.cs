using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_mob1 : MonoBehaviour
{
    #region//インスペクターで設定
    [Header("画面外でも行動するか")] public bool nonVisible;
    [Header("速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("接触判定")] public EnemyCollisionCheck checkCollision;
    
    [Header("ダメージSE")] public AudioClip damageSE;
    #endregion

    #region//プライベート変数
    private Rigidbody2D rb = null;
    private SpriteRenderer sr = null;
    private Animator anim = null;
    private ObjectCollision oc = null;
    private BoxCollider2D col = null;
    private bool rightTleftF = false;
    private bool isDead = false;
    private string playerTag = "Player";
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
        if (!oc.playerStepOn)
        {
            if (sr.isVisible || nonVisible)
            {
                if (checkCollision.isOn)
                {
                    rightTleftF = !rightTleftF;
                }
                int xVector = -1;
                if (rightTleftF)
                {
                    xVector = 1;
                    transform.localScale = new Vector3(-1, 1, 1);

                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                rb.velocity = new Vector2(xVector * speed, -gravity);
                //Debug.Log("見えている");
            }
            else
            {
                rb.Sleep();
            }
        }
        else if (oc.stepOnTag == playerTag || oc.stepOnTag == playerKickBulletTag)
        {
            if (!isDead)
            {
                anim.Play("enemy_death");
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
            oc.playerStepOn = false;
        }
        
    }
}
