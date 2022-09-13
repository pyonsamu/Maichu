using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    [Header("クリックSE")] public AudioClip clickSE;

    private bool firstPush = false;


    public void Start()
    {
        GManager.instance.Reset();
    }

    public void PressStart()
    {
        Debug.Log("Press Start");
        if (!firstPush)
        {
            GManager.instance.PlaySE(clickSE);
            GManager.instance.GameStart();
            SceneManager.LoadScene("main");
            Debug.Log("Go Next");
            firstPush = true;
        }
    }
}
