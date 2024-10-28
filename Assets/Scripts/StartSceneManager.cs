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

    // Start is called before the first frame update
    void Start()
    {
        LoadBestRecord();
        if (scoreText != null)
        {
            scoreText.text = LastScoreFormat();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    string LastScoreFormat()
    {
        string scoreFormat = "Best Score : {0} ({1})";
        return string.Format(scoreFormat, lastScore, bestTime);
    }

    void LoadBestRecord()
    {
        
    }
}