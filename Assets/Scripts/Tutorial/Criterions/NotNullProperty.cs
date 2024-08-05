using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;

public class NotNullProperty : Criterion
{
    [SerializeField] private FutureObjectReference m_Target;

    [SerializeField] private string propertyPath = "";

    public override void StartTesting()
    {
        base.StartTesting();
        UpdateCompletion();

        EditorApplication.update += UpdateCompletion;        
    }

    public override void StopTesting()
    {
        base.StopTesting();

        EditorApplication.update -= UpdateCompletion;
    }

    protected override bool EvaluateCompletion()
    {
        if (m_Target.SceneObjectReference.ReferencedObject == null) return false;
        
        SerializedObject serializedObject = new SerializedObject(m_Target.SceneObjectReference.ReferencedObject);
        SerializedProperty serializedProperty = serializedObject.FindProperty(propertyPath);
        return serializedProperty.objectReferenceValue??false;

    }

    public override bool AutoComplete()
    {
        return true;
    }
}
