using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    public static class ScriptableObjectFinder
    {
        public static List<T> FindAssetsOfType<T>() where T : ScriptableObject
        {
            List<T> results = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset != null)
                {
                    results.Add(asset);
                    Debug.Log($"Found {typeof(T).Name}: {asset.name} at {path}");
                }
            }

            return results;
        }
    }
}