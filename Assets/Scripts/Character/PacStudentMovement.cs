using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PacStudentMovement : MonoBehaviour
{
    public float speed = 2.4f;

    // public List<Transform> waypoints = new List<Transform>();
    private Vector3 targetPosition;

    private Animator animator;
    private Tweener tweener;
    private AudioSource audioSource;
    private ParticleSystem dustParticleSystem;

    [SerializeField] private AudioClip footstepSound,
        eatSound,
        dieSound;

    private Vector3 basicScale,
        flipScale;

    public UnityEvent<GameObject> EmmitOnItemHit;
    public UnityEvent<GameObject> EmmitOnGhostHit;
    public bool Arrived()
    {
        var distance = Vector3.Distance(transform.position, targetPosition);
        return distance < 0.01f;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        tweener = GetComponent<Tweener>();
        audioSource = GetComponent<AudioSource>();
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
        var dust = this.transform.Find("DustSmoke");
        dustParticleSystem = dust.GetComponent<ParticleSystem>();
        dustParticleSystem.Stop();
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
            dustParticleSystem.Stop();
        }
        else
        {
            animator.Play("Walk");
            animator.speed = 2f;
            HandleFootstep();
            MoveToWaypoint(targetPosition);
            dustParticleSystem.Play();
        }
    }

    void MoveToWaypoint(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);
        if (transform.position.x < position.x)
        {
            transform.localScale = basicScale;
        }
        else
        {
            transform.localScale = flipScale;
        }
    }


    public void SetTargetPosition(Waypoint wp)
    {
        targetPosition = wp.position;
    }
    
    public void ResetPosition()
    {
        // transform.position = targetPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ghost"))
        {
            EmmitOnGhostHit.Invoke(other.gameObject);
        }
        else if (other.CompareTag("item"))
        {
            audioSource.PlayOneShot(eatSound);
            EmmitOnItemHit.Invoke(other.gameObject);
        }
    }


    // audio handle
    void HandleFootstep()
    {
        audioSource.PlayOneShot(footstepSound);
    }

    public void DeadTarget()
    {
        audioSource.PlayOneShot(dieSound);
    }
}