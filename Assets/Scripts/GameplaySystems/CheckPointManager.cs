using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [SerializeField] private GameObject checkPointGameObject;
    [SerializeField] private Transform playerTransform;
    [HideInInspector,SerializeField] private List<Vector2> checkPoints;
    [SerializeField] private float checkPointAcquiredDistance = 1;

    private Vector2 lastCheckpoint;

    public List<Vector2> CheckPoints => checkPoints;

    private void Awake()
    {
        if (checkPointGameObject == null) return;
        
        foreach (var checkPoint in checkPoints)
        {
            Instantiate(checkPointGameObject, checkPoint, quaternion.identity);
        }
    }

    private void Update()
    {
        for (int i = checkPoints.Count - 1; i >= 0; i--)
        {
            if (Vector2.Distance(playerTransform.position, checkPoints[i]) > checkPointAcquiredDistance) continue;

            lastCheckpoint = checkPoints[i];
            checkPoints.RemoveAt(i);
        }
    }

    public void PlaceAtCheckPoint()
    {
        playerTransform.position = lastCheckpoint;
    }
}

[CustomEditor(typeof(CheckPointManager))]
public class CheckPointManagerEditor : Editor
{
    private CheckPointManager checkPointManager;
    private SerializedProperty checkPointsProperty;
    private List<Vector2> checkPoints;

    private void OnEnable()
    {
        checkPointsProperty = serializedObject.FindProperty("checkPoints");
        checkPointManager = target as CheckPointManager;
    }

    private void OnSceneGUI()
    {
        MakePositionHandles();
    }
    
    private void MakePositionHandles()
    {
        checkPoints = checkPointManager.CheckPoints;
        if (checkPoints.Count <= 0)
            return;
        
        for (int i = 0; i < checkPoints.Count; i++)
        {
            // produces a position handle to easily assign positions for moving platform
            
            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = Handles.PositionHandle(checkPoints[i], Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(checkPointManager, "Move Position Handle");
                checkPoints[i] = newPosition;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        serializedObject.Update();
        EditorGUILayout.PropertyField(checkPointsProperty, true);
        serializedObject.ApplyModifiedProperties();
    }
}
