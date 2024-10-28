using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween
{
    public Transform Target { get; private set; }
    public Vector3 StartPos { get; private set; }
    public Vector3 EndPos { get; private set; }
    public float StartTime { get; private set; }
    public float Duration { get; private set; } = 1.0f;
    public string EaseType { get; private set; } = "linear";

    public bool IsComplete { get; set; } = false;

    public Tween(
        Transform target,
        Vector3 startPos,
        Vector3 endPos,
        float startTime,
        float duration = 1.0f,
        string easeType = "linear"
    )
    {
        this.Target = target;
        this.StartPos = startPos;
        this.EndPos = endPos;
        this.StartTime = startTime;
        this.Duration = duration;
        this.EaseType = easeType;
    }
}
