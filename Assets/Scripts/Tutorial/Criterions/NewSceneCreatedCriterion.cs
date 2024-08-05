using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Very specific Criterion to check wether a new scene has been created or not
namespace Tutorial.Criterions
{
    public class NewSceneCreatedCriterion : Criterion
    {
        private Scene startScene;
    
        public override void StartTesting()
        {
            startScene = EditorSceneManager.GetActiveScene();
            
            base.StartTesting();
            UpdateCompletion();


            EditorApplication.update += UpdateCompletion;
        }

        protected override bool EvaluateCompletion()
        {
            return startScene != EditorSceneManager.GetActiveScene();
        }

        public override void StopTesting()
        {
            base.StopTesting();
            EditorApplication.update -= UpdateCompletion;
        }

        public override bool AutoComplete()
        {
            return true;
        }
    }
}
