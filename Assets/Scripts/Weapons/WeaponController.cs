using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform weaponParentTransform;
    [SerializeField] private float pickupDistance = 1f; 
        
        
    private Weapon equippedWeapon;

    private Weapon[] weapons;
    private void Start()
    {
        weapons = FindObjectsOfType<Weapon>();
    }

    private void Update()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].Equipped | Vector2.Distance(transform.position, weapons[i].transform.position) > pickupDistance) continue;
            
            SetCurrentWeapon(weapons[i]);
        }
    }


    private void SetCurrentWeapon(Weapon weapon)
    {
        if (equippedWeapon)
        {
            equippedWeapon.Equipped = false;
        }
        
        weaponParentTransform.DetachChildren();
        equippedWeapon = weapon;
        weapon.Equipped = true;
        weapon.transform.SetParent(weaponParentTransform);
        weapon.transform.localPosition = Vector2.zero;
    }
}
