﻿using UnityEditor;

namespace Utility
{
    public static class EditorSceneUtility
    {
        /// <summary>
        /// Adds a Scene to Build Settings.
        /// </summary>
        /// <param name="scene">Scene to be added</param>
        /// <param name="enabled">if scene should be enabled in settings</param>
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
    
}