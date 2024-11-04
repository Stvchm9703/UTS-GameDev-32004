using System.Collections;
using System.Collections.Generic;
// using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    public int lastScore = 0;
    public string bestTime = "";

    [SerializeField] private Text scoreText;
    [SerializeField] private Button level1Button, level2Button;

    // Start is called before the first frame update
    void Start()
    {
        LoadBestRecord();
        if (scoreText != null)
        {
            scoreText.text = LastScoreFormat();
        }
        if (level1Button) level1Button.onClick.AddListener(() => LoadLevel(1));
    }


    string LastScoreFormat()
    {
        string scoreFormat = "Best Score : {0} ({1})";
        return string.Format(scoreFormat, lastScore, bestTime);
    }

    void LoadBestRecord()
    {
        lastScore = PlayerPrefs.GetInt("HighScore", lastScore);
        bestTime = PlayerPrefs.GetString("HighScoreTime", bestTime);
    }
    void LoadLevel(int level)
    {
        SceneManager.LoadScene("Level" + level);
    }
}