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
    private List<SerializedProperty> _events;
    private SerializedProperty _isTriggerProperty;
    private SerializedProperty _checkItemsProperty;
    private SerializedProperty _itemsProperty;
    private SerializedProperty _needAllItemsProperty;
    private void OnEnable()
    {
        _isTriggerProperty = serializedObject.FindProperty("isTrigger");
        _checkItemsProperty = serializedObject.FindProperty("checkItems");
        _itemsProperty = serializedObject.FindProperty("itemsToCheck");
        _needAllItemsProperty = serializedObject.FindProperty("needAllItems");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        
        AssignEvents();

        EditorGUILayout.PropertyField(_isTriggerProperty);

        EditorGUILayout.PropertyField(_checkItemsProperty);

        if (_checkItemsProperty.boolValue)
        {
            EditorGUILayout.PropertyField(_needAllItemsProperty);
            EditorGUILayout.PropertyField(_itemsProperty);
        }
        
        foreach (var eventProperty in _events)
        {
            EditorGUILayout.PropertyField(eventProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AssignEvents()
    {
        _events = new();

        if (_isTriggerProperty.boolValue)
        {
            _events.Add(serializedObject.FindProperty("onTriggerEnter"));
            _events.Add(serializedObject.FindProperty("onTriggerExit"));
        }
        else
        {
            _events.Add(serializedObject.FindProperty("onCollisionEnter"));
            _events.Add(serializedObject.FindProperty("onCollisionExit"));
        }

    }
}