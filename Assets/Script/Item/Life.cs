using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour
{

    private Text lifeText = null;
    private int oldLife = 0;

    // Start is called before the first frame update
    void Start()
    {
        lifeText = GetComponent<Text>();
        if (GManager.instance != null)
        {
            lifeText.text = "Life " + GManager.instance.life;
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
        if (oldLife != GManager.instance.life)
        {
            lifeText.text = "Life " + GManager.instance.life;
            oldLife = GManager.instance.life;
        }
    }
}
