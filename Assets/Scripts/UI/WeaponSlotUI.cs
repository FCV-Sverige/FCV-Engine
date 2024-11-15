using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text slotPosition;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Image borderImage;

    /// <summary>
    /// Sets the image of the weapon slot
    /// </summary>
    /// <param name="weaponSprite">Sprite to be shown</param>
    public void SetImage(Sprite weaponSprite)
    {
        weaponImage.sprite = weaponSprite;
    }
    
    /// <summary>
    /// Sets the text to visualize what key needs to be pressed for equip
    /// </summary>
    /// <param name="key">index++ from weapon controller of weapon</param>
    public void SetSlotIndex(int key)
    {
        slotPosition.text = $"{key}";
    }

    public void SetBorderActiveState(bool active)
    {
        borderImage.enabled = active;
    }
    
    
}
