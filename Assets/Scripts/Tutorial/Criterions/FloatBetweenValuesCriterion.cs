using System.Collections;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;

public class FloatBetweenValuesCriterion : Criterion
{
    [SerializeField] private float minValue, maxValue;

    [SerializeField] private FutureObjectReference futureObjectReference;
    [SerializeField] private string propertyPath;

    private Object TargetObject => futureObjectReference.SceneObjectReference.ReferencedObject;
    
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
        SerializedProperty floatProperty = new SerializedObject(TargetObject).FindProperty(propertyPath);

        if (floatProperty == null)
        {
            Debug.Log("Property path returns null");
            return false;
        }

        return floatProperty.floatValue < maxValue && floatProperty.floatValue > minValue;
    }

    public override bool AutoComplete()
    {
        throw new System.NotImplementedException();
    }
}
