using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [Header("プレイヤー接触判定")] public CollisionCheck playerCheck;

    private bool isGoal = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerCheck.IsOn())
        {
            isGoal = true;
        }
    }

    public bool IsGoalWaiting()
    {
        return isGoal;
    }
}
