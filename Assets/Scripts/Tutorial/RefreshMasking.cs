using System.Threading.Tasks;
using Unity.Tutorials.Core.Editor;
using UnityEngine;


[CreateAssetMenu(fileName = "RefreshMasking", menuName = "Tutorials/Create RefreshMasking")]
public class RefreshMasking : ScriptableObject
{
    
    public void StartConstantRefreshing()
    {
        EditorSelection.OnEditorInteracted += UpdateMasking;
    }

    public void StopConstantRefreshing()
    {
        EditorSelection.OnEditorInteracted -= UpdateMasking;
    }

    private void OnDisable()
    {
        EditorSelection.OnEditorInteracted -= UpdateMasking;
    }

    private async void UpdateMasking()
    {
        await Task.Yield();
        if (!TutorialWindow.Instance.CurrentTutorial)
        {
            return;
        }
        
        TutorialWindow.Instance.CurrentTutorial.CurrentPage.RaiseMaskingSettingsChanged();
    }
}
