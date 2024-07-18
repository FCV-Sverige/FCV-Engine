using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Custom2DCollider))]
public class PressurePlate : MonoBehaviour
{
    [SerializeField] private UnityEvent Activated;
    [SerializeField] private float pushTime = .5f;
    [SerializeField] private bool useStandTime = false;

    private float currentPushTime = 0;
    private bool activated = false;

    private Vector2 startPosition;
    private Vector2 endPosition => startPosition + Vector2.down * .3f;

    private void Awake()
    {
        startPosition = transform.position;
    }

    public void OnStandingOn()
    {
        if (activated) return;
        print("standing On");
        currentPushTime += Time.deltaTime;

        if (!(currentPushTime >= pushTime))
        {
            PushPlateDown(Mathf.Clamp01(currentPushTime / pushTime));
            return;
        }
        print("Activated");
        activated = true;
        Activated?.Invoke();
    }

    private void PushPlateDown(float lerpFactor)
    {
        transform.position = Vector2.Lerp(startPosition, endPosition, lerpFactor);
    }
}
