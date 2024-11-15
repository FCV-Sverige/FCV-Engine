using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float parallaxEffect;
    private float length, startPosition;
    private Transform cameraTransform;


    private void Start()
    {
        startPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        if (Camera.main != null) cameraTransform = Camera.main.transform;
    }
    
    /// <summary>
    /// Moves backgrounds in parallax effect and places them acording for seamless scrolling
    /// </summary>
    private void Update()
    {
        float temp = (cameraTransform.position.x * (1 - parallaxEffect));
        float dist = (cameraTransform.position.x * parallaxEffect);

        transform.position = new Vector3(startPosition + dist, transform.position.y, transform.position.z);
        
        // if they extend beyond length of background sprite move one unit in corresponding direction for seamless scrolling
        if (temp > startPosition + length) startPosition += length;
        else if (temp < startPosition - length) startPosition -= length;
    }
}
