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
    
    [HideInInspector, SerializeField] private UnityEvent onCollisionEnter;

    [HideInInspector, SerializeField] private UnityEvent onCollisionExit;
    
    [HideInInspector, SerializeField] private UnityEvent onTriggerEnter;
    
    [HideInInspector, SerializeField] private UnityEvent onTriggerExit;

    public void Test()
    {
        Debug.Log("Test print");
    }

    private void Awake()
    {
        if (TryGetComponent(out Collider2D collider2D))
        {
            collider2D.isTrigger = isTrigger;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        onTriggerExit.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        onCollisionEnter.Invoke();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        onCollisionExit.Invoke();
    }
}

[CustomEditor(typeof(Custom2DCollider))]
public class Custom2DColliderEditor : Editor
{
    private List<SerializedProperty> events;
    private SerializedProperty isTriggerProperty;
    private void OnEnable()
    {
        isTriggerProperty = serializedObject.FindProperty("isTrigger");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
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

        EditorGUILayout.PropertyField(isTriggerProperty);
        
        foreach (var eventProperty in events)
        {
            EditorGUILayout.PropertyField(eventProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }
}