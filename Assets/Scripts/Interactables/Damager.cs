using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Damager : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float collisionDistance;
    [SerializeField] private float attackCooldown;

    [Space(15)] [Header("Direction Options"), Space(5)] 
    
    [SerializeField] private bool flippedBySpeed = false;
    [SerializeField, Range(0, 360)] private float fovAngle = 360;
    [SerializeField, Range(0, 360)] private float startAngle = 90;
    
    private new Collider2D collider2D;
    private Vector2 StartDirection => new(Mathf.Cos(CorrectedStartAngle * Mathf.Deg2Rad), Mathf.Sin(CorrectedStartAngle * Mathf.Deg2Rad));
    private float CorrectedStartAngle => xSignedDirection >= 0 ? startAngle : startAngle + 180;

    private bool canAttack = true;

    private Vector2 lastPosition;


    private int xSignedDirection = 1;

    private void Awake()
    {
        lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        xSignedDirection = flippedBySpeed ? (int) Mathf.Sign(((Vector2)transform.position - lastPosition).x) : 1;
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (!canAttack) return;
        
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, collisionDistance, layerMask);
        
        if (hitCollider)
            CollisionChecks(hitCollider);
    }

    private void DealDamage(Health health)
    {
        if (!health)
        {
            print("Health component is null");
            return;
        }
        health.RemoveHealth(damage);
        
        canAttack = false;
        
        Invoke(nameof(EnableAttack), attackCooldown);
    }

    private void EnableAttack()
    {
        canAttack = true;
    }

    private void CollisionChecks(Collider2D other)
    {
        if (!FOVUtility.IsWithinFOVAndRange(transform.position, other.transform.position, StartDirection, float.MaxValue, fovAngle))
        {
            print("Not in within FOV");
            return;
        }

        if (!other.TryGetComponent(out Health health))
        {
            print("Crazy");
            return;
        }
        
        DealDamage(health);
    }

    private void OnDrawGizmosSelected()
    {
        Color color;
        Gizmos.color = color = Color.red;
        color.a = .1f;
        Handles.color = color;
        
        Gizmos.DrawWireSphere(transform.position, collisionDistance);
        
        FOVUtility.DrawFOV(transform.position, StartDirection, collisionDistance, fovAngle);
    }
}
