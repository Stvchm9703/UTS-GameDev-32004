using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameHUIManager : MonoBehaviour
{

    // life-cherry
    public int life = 3;
    public List<GameObject> lifeCherry = new List<GameObject>();
    
    // score
    public int score = 0;
    public Text scoreText;
    
    // time
    float _survivalTime = 0;
    [SerializeField] Text survivalTimeText;
    
    //  ghostTimer
    float _ghostTimer = 0;
    [SerializeField] GameObject ghostTimerGameObj;
    [SerializeField] Text ghostTimerText;
    // Start is called before the first frame update
    void Start()
    {
        UpdateScore(0);
    }

    void FixedUpdate()
    {
        UpdateSurvivalTime();
        if (_survivalTime > 0)
        {
            UpdateGhostTimer();
        }
    }


    public void RemoveLife()
    {
        life -= 1;
        var lastCherry = lifeCherry[^1];
        lastCherry.SetActive(false);
    }

    public void UpdateScore(int value)
    {
        score = value;
        scoreText.text = $"Score: {score}";
    }

    void UpdateSurvivalTime()
    {
        _survivalTime += Time.fixedTime;
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(_survivalTime);
        survivalTimeText.text = string.Format("Time : {0:00}:{1:00}:{2:00}", 
            timeSpan.Minutes, 
            timeSpan.Seconds, 
            timeSpan.Milliseconds / 10);
    }

    public void StartScareTime(float seconds)
    {
        _ghostTimer = seconds;
        ghostTimerGameObj.SetActive(true);
    }

    void UpdateGhostTimer()
    {
        _ghostTimer -= Time.fixedDeltaTime;
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(_ghostTimer * 1000);
        ghostTimerText.text = string.Format("Scared Time : {0:00}:{1:00}:{2:00}", 
            timeSpan.Minutes, 
            timeSpan.Seconds, 
            timeSpan.Milliseconds / 10);

        if (_ghostTimer <= 0)
        {
            ghostTimerGameObj.SetActive(false);
        }
    }
    
    public void ShowGameOver()
    {
        // show game over
    }
    
}
