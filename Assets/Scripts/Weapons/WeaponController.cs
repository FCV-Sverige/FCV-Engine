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
            if (!weapons[i].CanBeEquipped | weapons[i].IsEquipped | Vector2.Distance(transform.position, weapons[i].transform.position) > pickupDistance) continue;
            
            SetCurrentWeapon(weapons[i]);
        }
    }


    private void SetCurrentWeapon(Weapon weapon)
    {
        if (equippedWeapon)
        {
            equippedWeapon.IsEquipped = false;
            equippedWeapon.UnEquip();
        }
        
        weaponParentTransform.DetachChildren();
        equippedWeapon = weapon;
        weapon.IsEquipped = true;
        weapon.transform.SetParent(weaponParentTransform);
        weapon.transform.localPosition = Vector2.zero;
        weapon.Equip();
    }
}
