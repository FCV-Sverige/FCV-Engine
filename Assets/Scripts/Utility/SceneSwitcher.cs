using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private SceneAsset _sceneAsset;


    public void StartTransition(GameObject gameObject = null)
    {
        if (gameObject)
            DontDestroyOnLoad(gameObject);
        
        SceneManager.LoadScene(_sceneAsset.name);
    }

    private void OnValidate()
    {
        var scenePath = AssetDatabase.GetAssetPath(_sceneAsset);
        bool sceneAdded = false;
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (sceneAdded) continue;
            
            sceneAdded = scene.path == scenePath;
        }
        
        if (sceneAdded) return;
        
        AddSceneToBuildSettings(_sceneAsset);
    }
    
    /// <summary>
    /// Adds a Scene to Build Settings.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="enabled"></param>
    public static void AddSceneToBuildSettings(SceneAsset scene, bool enabled = true)
    {
        var scenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length + 1];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
        {
            scenes[i] = EditorBuildSettings.scenes[i];
        }

        scenes[^1] = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(scene), enabled);

        EditorBuildSettings.scenes = scenes;
    }
}
