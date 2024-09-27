using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1.2f;

    // public List<Transform> waypoints = new List<Transform>();
    private Vector3 targetPosition;

    private Animator animator;
    private Tweener tweener;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip footstepSound,
        eatSound,
        dieSound;

    private Vector3 basicScale,
        flipScale;

    public bool Arrived()
    {
        if (targetPosition == null)
        {
            return true;
        }
        var distance = Vector3.Distance(transform.position, targetPosition);
        return distance < 0.1f;
    }

    void Start()
    {
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMove();
    }

    private void UpdateMove()
    {
        if (Arrived())
        {
            animator.Play("Idle");
        }
        else
        {
            MoveToWaypoint(targetPosition);
        }
    }

    void MoveToWaypoint(Vector3 position)
    {
        tweener.AddTween(transform, transform.position, position, Time.time, speed);
        if (transform.position.x < position.x)
        {
            transform.localScale = basicScale;
        }
        else
        {
            transform.localScale = flipScale;
        }
        animator.Play("Walk");
    }

    public void SetTargetWaypoint(Vector3 waypoint)
    {
        targetPosition = waypoint;
    }
}
