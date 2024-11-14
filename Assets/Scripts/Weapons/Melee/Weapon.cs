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
    [SerializeField] protected int damage;
    [SerializeField] protected float cooldown;

    [SerializeField] protected UnityEvent fireAction;

    private float currentCooldown = 0;

    protected bool Equipped = false;
    public bool IsEquipped => Equipped;
    
    /// <summary>
    /// Equips weapon and subcribes to fireAction
    /// </summary>
    public virtual void Equip()
    {
        Equipped = true;
        fireAction.AddListener(Fire);
    }
    /// <summary>
    /// Unequips and un-subscribes to fireAction
    /// </summary>
    public virtual void UnEquip()
    {
        Equipped = false;
        fireAction.RemoveListener(Fire);
    }
    
    /// <summary>
    /// Updates cooldown and controls input for invoking fireAction
    /// </summary>
    protected virtual void Update()
    {
        if (!Equipped) return;
        
        currentCooldown -= currentCooldown >= 0 ? Time.deltaTime : 0;
        bool leftClick = useLeftClick && Input.GetMouseButtonDown(0);
        if ((Input.GetKeyDown(fireButton) || leftClick) && CanFire()) fireAction.Invoke();
    }
    
    /// <summary>
    /// Base function for Fire, should be overriden to control what happens on fire
    /// </summary>
    protected virtual void Fire()
    {
        currentCooldown = cooldown;
    }
    
    /// <summary>
    /// function for if the cooldown has elapsed
    /// </summary>
    /// <returns>true if cooldown has elapsed</returns>
    protected virtual bool CanFire()
    {
        return currentCooldown <= 0f;
    }
}
