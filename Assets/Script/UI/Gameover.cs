using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameover : MonoBehaviour
{

    [Header("クリックSE")] public AudioClip clickSE;

    private bool firstPushRestart = false;
    private bool firstPushTitle = false;

    public void PressRestart()
    {
        if (!firstPushRestart)
        {
            GManager.instance.PlaySE(clickSE);
            GManager.instance.Reset();
            GManager.instance.GameStart();
            SceneManager.LoadScene("main");
            firstPushRestart = true;
        }
    }

    public void PressTitle()
    {
        if (!firstPushTitle)
        {
            GManager.instance.PlaySE(clickSE);
            SceneManager.LoadScene("titleScene");
            firstPushTitle = true;
        }
    }
}
