using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlacePlant))]
public class PlacePlantEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlacePlant placePlant = (PlacePlant)target;
/*        if (GUILayout.Button("Place Plant"))
        {
            placePlant.Place();
        }*/
    }
}
