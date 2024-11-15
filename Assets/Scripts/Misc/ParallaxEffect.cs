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

    private void Update()
    {
        float temp = (cameraTransform.position.x * (1 - parallaxEffect));
        if (gameObject.name == "3")
            print($"{temp}, {startPosition + length}");
        float dist = (cameraTransform.position.x * parallaxEffect);

        transform.position = new Vector3(startPosition + dist, transform.position.y, transform.position.z);

        if (temp > startPosition + length) startPosition += length;
        else if (temp < startPosition - length) startPosition -= length;
    }
}
