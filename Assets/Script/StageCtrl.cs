using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageCtrl : MonoBehaviour
{
    [Header("プレイヤーゲームオブジェクト")] public GameObject playerObj;
    [Header("コンティニュー位置")] public GameObject[] continuePoint;
    [Header("ゴールオブジェクト")] public Goal goal;
    private Hero p;


    // Start is called before the first frame update
    void Start()
    {
        if(playerObj != null && continuePoint != null && continuePoint.Length > 0)
        {
            playerObj.transform.position = continuePoint[0].transform.position;
            p = playerObj.GetComponent<Hero>();
        }
        else
        {
            Debug.Log("設定が足りてない");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (goal.IsGoalWaiting())
        {
            PlayerGoal();
        }
        else if(p != null && p.IsDeadWaiting())
        {
            GManager.instance.StopTimer();
            SceneManager.LoadScene("GameoverScene");
        }
        else if(p != null && p.IsContinueWaiting())
        {
            if(continuePoint.Length > GManager.instance.continueNum)
            {
                Debug.Log(p.damagedTag);
                if(p.damagedTag == "Spike")
                {
                    playerObj.transform.position = continuePoint[GManager.instance.continueNum].transform.position;
                }
                else if (p.damagedTag == "Enemy" || p.damagedTag == "Bullet")
                {

                }
                
                p.ContinuePlayer();
            }
        }
    }

    public void PlayerGoal()
    {
        GManager.instance.StopTimer();
        SceneManager.LoadScene("clearScene");

    }
}
