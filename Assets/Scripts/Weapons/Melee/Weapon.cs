using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    [SerializeField] private KeyCode fireButton;
    [SerializeField] protected bool useLeftClick = false;
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldown;

    [SerializeField] protected UnityEvent fireAction;

    private float currentCooldown = 0;

    public bool IsEquipped;
    
    public virtual void Equip()
    {
        fireAction.AddListener(Fire);
    }

    public virtual void UnEquip()
    {
        fireAction.RemoveListener(Fire);
        transform.position -= (Vector3)Vector2.left * 3;
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
}
