using System.Collections;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;

public class TransformChangedCriterion : Criterion
{
    
    [SerializeField]
    private ObjectReference objectReference = new ObjectReference();

    private Transform Transform => ((GameObject)objectReference.SceneObjectReference.ReferencedObject).transform;

    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;
    
    public override void StartTesting()
    {
        base.StartTesting();
        UpdateCompletion();

        position = Transform.position;
        rotation = Transform.rotation;
        scale = Transform.localScale;

        EditorApplication.update += UpdateCompletion;
    }
    
    public override void StopTesting()
    {
        base.StopTesting();

        EditorApplication.update -= UpdateCompletion;
    }

    protected override bool EvaluateCompletion()
    {
        return position != Transform.position || rotation != Transform.rotation || scale != Transform.localScale;
    }

    public override bool AutoComplete()
    {
        return true;
    }
}
