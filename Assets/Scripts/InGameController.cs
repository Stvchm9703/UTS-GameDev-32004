using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameController : MonoBehaviour
{
    [SerializeField] private InGameHUIManager inGameHUIManager;
    [SerializeField] private PacStudentController pacStudentController;
    [HideInInspector] public PacStudentMovement pacStudentMovement;
    [SerializeField] private BGMController bgmController;
    [SerializeField] public LevelGenerator levelGenerator;
    [SerializeField] private GhostController ghostController;

    // game state value
    [SerializeField] private int PowerPelletScore = 50, PelletScore = 10, CherryScore = 100, ScaredGhostScore = 300;
    [SerializeField] private int ScareTimeSecond = 10;
    float survivalTime = 0, ghostTimer = 0;
    int score = 0, highScore = 0, life = 3;
    public bool InScareState = false;
    bool ReadyState = true, GameOverState = false;

    [SerializeField] private GameObject GhostBluePrefab, GhostGreenPrefab, GhostRedPrefab, GhostYellowPrefab;
    [HideInInspector] public GameObject GhostBlueInst, GhostGreenInst, GhostRedInst, GhostYellowInst;
    [HideInInspector] public EnemyMovement emBlue, emGreen, emRed, emYellow;
    // [SerializeField] private GameObject GhostBlueLabel, GhostGreenLable, GhostRedLable, GhostYellowLable;

    [SerializeField] private Transform mapTransform;

    Coroutine scareTimerCoroutine;

    [HideInInspector] public UnityEvent<bool, bool> EmmitOnScareStateChange = new UnityEvent<bool, bool>();
    [HideInInspector] public UnityEvent<bool, bool> EmmitOnReadyStateChange = new UnityEvent<bool, bool>();

    void Start()
    {
        InitGhostInstance();
        InitPacStudent();
        InitExitGameButton();
        ResetGame();
        StartCoroutine(StartCountdown());
        InitGhostController();
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

    private void InitPacStudent()
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
                StartCoroutine(pacStudentController.Teleport(wp1, wp2))
            );

            emBlue.RespwanPoint = levelGenerator.GhostRespawnPoint[0];
            emBlue.TargetPoint = levelGenerator.GhostRespawnPoint[0];
            emGreen.RespwanPoint = levelGenerator.GhostRespawnPoint[1];
            emGreen.TargetPoint = levelGenerator.GhostRespawnPoint[1];
            emRed.RespwanPoint = levelGenerator.GhostRespawnPoint[2];
            emRed.TargetPoint = levelGenerator.GhostRespawnPoint[2];
            emYellow.RespwanPoint = levelGenerator.GhostRespawnPoint[3];
            emYellow.TargetPoint = levelGenerator.GhostRespawnPoint[3];
        }

        pacStudentController.SetParseState(true);
    }

    private void InitGhostInstance()
    {
        if (GhostBluePrefab != null)
        {
            GhostBlueInst = Instantiate(GhostBluePrefab, mapTransform);
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

    void InitGhostController()
    {
        ghostController = GetComponent<GhostController>();
        ghostController.Init();
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
        var lastState = this.InScareState;
        this.InScareState = state;
        if (state)
        {
            bgmController.PlayScared();

            inGameHUIManager.ToggleScareTime(true);
            scareTimerCoroutine = StartCoroutine(ScareTimer(ScareTimeSecond));
        }
        else
        {
            if (scareTimerCoroutine != null)
            {
                StopCoroutine(scareTimerCoroutine);
                scareTimerCoroutine = null;
            }

            inGameHUIManager.ToggleScareTime(false);
            bgmController.PlayNormal();
        }

        EmmitOnScareStateChange.Invoke(lastState, state);
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

    void InitExitGameButton()
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
        var lastState = this.ReadyState;
        this.ReadyState = state;
        // pacStudentMovement.PauseMovement = true;
        pacStudentController.SetParseState(state);

        EmmitOnReadyStateChange.Invoke(lastState, state);
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