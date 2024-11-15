using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private WeaponSlotUI slotPrefab;
    [SerializeField] private RectTransform weaponUIHolder;

    [SerializeField] private bool setupEventsAutomatically = false;

    private List<WeaponSlotUI> weaponSlotUis;

    private void SetupEventsAutomatically()
    {
        if (!setupEventsAutomatically) return;
        
        WeaponController weaponController = FindObjectOfType<WeaponController>();

        if (!weaponController) return;
        
        if (!MethodSubscribed(weaponController.weaponPickedUp, nameof(AddWeaponSlot)))
            weaponController.weaponPickedUp.AddListener(AddWeaponSlot);
        
        if (!MethodSubscribed(weaponController.weaponPickedUp, nameof(WeaponChanged)))
            weaponController.weaponChanged.AddListener(WeaponChanged);
        
    }

    private void Start()
    {
        weaponSlotUis = new List<WeaponSlotUI>();
        SetupEventsAutomatically();
    }
    
    /// <summary>
    /// Checks if a Unity Event has a certain method subscribed
    /// </summary>
    /// <param name="event">Unity Event to check against</param>
    /// <param name="methodName">Name of method to be checked</param>
    /// <returns></returns>
    private bool MethodSubscribed([NotNull] UnityEventBase @event, [NotNull] string methodName)
    {
        if (@event == null) throw new ArgumentNullException(nameof(@event));
        if (methodName == null) throw new ArgumentNullException(nameof(methodName));
        
        for (int i = 0; i < @event.GetPersistentEventCount(); i++)
        {
            if (@event.GetPersistentMethodName(i) == nameof(methodName)) return true;
        }

        return false;
    }
    
    /// <summary>
    /// Add and setups a weapon slot UI for weapon provided 
    /// </summary>
    /// <param name="weapon">Weapon to be added to UI</param>
    /// <param name="key">The key used to equip weapon when changing weapon</param>
    public void AddWeaponSlot(Weapon weapon, int key)
    {
        WeaponSlotUI weaponSlotUI = Instantiate(slotPrefab, weaponUIHolder);
        Sprite sprite;

        if (weapon.TryGetComponent(out SpriteRenderer spriteRenderer))
            sprite = spriteRenderer.sprite;
        else
        {
            sprite = weapon.GetComponentInChildren<SpriteRenderer>().sprite;
        }

        if (sprite)
            weaponSlotUI.SetImage(sprite);

        weaponSlotUI.SetSlotIndex(key);
        
        weaponSlotUis.Add(weaponSlotUI);
    }
    
    /// <summary>
    /// Activates border of the index provided and deactivates all other
    /// </summary>
    /// <param name="index">index of weapon</param>
    public void WeaponChanged(int index)
    {
        for (int i = 0; i < weaponSlotUis.Count; i++)
        {
            weaponSlotUis[i].SetBorderActiveState(i == index);
        }
    }

}
