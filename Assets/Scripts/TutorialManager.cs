using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class TutorialManager
{
    static TutorialManager()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();
        // Assume Tools tab is at 10, 10 and the size of the button is 100x20 (these values might not be accurate)
        Rect toolsButtonRect = new Rect(10, 10, 100, 20);
        DrawArrow(toolsButtonRect);

        // Draw an arrow using lines or a texture
        // This would be a custom method you create to draw an arrow pointing to the given rect
        DrawArrow(toolsButtonRect);

        Handles.EndGUI();
    }

    private static void DrawArrow(Rect targetRect)
    {
        // Your drawing logic here
        // Define the starting point of the arrow based on the targetRect
        Vector3 arrowStart = new Vector3(targetRect.xMax + 10, targetRect.center.y, 0);

        // Define the end point (tip of the arrow)
        Vector3 arrowEnd = new Vector3(targetRect.xMin - 10, targetRect.center.y, 0);

        // Define the direction from start to end
        Vector3 direction = (arrowEnd - arrowStart).normalized;

        // Draw the main line of the arrow
        Handles.DrawLine(arrowStart, arrowEnd);

        // Draw the arrowhead
        Vector3 right = new Vector3(direction.y, -direction.x, 0) * 10;
        Vector3 left = new Vector3(-direction.y, direction.x, 0) * 10;
        Handles.DrawLine(arrowEnd, arrowEnd + right);
        Handles.DrawLine(arrowEnd, arrowEnd + left);
    }
}