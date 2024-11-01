using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameController : MonoBehaviour
{
    [SerializeField] private InGameHUIManager inGameHUIManager;
    [SerializeField] private PacStudentController pacStudentController;
    [SerializeField] private PacStudentMovement pacStudentMovement;
    [SerializeField] private BGMController bgmController;

    // game state value
    [SerializeField] private int PowerPelletScore = 50, PelletScore = 10, CherryScore = 100, ScaredGhostScore = 300;
    [SerializeField] private int ScareTimeSecond = 10;

    [SerializeField] private int score = 0, life = 3;

    [SerializeField] private bool InScareState = false;

    [SerializeField] private bool GameOverState = false, PauseState = false;

    [SerializeField] private GameObject GhostBluePrefab, GhostGreenPrefab, GhostRedPrefab, GhostYellowPrefab;

    private GameObject GhostBlueInst, GhostGreenInst, GhostRedInst, GhostYellowInst;
    private EnemyMovement emBlue, emGreen, emRed, emYellow;
    // [SerializeField] private GameObject GhostBlueLabel, GhostGreenLable, GhostRedLable, GhostYellowLable;

    [SerializeField] private Transform mapTransform;

    void Start()
    {
        OnGhostInit();
        OnPacStudentInit();
        OnExitGameButtonInit();
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

            inGameHUIManager.StartScareTime(ScareTimeSecond);
            StartCoroutine(ScareTimer(ScareTimeSecond));
        }
        else
        {
            emBlue.ChangeState(EnemyState.Normal);
            emGreen.ChangeState(EnemyState.Normal);
            emRed.ChangeState(EnemyState.Normal);
            emYellow.ChangeState(EnemyState.Normal);
            bgmController.PlayNormal();
        }
    }


    IEnumerator ScareTimer(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        UpdateGhostState(false);
    }

    void UpdateLife()
    {
        this.life--;
        inGameHUIManager.RemoveLife();
        // setup location for resume   
        if (life <= 0)
        {
            GameOver();
        }
        else
        {
            ResetGame();
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
    
    void GameOver()
    {
        GameOverState = true;
        inGameHUIManager.ShowGameOver();
    }
    
    void ResetGame()
    {
        pacStudentMovement.ResetPosition();
        emBlue.ResetPosition();
        emGreen.ResetPosition();
        emRed.ResetPosition();
        emYellow.ResetPosition();
    }
    
    void SaveScore()
    {
        PlayerPrefs.SetInt("Score", score);
    }
}