using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10;

    [SerializeField] private float lifetime = 5;

    [SerializeField] private LayerMask hittableLayer;

    public UnityEvent<Health, Vector2> hitAction;
    private void Awake()
    {
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;

        if (lifetime < 0)
        {
            Destroy(gameObject);
        }
        
        transform.position += transform.right * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!LayerMaskUtility.IsInLayerMask(other.gameObject, hittableLayer)) return;
        if (other.gameObject.TryGetComponent(out Health health))
        {
            hitAction.Invoke(health, other.transform.position - transform.position);
        }
        Destroy(gameObject);
    }
}
