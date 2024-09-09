using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Door : MonoBehaviour
{
    [SerializeField] private Vector2 doorOpenOffset;
    [SerializeField] private Animator animator;
    [SerializeField] private float animationTime = 1f;
    [SerializeField] protected UnityEvent doorOpeningStarted;
    [FormerlySerializedAs("doorClosed")] [SerializeField] protected UnityEvent doorClosingStarted;

    private Vector2 closedPosition;
    private Vector2 OpenPosition => closedPosition + doorOpenOffset;

    private bool moving = false;
    private bool opened = false;
    
    private void Awake()
    {
        closedPosition = transform.position;
    }

    public void StartDoorOpeningAnimation()
    {
        if (animator)
            animator.SetTrigger("OpenDoor");
    }

    public void OpenDoor()
    {
        if (opened | moving) return;
        doorOpeningStarted.Invoke();
        StartCoroutine(MoveDoor(closedPosition, OpenPosition));
        opened = true;
    }

    public void CloseDoor()
    {
        if (!opened | moving) return;
        doorClosingStarted.Invoke();
        StartCoroutine(MoveDoor(OpenPosition, closedPosition));
        opened = false;
    }

    private IEnumerator MoveDoor(Vector2 start, Vector2 end)
    {
        moving = true;
        float t = 0;
        float startTime = Time.time;

        while (t < 1f)
        {
            t = (Time.time - startTime) / animationTime;
            t = Mathf.Clamp01(t);
            t = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector2.Lerp(start, end, t);

            yield return new WaitForEndOfFrame();
        }

        moving = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((Vector2)transform.position + doorOpenOffset, Vector2.one);
    }
}
