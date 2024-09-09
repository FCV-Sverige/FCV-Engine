using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ItemName : PropertyAttribute
{
    public ItemName()
    {
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ItemName))]
public class ItemNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ItemName ItemName = (ItemName)attribute;

        // Load the ScriptableObject containing the list of strings
        ItemDatabase itemDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabase>("Assets/EditorResources/ItemNameDatabase.asset");

        List<string> itemNames = itemDatabase.GetAllItemNames();

        if (itemDatabase != null && itemNames.Count > 0)
        {
            int selectedIndex = Mathf.Max(0, itemNames.IndexOf(property.stringValue));
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, itemNames.ToArray());
            property.stringValue = itemNames[selectedIndex];
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
            EditorGUILayout.HelpBox($"ItemDatabase asset not found at path: Assets/EditorResources/ItemNameDatabase.asset", MessageType.Error);
        }
    }
}
#endif
