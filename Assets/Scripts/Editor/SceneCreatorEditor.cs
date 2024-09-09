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
    
    private void OnGUI()
    {
        NewSceneName = EditorGUILayout.TextField(NameContent, NewSceneName);
        CopySceneAsset = (SceneAsset)EditorGUILayout.ObjectField(CopySceneAsset, typeof(SceneAsset));

        if (GUILayout.Button("Create"))
            CheckAndCreateScene();
    }

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

    protected void CreateScene()
    {
        string[] result = AssetDatabase.FindAssets("DemoScene");

        if (result.Length > 0)
        {
            string newScenePath = "Assets/Scenes/" + NewSceneName + ".unity";
            AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(result[0]), newScenePath);
            AssetDatabase.Refresh();
            Scene newScene = EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Single);
            AddSceneToBuildSettings(newScene);
            Close();
        }
        else
        {
            //Debug.LogError("The template scene <b>_TemplateScene</b> couldn't be found ");
            EditorUtility.DisplayDialog("Error",
                "The scene DemoScene was not found in Assets/Scenes folder. This scene is required by the Scene Creator.",
                "OK");
        }
    }

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