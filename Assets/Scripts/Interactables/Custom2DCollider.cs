using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
class Custom2DCollider : MonoBehaviour
{
    [HideInInspector, SerializeField] private bool isTrigger = false;
    [HideInInspector, SerializeField] private bool checkItems = false;
    [HideInInspector, SerializeField] private bool needAllItems = false;
    [HideInInspector, SerializeField] private List<string> itemsToCheck;
    
    [HideInInspector, SerializeField] private UnityEvent onCollisionEnter;

    [HideInInspector, SerializeField] private UnityEvent onCollisionExit;
    
    [HideInInspector, SerializeField] private UnityEvent onTriggerEnter;
    
    [HideInInspector, SerializeField] private UnityEvent onTriggerExit;
    

    private void Awake()
    {
        if (TryGetComponent(out Collider2D collider2D))
        {
            collider2D.isTrigger = isTrigger;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FireEvent(ref onTriggerEnter, other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        FireEvent(ref onTriggerExit, other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        FireEvent(ref onCollisionEnter, other.gameObject);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        FireEvent(ref onCollisionExit, other.gameObject);
    }

    private void FireEvent(ref UnityEvent unityEvent, GameObject collidingObject)
    {
        if (!checkItems)
            unityEvent.Invoke();
        
        if (collidingObject.TryGetComponent(out Inventory inventory) && CheckItems(inventory))
            unityEvent.Invoke();
    }

    private bool CheckItems(Inventory inventory)
    {
        foreach (var item in itemsToCheck)
        {
            bool hasItem = inventory.HasItem(item);
            
            if (hasItem && !needAllItems) return true;

            if (!hasItem && needAllItems) return false;
        }

        return true;
    }
}

[CustomEditor(typeof(Custom2DCollider))]
public class Custom2DColliderEditor : Editor
{
    private List<SerializedProperty> events;
    private SerializedProperty isTriggerProperty;
    private SerializedProperty checkItemsProperty;
    private SerializedProperty itemsProperty;
    private SerializedProperty needAllItemsProperty;
    private void OnEnable()
    {
        isTriggerProperty = serializedObject.FindProperty("isTrigger");
        checkItemsProperty = serializedObject.FindProperty("checkItems");
        itemsProperty = serializedObject.FindProperty("itemsToCheck");
        needAllItemsProperty = serializedObject.FindProperty("needAllItems");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        
        AssignEvents();

        EditorGUILayout.PropertyField(isTriggerProperty);

        EditorGUILayout.PropertyField(checkItemsProperty);

        if (checkItemsProperty.boolValue)
        {
            EditorGUILayout.PropertyField(needAllItemsProperty);
            EditorGUILayout.PropertyField(itemsProperty);
        }
        
        foreach (var eventProperty in events)
        {
            EditorGUILayout.PropertyField(eventProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AssignEvents()
    {
        events = new();

        if (isTriggerProperty.boolValue)
        {
            events.Add(serializedObject.FindProperty("onTriggerEnter"));
            events.Add(serializedObject.FindProperty("onTriggerExit"));
        }
        else
        {
            events.Add(serializedObject.FindProperty("onCollisionEnter"));
            events.Add(serializedObject.FindProperty("onCollisionExit"));
        }

    }
}