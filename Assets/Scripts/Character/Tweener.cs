using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    // private Tween activeTween;
    private List<Tween> activeTweens = new List<Tween>();

    // public bool IsTweening => activeTweens.Count > 0;

    public bool TweenExists(Transform target)
    {
        return activeTweens.FindIndex(tween => tween.Target == target) != -1;
    }

    // Start is called before the first frame update

    // Update is called once per frame
    void FixedUpdate()
    {
        activeTweens.ForEach(onUpdateTween);
        activeTweens.RemoveAll(tween => tween.IsComplete);
    }

    public bool AddTween(
        Transform targetObject,
        Vector3 startPos,
        Vector3 endPos,
        float startTime,
        float duration = 1.0f,
        string easeType = "linear"
    )
    {
        // if (activeTween != null)
        // {
        //     Debug.LogWarning("Already tweening");
        //     return;
        // }
        if (TweenExists(targetObject))
        {
            return false;
        }
        activeTweens.Add(new Tween(targetObject, startPos, endPos, startTime, duration, easeType));
        return true;
    }

    void onUpdateTween(Tween tweenObj)
    {
        var distance = Vector3.Distance(tweenObj.Target.position, tweenObj.EndPos);
        if (distance > 0.1f)
        {
            // float timeFraction = (Time.time - activeTween.StartTime) / activeTween.Duration;
            float timeFraction = GetTimeFraction(
                tweenObj.StartTime,
                tweenObj.Duration,
                tweenObj.EaseType
            );
            tweenObj.Target.position = Vector3.Lerp(
                tweenObj.StartPos,
                tweenObj.EndPos,
                timeFraction
            );
        }
        else
        {
            tweenObj.Target.position = tweenObj.EndPos;
            tweenObj.IsComplete = true;
        }
    }

    float GetTimeFraction(float startTime, float duration, string easeType)
    {
        float progress = (Time.time - startTime) / duration;

        switch (easeType)
        {
            case "easeInSine":
                return easeInSine(progress);
            case "easeOutSine":
                return easeOutSine(progress);
            case "easeInOutSine":
                return easeInOutSine(progress);
            case "easeInQuad":
                return easeInQuad(progress);
            case "easeOutQuad":
                return easeOutQuad(progress);
            case "easeInOutQuad":
                return easeInOutQuad(progress);
            case "easeInCubic":
                return easeInCubic(progress);
            case "easeOutCubic":
                return easeOutCubic(progress);
            case "easeInOutCubic":
                return easeInOutCubic(progress);
            case "easeInQuart":
                return easeInQuart(progress);
            case "easeOutQuart":
                return easeOutQuart(progress);
            case "easeInOutQuart":
                return easeInOutQuart(progress);
            case "linear":
            default:
                return linear(progress);
        }
    }

    float linear(float x) => x;

    float easeInSine(float x) => 1 - Mathf.Cos((x * Mathf.PI) / 2);

    float easeOutSine(float x) => Mathf.Sin((x * Mathf.PI) / 2);

    float easeInOutSine(float x) => -(Mathf.Cos(Mathf.PI * x) - 1) / 2;

    float easeInQuad(float x) => x * x;

    float easeOutQuad(float x) => 1 - (1 - x) * (1 - x);

    float easeInOutQuad(float x) => x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;

    float easeInCubic(float x) => x * x * x;

    float easeOutCubic(float x) => 1 - Mathf.Pow(1 - x, 3);

    float easeInOutCubic(float x) => x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;

    float easeInQuart(float x) => x * x * x * x;

    float easeOutQuart(float x) => 1 - Mathf.Pow(1 - x, 4);

    float easeInOutQuart(float x) => x < 0.5 ? 8 * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 4) / 2;
}
