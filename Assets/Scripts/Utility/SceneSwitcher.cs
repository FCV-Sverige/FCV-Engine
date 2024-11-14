#if UNITY_EDITOR
using UnityEditor;
using Utility;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SceneSwitcher : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset _sceneAsset;
#endif

    private string scenePath;

    /// <summary>
    /// Starts the switching of scenes and makes an object not be destroyed on load if that is required
    /// </summary>
    /// <param name="transitioningGameObject">gameobject not to be destroyed</param>
    public void StartTransition(GameObject transitioningGameObject = null)
    {
        if (transitioningGameObject)
            DontDestroyOnLoad(transitioningGameObject);
        
        SceneManager.LoadScene(scenePath);
    }
#if UNITY_EDITOR
    
    /// <summary>
    /// If scene asset path is not in build settings: add it
    /// </summary>
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
