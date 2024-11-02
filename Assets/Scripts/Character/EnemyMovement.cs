using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

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

    public List<Transform> waypoints = new List<Transform>();
    [SerializeField] private int currentWaypoint = 0;

    public Waypoint RespwanPoint;

    private Animator animator;
    // private EnemyAnimationStateController enemyAnimationStateController;
    private Tweener tweener;


    [SerializeField] public EnemyState state = EnemyState.Idle;

    private Vector3 basicScale, flipScale;

    public bool isPaused = true;
    PacStudentMovement pacStudentMovement;


    public bool Arrived()
    {
        if (waypoints.Count == 0)
        {
            return false;
        }

        var distance = Vector3.Distance(transform.position, waypoints[currentWaypoint].position);
        return distance < 0.05f;
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

    public virtual void UpdateMove()
    {
        if (Arrived())
        {
            StartCoroutine(Wait(1.0f));
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Count)
            {
                currentWaypoint = 0;
            }
        }
        else
        {
            if (waypoints.Count == 0)
                return;
            MoveToWaypoint(waypoints[currentWaypoint]);
        }
    }

    IEnumerator Wait(float time)
    {
        animator.Play("Idle");
        // enemyAnimationStateController.CurrentState = EnemyState.Idle;
        yield return new WaitForSeconds(time);
    }

    void MoveToWaypoint(Transform waypoint)
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

        ChangeState(EnemyState.Normal);
    }

    public void ChangeState(EnemyState newState)
    {
        // enemyAnimationStateController.CurrentState = state;
        switch (newState)
        {
            case EnemyState.Idle:
                // animator.Play("Idle");
                animator.SetTrigger("Idle");
                break;
            case EnemyState.Normal:
                animator.SetTrigger("Normal");
                // animator.Play("Walk");
                break;
            case EnemyState.Scared:
                animator.SetTrigger("Scare");
                // animator.Play("Scare");
                break;
            case EnemyState.Dead:
                animator.SetTrigger("Dead");
                // animator.Play("Dead");
                break;
        }
        state = newState;

    }

    public void ResetPosition()
    {
        this.transform.position = RespwanPoint.position;
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
        isPaused = state;
    }
    
}