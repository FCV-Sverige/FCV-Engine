using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform weaponParentTransform;
    [SerializeField] private float pickupDistance = 1f;


    private int currentIndex;
    private List<Weapon> equippedWeapons = new();
    
    
    private List<Weapon> weapons;

    private static readonly KeyCode[] NumberKeys =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9
    };

    private void Start()
    {
        weapons = FindObjectsOfType<Weapon>().ToList();
    }

    private void Update()
    {
        for (int i = weapons.Count - 1; i >= 0; i--)
        {
            if (Vector2.Distance(transform.position, weapons[i].transform.position) > pickupDistance) continue;
            
            AddWeapon(weapons[i]);
            SetActiveWeapon(equippedWeapons.Count - 1);
        }
        
        CheckForInput();
    }


    private void AddWeapon(Weapon weapon)
    {
        weapon.transform.SetParent(weaponParentTransform);
        weapon.transform.localPosition = Vector2.zero;
        weapon.gameObject.SetActive(false);
        
        equippedWeapons.Add(weapon);
        weapons.Remove(weapon);
    }

    private void CheckForInput()
    {
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            if (!Input.GetKeyDown(NumberKeys[i])) continue;
            
            SetActiveWeapon(i);
        }
    }

    private void SetActiveWeapon(int index)
    {
        equippedWeapons[currentIndex].UnEquip();
        equippedWeapons[currentIndex].gameObject.SetActive(false);
            
        currentIndex = index;
            
        equippedWeapons[currentIndex].gameObject.SetActive(true);
        equippedWeapons[currentIndex].Equip();
    }
}
