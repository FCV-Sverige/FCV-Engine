using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Weapon : MonoBehaviour
{
    [SerializeField] private KeyCode fireButton;
    [SerializeField] protected bool useLeftClick = false;
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldown;

    [SerializeField] protected UnityEvent fireAction;

    private float currentCooldown = 0;

    public bool CanBeEquipped = true;
    public bool IsEquipped;

    protected Collider2D weaponCollider2D;
    protected Rigidbody2D rigidbody2D;

    protected virtual void Awake()
    {
        TryGetComponent(out weaponCollider2D);
        TryGetComponent(out rigidbody2D);

        weaponCollider2D.isTrigger = false;
        rigidbody2D.isKinematic = false;
    }

    public virtual void Equip()
    {
        fireAction.AddListener(Fire);
        weaponCollider2D.enabled = false;
        rigidbody2D.isKinematic = true;
    }

    public virtual void UnEquip()
    {
        CanBeEquipped = false;
        fireAction.RemoveListener(Fire);
        if (weaponCollider2D && rigidbody2D)
        {
            transform.position += Vector3.up;
            weaponCollider2D.enabled = true;
            weaponCollider2D.isTrigger = false;
            rigidbody2D.isKinematic = false;
            
            bool leftDirection = Random.value > .5;
            Vector2 direction = GetRandomDirectionInCone(90 + (leftDirection ? 22.5f : -22.5f), 12);
            
            Debug.DrawLine(transform.position, (Vector2)transform.position + direction * 5, Color.red, 5);
            rigidbody2D.AddForce(direction * 5, ForceMode2D.Impulse);
        }

        StartCoroutine(EquipBuffer());
    }

    private IEnumerator EquipBuffer()
    {
        yield return new WaitForSeconds(1);
        CanBeEquipped = true;
    }

    protected virtual void Update()
    {
        if (!IsEquipped) return;
        
        currentCooldown -= currentCooldown >= 0 ? Time.deltaTime : 0;
        bool leftClick = useLeftClick && Input.GetMouseButtonDown(0);
        if ((Input.GetKeyDown(fireButton) || leftClick) && CanFire()) fireAction.Invoke();
    }

    protected virtual void Fire()
    {
        currentCooldown = cooldown;
    }

    protected virtual bool CanFire()
    {
        return currentCooldown <= 0f;
    }
    
    public Vector2 GetRandomDirectionInCone(float angle, float coneAngle)
    {
        // Random angle within the cone
        float halfConeAngle = coneAngle / 2f;
        float randomAngle = Random.Range(-halfConeAngle, halfConeAngle);
        
        // Total angle in radians
        float totalAngle = angle + randomAngle;

        // Convert angle to direction
        float rad = totalAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        
        return direction.normalized;
    }
}
