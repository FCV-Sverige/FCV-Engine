using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class PressureButton : MonoBehaviour
{
    [SerializeField] private float standTime = 1;
    [SerializeField] private LayerMask layerMask;
    // if the buttons is supposed to bounce back if button is not fully pressed when player leaves it
    [SerializeField] private bool bounceBack; 
    [SerializeField] private float pushedYOffset = -.5f;

    public UnityEvent buttonFired;
    // invokes event whenever the state is changed of the button, used for animations
    public UnityEvent<float> percentilePushed; 

    private float time = 0;
    private bool isButtonFired = false;
    private bool standingOnButton;
    private Vector2 startPosition;
    private float WorldYOffset => startPosition.y + pushedYOffset;

    private void Awake()
    {
        startPosition = transform.position;
        percentilePushed.AddListener(AnimateOffset);
    }
    
    private void Update()
    {
        if (standingOnButton || isButtonFired) return;
        if (!bounceBack) return;

        if (time <= 0) return;
        // bounces back to original state if player is not standing anymore and buttonfired has not been activated
        time -= Time.deltaTime;
        time = Mathf.Clamp(time, 0, standTime);
        percentilePushed.Invoke(Mathf.Clamp01(time/standTime));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!LayerMaskUtility.IsInLayerMask(other.gameObject, layerMask) || isButtonFired) return;

        standingOnButton = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!LayerMaskUtility.IsInLayerMask(other.gameObject, layerMask) || isButtonFired) return;

        standingOnButton = true;
    }
    // If play is inside trigger it starts pushing the button down
    private void OnTriggerStay2D(Collider2D other)
    { 
        if (!LayerMaskUtility.IsInLayerMask(other.gameObject, layerMask) || isButtonFired) return;

        time += Time.deltaTime;
        
        percentilePushed.Invoke(Mathf.Clamp01(time/standTime));
        
        if (time < standTime) return;

        isButtonFired = true;
        buttonFired.Invoke();
    }
    
    /// <summary>
    /// Moves the position to the WorldYOffset from the startPosition.y using a percentile(0-1)
    /// </summary>
    /// <param name="percentile"></param>
    private void AnimateOffset(float percentile)
    {
        float startY = startPosition.y;

        float currentY = Mathf.Lerp(startY, WorldYOffset, percentile);
        Vector2 currentPosition = transform.position;
        currentPosition.y = currentY;
        transform.position = currentPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(new Vector3(0, pushedYOffset, 0), .1f);
    }
}
