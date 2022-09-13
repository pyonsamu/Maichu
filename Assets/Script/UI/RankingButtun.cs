using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingButtun : MonoBehaviour
{
    [Header("クリックSE")] public AudioClip clickSE;

    private bool firstPushRestart = false;
    private bool firstPushTitle = false;
    private int rankTimeNum = 0;
    private int rankScoreNum = 1;

    public void PressRankingTime()
    {
        GManager.instance.PlaySE(clickSE);

        //小数点第4位で切り捨て1.111
        float displayTime = Mathf.Floor(GManager.instance.timer);
        float timeMS = Mathf.Floor((GManager.instance.timer - displayTime) * 1000);
        float timeMS1 = timeMS % 10;
        float timeMS10 = ((timeMS % 100) - timeMS1) / 10;
        float timeMS100 = (timeMS - timeMS10 * 10 - timeMS1) / 100;
        float timeS = displayTime % 60;
        float timeS1 = timeS % 10;
        float timeS10 = (timeS - timeS1) / 10;
        float timeM = (displayTime - timeS) / 60;
        float timeM1 = timeM % 10;
        float timeM10 = (timeM - timeM1) / 10;
        float time = timeM10*1000000 + timeM1*100000 + timeS10*10000 + timeS1*1000 + timeMS100*100 + timeMS10*10 + timeMS1;
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(time, rankTimeNum);
    }

    public void PressRankingScore()
    {
        GManager.instance.PlaySE(clickSE);
        
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(GManager.instance.score, rankScoreNum);
    }

}
