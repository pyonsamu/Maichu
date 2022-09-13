using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region//インスペクターで設定
    [Header("画面外でも行動するか")] public bool nonVisible;
    [Header("発射速度X")] public float speedx;
    [Header("発射速度Y")] public float speedy;
    [Header("プレイヤーの蹴るボール速度")] public float playerKickSpeed;
    [Header("重力")] public float gravity;
    [Header("消えるまでの時間制限")] public float deleteLimitTime;
    [Header("発射する敵")] public Enemy_mob2 enemy2;
    #endregion

    #region//プライベート変数
    private Rigidbody2D rb = null;
    private SpriteRenderer sr = null;
    private ObjectCollision oc = null;
    private CircleCollider2D col = null;
    private bool rightTleftF = false;
    private bool isDead = false;
    private bool isKick = true;
    private float deleteTime = 0.0f;
    private string enemyTag = "Enemy";
    private string playerKickBulletTag = "PlayerKickBullet";
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        oc = GetComponent<ObjectCollision>();
        col = GetComponent<CircleCollider2D>();
        int xVector = -1;
        if (enemy2.rightTleftF)
        {
            xVector = 1;
        }
        else
        {
            xVector = -1;
        }
        rb.velocity = new Vector2(speedx * xVector,speedy);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (oc.playerStepOn)
        {
        //プレイヤーが蹴ったとき
            if (isKick)
            {
                //プレイヤーが蹴った球にタグを変更
                tag = playerKickBulletTag;

                //敵方向に飛ばす
                Vector2 vec = enemy2.transform.position - transform.position;
                rb.velocity = vec.normalized * playerKickSpeed;
                deleteTime = 0.0f;
                isKick = false;
            }
        }

        if (deleteTime > deleteLimitTime)
        {
            Destroy(gameObject);
        }
        else
        {

            deleteTime += Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == enemyTag) //敵と接触
        {
            foreach (ContactPoint2D p in collision.contacts)
            {
                
                //もう一度踏んづける
                ObjectCollision o = collision.gameObject.GetComponent<ObjectCollision>();
                if (o != null)
                {
                    o.playerStepOn = true; //踏んづけたものに踏んづけたことを知らせる
                    o.stepOnTag = tag;
                }
                else
                {
                    Debug.Log("ObjectCollisionがついてない");
                }
                
            }

        }

    }
}
