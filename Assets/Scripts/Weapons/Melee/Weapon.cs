using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(FloatAnimation))]
public class Weapon : MonoBehaviour
{
    [SerializeField] private KeyCode fireButton;
    [SerializeField] protected bool useLeftClick = false;
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldown;

    [SerializeField] protected UnityEvent fireAction;

    private float currentCooldown = 0;

    protected bool Equipped = false;
    public bool IsEquipped => Equipped;

    public virtual void Equip()
    {
        Equipped = true;
        fireAction.AddListener(Fire);
    }

    public virtual void UnEquip()
    {
        Equipped = false;
        fireAction.RemoveListener(Fire);
    }

    protected virtual void Update()
    {
        if (!Equipped) return;
        
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
