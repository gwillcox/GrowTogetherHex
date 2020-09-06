using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameController gameController = (GameController)target;
        if (GUILayout.Button("Redraw Map"))
        {
            gameController.CreateMap();
        }
        if (GUILayout.Button("Redraw Shade"))
        {
            gameController.CalculateShade();
        }
        if (GUILayout.Button("Step Time"))
        {
            gameController.StepTime();
        }
    }
}
