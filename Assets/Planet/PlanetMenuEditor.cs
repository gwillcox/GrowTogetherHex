using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreatePlanet))]
public class CreatePlanetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CreatePlanet createPlanet = (CreatePlanet)target;
        if (GUILayout.Button("Restart"))
        {
            createPlanet.CreateIcosahedron();
            createPlanet.CreateIcosahedron();
        }
    }
}
