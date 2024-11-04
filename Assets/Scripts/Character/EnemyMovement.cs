using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum EnemyState
{
    Idle,
    Normal,
    Scared,
    Dead,
    Revive,
}

public class EnemyMovement : MonoBehaviour
{
    public float speed = 1.0f;

    [HideInInspector] public Queue<Waypoint> CurrentRoute = new Queue<Waypoint>();

    [HideInInspector] public Waypoint TargetPoint, RespwanPoint;

    private Animator animator;

    // private EnemyAnimationStateController enemyAnimationStateController;
    private Tweener tweener;


    [SerializeField] public EnemyState state = EnemyState.Idle;

    private Vector3 basicScale, flipScale;

    public bool isPaused = true;
    PacStudentMovement pacStudentMovement;


    [HideInInspector] public UnityEvent emmitOnAllRouteCompleted = new UnityEvent();

    [HideInInspector]
    public UnityEvent<EnemyState, EnemyState> emmitOnChangeState = new UnityEvent<EnemyState, EnemyState>();

    [HideInInspector] public UnityEvent emmitOnBackToRespawn = new UnityEvent();


    public bool Arrived()
    {
        var distance = Vector3.Distance(transform.position, TargetPoint.position);
        return distance < 0.01f;
    }

    void Start()
    {
        var student = GameObject.FindWithTag("Player");
        pacStudentMovement = student.GetComponent<PacStudentMovement>();
        // enemyAnimationStateController = GetComponent<EnemyAnimationStateController>();
        animator = GetComponent<Animator>();
        tweener = GetComponent<Tweener>();
        if (tweener == null)
            tweener = gameObject.AddComponent<Tweener>();

        basicScale = new Vector3(
            transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z
        );
        flipScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z
        );
        ChangeState(EnemyState.Normal);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPaused) return;
        UpdateMove();
    }

    void UpdateMove()
    {
        if (Arrived())
        {
            Debug.Log(this.gameObject.name + " Arrived CurrentRoute.Count: " + CurrentRoute.Count);
            if (CurrentRoute.Count == 0)
            {
                Debug.Log("Arrived at destination");
                emmitOnAllRouteCompleted.Invoke();
                if (TargetPoint == RespwanPoint)
                {
                    emmitOnBackToRespawn.Invoke();
                }
                return;
            }

            TargetPoint = CurrentRoute.Dequeue();
        }
        else
        {
            MoveToWaypoint(TargetPoint);
        }
    }

    public void SetRoute(List<Waypoint> route, bool force = false)
    {
        if ((CurrentRoute.Count == 0 || force) == false)
        {
            return;
        }

        CurrentRoute.Clear();
        route.Reverse();
        CurrentRoute = new Queue<Waypoint>(route);
        TargetPoint = CurrentRoute.Dequeue();
    }

    IEnumerator Wait(float time)
    {
        animator.Play("Idle");
        // enemyAnimationStateController.CurrentState = EnemyState.Idle;
        yield return new WaitForSeconds(time);
    }

    void MoveToWaypoint(Waypoint waypoint)
    {
        tweener.AddTween(transform, transform.position, waypoint.position, Time.time, speed);
        if (transform.position.x < waypoint.position.x)
        {
            transform.localScale = basicScale;
        }
        else
        {
            transform.localScale = flipScale;
        }
        // ChangeState(EnemyState.Normal);
    }

    public void ChangeState(EnemyState newState)
    {
        // enemyAnimationStateController.CurrentState = state;
        switch (newState)
        {
            case EnemyState.Idle:
                animator.SetTrigger("Idle");
                break;
            case EnemyState.Normal:
                animator.SetTrigger("Normal");
                break;
            case EnemyState.Scared:
                animator.SetTrigger("Scare");
                break;
            case EnemyState.Dead:
                animator.SetTrigger("Dead");
                break;
        }

        emmitOnChangeState.Invoke(state, newState);
        state = newState;
    }

    public void ResetPosition()
    {
        CurrentRoute.Clear();
        TargetPoint = RespwanPoint;
        this.transform.position = RespwanPoint.position;
        emmitOnAllRouteCompleted.Invoke();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            pacStudentMovement.EmmitOnGhostHit.Invoke(this.gameObject);
        }
    }

    public void SetPause(bool state)
    {
        // Debug.Log(this.gameObject.name + " set pause " + state);
        isPaused = state;
    }
}