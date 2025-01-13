using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using Utility;

public static class EventListenerAdder
{
    public static void AddListenerToScriptableObjects(Tutorial[] eventAssets, RefreshMasking targetAsset)
    {
        foreach (var asset in eventAssets)
        {
            if (asset == null) continue;
            
                // Add the persistent listener
                UnityEventTools.AddPersistentListener(asset.PageInitiated, targetAsset.StartConstantRefreshing);
                UnityEventTools.AddPersistentListener(asset.Completed, targetAsset.StopConstantRefreshing);
                UnityEventTools.AddPersistentListener(asset.GoingBack, targetAsset.StopConstantRefreshing);
                UnityEventTools.AddPersistentListener(asset.Quit, targetAsset.StopConstantRefreshing);
                asset.PageInitiated.SetPersistentListenerState(asset.PageInitiated.GetPersistentEventCount() - 1, UnityEventCallState.EditorAndRuntime);
                asset.Completed.SetPersistentListenerState(asset.Completed.GetPersistentEventCount() - 1, UnityEventCallState.EditorAndRuntime);
                asset.GoingBack.SetPersistentListenerState(asset.GoingBack.GetPersistentEventCount() - 1, UnityEventCallState.EditorAndRuntime);
                asset.Quit.SetPersistentListenerState(asset.Quit.GetPersistentEventCount() - 1, UnityEventCallState.EditorAndRuntime);

                // Mark the asset as dirty so changes are saved
                EditorUtility.SetDirty(asset);
                Debug.Log($"Added listener to {asset.name}");
        }

        // Save changes
        AssetDatabase.SaveAssets();
    }
    
    public static void RemoveListenerFromScriptableObjects(Tutorial[] eventAssets, RefreshMasking targetAsset)
    {
        foreach (var asset in eventAssets)
        {
            if (asset == null) continue;
            
            RemoveListener(asset.PageInitiated);
            RemoveListener(asset.Completed);
            RemoveListener(asset.Quit);
            RemoveListener(asset.GoingBack);

            // Mark the asset as dirty so changes are saved
            EditorUtility.SetDirty(asset);
            Debug.Log($"Removed listener from {asset.name}");
        }

        // Save changes
        AssetDatabase.SaveAssets();
    }

    private static void RemoveListener(UnityEventBase unityEvent)
    {
        for (int i = unityEvent.GetPersistentEventCount() - 1; i >= 0; i--)
        {
            UnityEventTools.RemovePersistentListener(unityEvent, i);
        }

    }
}