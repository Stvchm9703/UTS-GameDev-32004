using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public enum EnemyState
{
    Normal,
    Scared,
    Dead,
    Revive,
}

public class EnemyMovement : MonoBehaviour
{
    public float speed = 1.0f;
    public List<Transform> waypoints = new List<Transform>();

    private Animator animator;
    private Tweener tweener;

    [SerializeField]
    private int currentWaypoint = 0;

    [SerializeField]
    public EnemyState state = EnemyState.Normal;

    private Vector3 basicScale, flipScale;

    public bool Arrived()
    {
        if (waypoints.Count == 0)
        {
            return false;
        }
        var distance = Vector3.Distance(transform.position, waypoints[currentWaypoint].position);
        return distance < 0.1f;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        tweener = GetComponent<Tweener>();
        if (tweener == null)
            tweener = gameObject.AddComponent<Tweener>();

        basicScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        flipScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
    }

    private void UpdateMove()
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
            MoveToWaypoint(waypoints[currentWaypoint]);
        }
    }

    IEnumerator Wait(float time)
    {
        animator.Play("Idle");
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

    void ChangeState(EnemyState newState)
    {
        state = newState;
        switch (state)
        {
            case EnemyState.Normal:
                animator.Play("Walk");
                break;
            case EnemyState.Scared:
                animator.Play("Scared");
                break;
            case EnemyState.Dead:
                animator.Play("Dead");
                break;
            case EnemyState.Revive:
                animator.Play("Revive");
                break;
        }
    }
}
