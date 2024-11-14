using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public class MainMenuHandler : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset _sceneAsset;
#endif

    private string scenePath;
    
    public void StartGame()
    {
        SceneManager.LoadScene(scenePath);
    }

    public void QuitGame()
    {
        Application.Quit();
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

        scenePath = sceneAssetPath;
        
        if (sceneAdded) return;
        
        EditorSceneUtility.AddSceneToBuildSettings(_sceneAsset);
    }
#endif
}
