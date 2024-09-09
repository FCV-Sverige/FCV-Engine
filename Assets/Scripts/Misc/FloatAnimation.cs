using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    [SerializeField] private Vector2 floatEndPoint = new (0, -.5f);
    [SerializeField] private float animationTime = .5f;
    [SerializeField] private AnimationCurve easingCurve;

    private Vector2 startPoint;

    private bool isFloating = true;

    private void Awake()
    {
        startPoint = transform.position;
    }

    private void Start()
    {
        StartCoroutine(FloatAnimationLogic());
    }

    private IEnumerator FloatAnimationLogic()
    {
        float t = 0;
        while (isFloating)
        {
            t += Time.deltaTime;
            t %= animationTime;
            float alpha = Mathf.PingPong(t / animationTime * 2, 1f);
            alpha = easingCurve.keys.Length > 1 ? easingCurve.Evaluate(alpha) : alpha;
            
            transform.position = Vector2.Lerp(startPoint, startPoint + floatEndPoint, alpha);
            yield return new WaitForEndOfFrame();
        }
    }

    public void StopAnimation()
    {
        isFloating = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + floatEndPoint);
    }
}
