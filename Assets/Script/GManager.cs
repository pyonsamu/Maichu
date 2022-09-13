using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GManager : MonoBehaviour
{
    public static GManager instance = null;
    [Header("スコア")] public int score;
    [Header("ライフ")] public int life;
    [Header("コンティニューポイント")] public int continueNum;
    [HideInInspector] public float timer = 0.0f;

    private bool timerFlag = false;
    
    

    private AudioSource audioSource = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (timerFlag)
        {
            timer += Time.deltaTime;
        }
        
    }

    public void StopTimer()
    {
        timerFlag = false;
    }


    public void GameStart()
    {
        timer = 0.0f;
        timerFlag = true;
    } 

    public void Reset()
    {
        score = 0;
        life = 3;
        continueNum = 0;
    }

    public void PlaySE(AudioClip clip)
    {
        if(audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("オーディオソース設定されてない");
        }
    }
}
