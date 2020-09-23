using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateMap))]
public class CreateMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CreateMap createMap = (CreateMap)target;
        if (GUILayout.Button("Restart"))
        {
            createMap.RecreateMap();
        }
        if (GUILayout.Button("Save"))
        {
            createMap.SaveMap();
        }
    }
}
