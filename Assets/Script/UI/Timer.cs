using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private Text timerText = null;

    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
        if (GManager.instance != null)
        {
            //小数点第4位で切り捨て1.111
            float displayTime = Mathf.Floor(GManager.instance.timer);
            float timeS = displayTime % 60;
            float timeS1 = timeS % 10;
            float timeS10 = (timeS - timeS1) / 10;
            float timeM = (displayTime - timeS) / 60;
            float timeM1 = timeM % 10;
            float timeM10 = (timeM - timeM1) / 10;
            timerText.text = "Time " + timeM10 + timeM1 + ":" + timeS10 + timeS1;
        }
        else
        {
            Debug.Log("ゲームマネージャー置き忘れ");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //小数点第4位で切り捨て1.111
        float displayTime = Mathf.Floor(GManager.instance.timer);
        float timeS = displayTime % 60;
        float timeS1 = timeS % 10;
        float timeS10 = (timeS - timeS1) /10;
        float timeM = (displayTime - timeS) / 60;
        float timeM1 = timeM % 10;
        float timeM10 = (timeM - timeM1) /10 ;
        timerText.text = "Time " + timeM10 + timeM1 + ":" + timeS10 + timeS1;
        
    }
}
