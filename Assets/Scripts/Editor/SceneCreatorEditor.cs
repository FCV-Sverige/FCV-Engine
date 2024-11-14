using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCreatorEditor : EditorWindow
{
    protected string NewSceneName;

    protected SceneAsset CopySceneAsset;

    protected readonly GUIContent NameContent = new GUIContent("New Scene Name");

    [MenuItem("Tools/Create New Scene...", priority = 100)]
    private static void Init()
    {
        SceneCreatorEditor window = GetWindow<SceneCreatorEditor>();
        window.Show();
        window.NewSceneName = "NewScene";
    }

    private void CreateGUI()
    {
        string[] results = AssetDatabase.FindAssets("DemoScene");
        
        if (results.Length <= 0) return;
        
        SceneAsset demoScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(results[0]));
        CopySceneAsset = demoScene;
    }

    private void OnGUI()
    {
        NewSceneName = EditorGUILayout.TextField(NameContent, NewSceneName);
        
        CopySceneAsset = (SceneAsset)EditorGUILayout.ObjectField(CopySceneAsset, typeof(SceneAsset), false);

        if (GUILayout.Button("Create"))
            CheckAndCreateScene();
    }
    
    /// <summary>
    /// Checks wether the current scene is not saved and prompts user to save or not before new scene is created and opened
    /// </summary>
    protected void CheckAndCreateScene()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Cannot create scenes while in play mode.  Exit play mode first.");
            return;
        }

        if (string.IsNullOrEmpty(NewSceneName))
        {
            Debug.LogWarning("Please enter a scene name before creating a scene.");
            return;
        }

        Scene currentActiveScene = SceneManager.GetActiveScene();

        if (currentActiveScene.isDirty)
        {
            string title = currentActiveScene.name + " Has Been Modified";
            string message = "Do you want to save the changes you made to " + currentActiveScene.path +
                             "?\nChanges will be lost if you don't save them.";
            int option = EditorUtility.DisplayDialogComplex(title, message, "Save", "Don't Save", "Cancel");

            if (option == 0)
            {
                EditorSceneManager.SaveScene(currentActiveScene);
            }
            else if (option == 2)
            {
                return;
            }
        }

        CreateScene();
    }
    
    /// <summary>
    /// Creates a new scene using the base fields in the class by copying a specified scene
    /// </summary>
    protected void CreateScene()
    {
        if (!CopySceneAsset)
        {
            EditorUtility.DisplayDialog("Error",
                "No scene to copy was assigned, this needs to be assigned so that we have a base scene to copy from",
                "OK");
            return;
        }
        
        string newScenePath = "Assets/Scenes/" + NewSceneName + ".unity";
        
        CopyAndMakeNewScene(AssetDatabase.GetAssetPath(CopySceneAsset), newScenePath);
        Scene newScene = EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Single);
        AddSceneToBuildSettings(newScene);
        Close();
    }

    /// <summary>
    /// Creates a new scene at the newScenePath using the path from the old scene
    /// </summary>
    /// <param name="path">Path to scene that is to be copied</param>
    /// <param name="newScenePath">Path for the new scene</param>
    protected void CopyAndMakeNewScene(string path, string newScenePath)
    {
        AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(path), newScenePath);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Adds the new scene to the build settings so it is included in build
    /// </summary>
    /// <param name="scene">Scene to be added to the build settings</param>
    protected void AddSceneToBuildSettings(Scene scene)
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        EditorBuildSettingsScene[] newBuildScenes = new EditorBuildSettingsScene[buildScenes.Length + 1];
        for (int i = 0; i < buildScenes.Length; i++)
        {
            newBuildScenes[i] = buildScenes[i];
        }

        newBuildScenes[buildScenes.Length] = new EditorBuildSettingsScene(scene.path, true);
        EditorBuildSettings.scenes = newBuildScenes;
    }
}