using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearScore : MonoBehaviour
{
    private Text clearScoreText = null;

    // Start is called before the first frame update
    void Start()
    {
        clearScoreText = GetComponent<Text>();
        if (GManager.instance != null)
        {
            clearScoreText.text = "Score: " + GManager.instance.score;
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
