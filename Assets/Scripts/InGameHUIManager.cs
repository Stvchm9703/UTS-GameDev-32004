using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameHUIManager : MonoBehaviour
{
    public List<GameObject> lifeCherry = new List<GameObject>();
    public Text scoreText;
    [SerializeField] Text survivalTimeText, ghostTimerText;
    [SerializeField] GameObject ghostTimerGameObj;
    
    
    // ReadyState Screen
    [SerializeField] GameObject readyScreen, readyText, countdown3Text, countdown2Text, countdown1Text, countdownGoText; 
    // GameOver Screen
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TMP_Text CurrentScoreText, HighScoreText;
    public UnityEvent EmmitOnReadyScreenEnd = new UnityEvent();

    public int HighScore;

    void Start()
    {
        UpdateScore(0);
    }

    public void UpdateLife(int currentLife)
    {
        // life -= 1;
        int lifeIndex = lifeCherry.Count - currentLife;
        var lastCherry = lifeCherry[^lifeIndex];
        lastCherry.SetActive(false);
    }

    public void UpdateScore(int value)
    {
        scoreText.text = $"Score: {value}";
    }

    public void UpdateSurvivalTime(float currentTime)
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(currentTime * 1000);
        survivalTimeText.text = string.Format("Time : {0:00}:{1:00}:{2:00}",
            timeSpan.Minutes,
            timeSpan.Seconds,
            timeSpan.Milliseconds / 10);
    }

    public void ToggleScareTime(bool scareState = false)
    {
        ghostTimerGameObj.SetActive(scareState);
    }

    public void UpdateGhostTimer(float ghostTimer)
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(ghostTimer * 1000);
        ghostTimerText.text = string.Format("Scared Time : {0:00}:{1:00}:{2:00}",
            timeSpan.Minutes,
            timeSpan.Seconds,
            timeSpan.Milliseconds / 10);
    }
    
    

    public void ShowGameOver(int score = 0, int highScore = 0)
    {
        // show game over
        CurrentScoreText.text = score.ToString();
        HighScoreText.text = highScore.ToString();
        if (score > highScore)
        {
            HighScoreText.text = score.ToString();
        }
        gameOverScreen.SetActive(true);
        
    }

    public IEnumerator StartCountdown()
    {
        readyScreen.SetActive(true);
        readyText.SetActive(true);
        yield return new WaitForSeconds(1f);
        readyText.SetActive(false);
        countdown3Text.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdown3Text.SetActive(false);
        countdown2Text.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdown2Text.SetActive(false);
        countdown1Text.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdown1Text.SetActive(false);
        countdownGoText.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdownGoText.SetActive(false);
        readyScreen.SetActive(false);
        EmmitOnReadyScreenEnd.Invoke();
    }
}