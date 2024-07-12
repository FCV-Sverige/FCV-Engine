using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [SerializeField] private KeyCode fireButton;
    [SerializeField] protected bool useLeftClick = false;
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldown;

    [SerializeField] protected UnityEvent fireAction;

    private float currentCooldown = 0;

    public bool Equipped;

    protected virtual void OnEnable()
    {
        fireAction.AddListener(Fire);
    }

    protected virtual void OnDisable()
    {
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
