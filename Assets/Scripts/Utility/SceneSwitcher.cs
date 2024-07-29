using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset _sceneAsset;
#endif

    private string sceneName;


    public void StartTransition(GameObject gameObject = null)
    {
        if (gameObject)
            DontDestroyOnLoad(gameObject);
        
        SceneManager.LoadScene(sceneName);
    }
#if UNITY_EDITOR
    
    private void OnValidate()
    {
        var sceneAssetPath = AssetDatabase.GetAssetPath(_sceneAsset);
        bool sceneAdded = false;
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (sceneAdded) continue;
            
            sceneAdded = scene.path == sceneAssetPath;
        }

        sceneName = _sceneAsset.name;
        
        if (sceneAdded) return;
        
        AddSceneToBuildSettings(_sceneAsset);
    }
    
    /// <summary>
    /// Adds a Scene to Build Settings.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="enabled"></param>
    private static void AddSceneToBuildSettings(SceneAsset scene, bool enabled = true)
    {
        var scenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length + 1];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
        {
            scenes[i] = EditorBuildSettings.scenes[i];
        }

        scenes[^1] = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(scene), enabled);

        EditorBuildSettings.scenes = scenes;
    }
#endif
}
