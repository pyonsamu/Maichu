using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearTime : MonoBehaviour
{

    private Text clearTimeText = null;

    // Start is called before the first frame update
    void Start()
    {
        clearTimeText = GetComponent<Text>();
        if (GManager.instance != null)
        {
            //小数点第4位で切り捨て1.111
            float displayTime = Mathf.Floor(GManager.instance.timer);
            float timeMS = Mathf.Floor((GManager.instance.timer - displayTime) * 1000);
            float timeMS1 = timeMS % 10;
            float timeMS10 = ((timeMS % 100) - timeMS1) / 10;
            float timeMS100 = (timeMS - timeMS10*10 - timeMS1) / 100;
            float timeS = displayTime % 60;
            float timeS1 = timeS % 10;
            float timeS10 = (timeS - timeS1) / 10;
            float timeM = (displayTime - timeS) / 60;
            float timeM1 = timeM % 10;
            float timeM10 = (timeM - timeM1) / 10;
            clearTimeText.text = "Time: " + timeM10 + timeM1 + "m" + timeS10 + timeS1 + "s" + timeMS100 + timeMS10 + timeMS1 + "ms";
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
        
    }
}
