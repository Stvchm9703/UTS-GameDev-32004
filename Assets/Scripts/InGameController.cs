using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameController : MonoBehaviour
{
    [SerializeField] private InGameHUIManager inGameHUIManager;
    [SerializeField] private PacStudentController pacStudentController;
    [SerializeField] private PacStudentMovement pacStudentMovement;
    [SerializeField] private BGMController bgmController;
    [SerializeField] private LevelGenerator levelGenerator;

    // game state value
    [SerializeField] private int PowerPelletScore = 50, PelletScore = 10, CherryScore = 100, ScaredGhostScore = 300;
    [SerializeField] private int ScareTimeSecond = 10;
    private float survivalTime = 0, ghostTimer = 0;

    [SerializeField] private int score = 0, highScore = 0, life = 3;

    [SerializeField] private bool InScareState = false;

    [SerializeField] private bool ReadyState = true, GameOverState = false;

    [SerializeField] private GameObject GhostBluePrefab, GhostGreenPrefab, GhostRedPrefab, GhostYellowPrefab;
    private GameObject GhostBlueInst, GhostGreenInst, GhostRedInst, GhostYellowInst;
    private EnemyMovement emBlue, emGreen, emRed, emYellow;
    // [SerializeField] private GameObject GhostBlueLabel, GhostGreenLable, GhostRedLable, GhostYellowLable;

    [SerializeField] private Transform mapTransform;

    Coroutine scareTimerCoroutine;

    void Start()
    {
        OnGhostInit();
        OnPacStudentInit();
        OnExitGameButtonInit();
        ResetGame();
        StartCoroutine(StartCountdown());
        LoadHighScore();
    }

    private void FixedUpdate()
    {
        if (GameOverState || ReadyState) return;

        UpdateSurvivalTime();
        if (ghostTimer > 0)
        {
            ghostTimer -= Time.fixedDeltaTime;
            inGameHUIManager.UpdateGhostTimer(ghostTimer);
        }
    }

    IEnumerator StartCountdown()
    {
        yield return inGameHUIManager.StartCountdown();
        SetPauseState(false);
        yield return null;
    }

    private void OnPacStudentInit()
    {
        var student = GameObject.FindWithTag("Player");
        pacStudentMovement = student.GetComponent<PacStudentMovement>();
        if (pacStudentMovement)
        {
            // pacStudentController
            pacStudentMovement.EmmitOnItemHit.AddListener(OnItemHit);
            pacStudentMovement.EmmitOnGhostHit.AddListener(OnGhostHit);
        }

        var grid = GameObject.Find("Grid");
        levelGenerator = grid.GetComponent<LevelGenerator>();
        if (levelGenerator)
        {
            levelGenerator.emmitOnTeleport.AddListener((Waypoint wp1, Waypoint wp2) =>
                StartCoroutine(
                    pacStudentController.Teleport(wp1, wp2)
                )
            );
            if (levelGenerator.GhostRespawnPoint.Count > 0)
            {
                emBlue.RespwanPoint = levelGenerator.GhostRespawnPoint[0];
                emGreen.RespwanPoint = levelGenerator.GhostRespawnPoint[1];
                emRed.RespwanPoint = levelGenerator.GhostRespawnPoint[2];
                emYellow.RespwanPoint = levelGenerator.GhostRespawnPoint[3];
            }
        }

        pacStudentController.SetParseState(true);
    }

    private void OnGhostInit()
    {
        if (GhostBluePrefab != null)
        {
            GhostBlueInst = Instantiate(GhostBluePrefab, Vector3.zero, Quaternion.identity, mapTransform);
            GhostBlueInst.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            emBlue = GhostBlueInst.GetComponent<EnemyMovement>();
        }

        if (GhostGreenPrefab != null)
        {
            GhostGreenInst = Instantiate(GhostGreenPrefab, mapTransform);
            GhostGreenInst.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            emGreen = GhostGreenInst.GetComponent<EnemyMovement>();
        }

        if (GhostRedPrefab != null)
        {
            GhostRedInst = Instantiate(GhostRedPrefab, mapTransform);
            GhostRedInst.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            emRed = GhostRedInst.GetComponent<EnemyMovement>();
        }

        if (GhostYellowPrefab != null)
        {
            GhostYellowInst = Instantiate(GhostYellowPrefab, mapTransform);
            GhostYellowInst.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            emYellow = GhostYellowInst.GetComponent<EnemyMovement>();
        }
    }

    void OnItemHit(GameObject item)
    {
        var itemCollider = item.GetComponent<ItemCollider>();
        if (itemCollider.waypoint.gridType == 6)
        {
            this.InScareState = true;
            this.score += PowerPelletScore;
            UpdateGhostState(true);
        }
        else if (itemCollider.waypoint.gridType == 8)
        {
            this.score += CherryScore;
        }
        else if (itemCollider.waypoint.gridType == 5)
        {
            this.score += PelletScore;
        }

        this.inGameHUIManager.UpdateScore(this.score);
    }

    void OnGhostHit(GameObject ghost)
    {
        Debug.Log("onGhostHit");
        Debug.Log(ghost);
        EnemyMovement tmp = null;
        if (ghost == this.GhostBlueInst) tmp = emBlue;
        if (ghost == this.GhostGreenInst) tmp = emGreen;
        if (ghost == this.GhostRedInst) tmp = emRed;
        if (ghost == this.GhostYellowInst) tmp = emYellow;

        if (InScareState)
        {
            if (tmp) tmp.ChangeState(EnemyState.Dead);
            score += ScaredGhostScore;
        }
        else
        {
            UpdateLife();
        }
    }

    void UpdateGhostState(bool state)
    {
        this.InScareState = state;
        if (state)
        {
            emBlue.ChangeState(EnemyState.Scared);
            emGreen.ChangeState(EnemyState.Scared);
            emRed.ChangeState(EnemyState.Scared);
            emYellow.ChangeState(EnemyState.Scared);
            bgmController.PlayScared();

            inGameHUIManager.ToggleScareTime(true);
            scareTimerCoroutine = StartCoroutine(ScareTimer(ScareTimeSecond));
        }
        else
        {
            Debug.Log("scare timer end");
            if (scareTimerCoroutine != null)
            {
                StopCoroutine(scareTimerCoroutine);
                scareTimerCoroutine = null;
            }

            emBlue.ChangeState(EnemyState.Normal);
            emGreen.ChangeState(EnemyState.Normal);
            emRed.ChangeState(EnemyState.Normal);
            emYellow.ChangeState(EnemyState.Normal);
            inGameHUIManager.ToggleScareTime(false);
            bgmController.PlayNormal();
        }
    }


    IEnumerator ScareTimer(int seconds)
    {
        this.ghostTimer = seconds;
        yield return new WaitForSeconds(seconds);
        UpdateGhostState(false);
    }

    void UpdateSurvivalTime()
    {
        survivalTime += Time.fixedDeltaTime;
        inGameHUIManager.UpdateSurvivalTime(survivalTime);
    }


    void UpdateLife()
    {
        this.life--;
        // setup location for resume   
        if (life <= 0)
        {
            StartCoroutine(GameOver());
        }
        else
        {
            inGameHUIManager.UpdateLife(this.life);
            ResetGame();
            StartCoroutine(StartCountdown());
        }
    }

    [SerializeField] private Button exitGameButton;

    void OnExitGameButtonInit()
    {
        exitGameButton = GameObject.Find("ExitButton").GetComponent<Button>();
        exitGameButton.onClick.AddListener(OnExitGameClick);
    }

    void OnExitGameClick()
    {
        Application.Quit();
    }

    IEnumerator GameOver()
    {
        GameOverState = true;
        inGameHUIManager.ShowGameOver(score, highScore);
        bgmController.PlayGameOver();
        SaveScore();
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("StartScene");
    }

    void ResetGame()
    {
        InScareState = false;
        SetPauseState(true);
        // pacStudentMovement.ResetPosition();
        pacStudentController.ResetPosition();
        emBlue.ResetPosition();
        emGreen.ResetPosition();
        emRed.ResetPosition();
        emYellow.ResetPosition();
    }

    void SetPauseState(bool state)
    {
        this.ReadyState = state;
        // pacStudentMovement.PauseMovement = true;
        pacStudentController.SetParseState(state);
        emBlue.SetPause(state);
        emGreen.SetPause(state);
        emRed.SetPause(state);
        emYellow.SetPause(state);
    }

    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (inGameHUIManager)
        {
            inGameHUIManager.HighScore = highScore;
        }
    }


    void SaveScore()
    {
        PlayerPrefs.SetInt("Score", score);
        // PlayerPrefs.GetInt()
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(survivalTime * 1000);
            var timeString = string.Format("{0:00}:{1:00}:{2:00}",
                timeSpan.Minutes,
                timeSpan.Seconds,
                timeSpan.Milliseconds / 10);
            PlayerPrefs.SetString("HighScoreTime", timeString);
        }

        PlayerPrefs.Save();
    }
}