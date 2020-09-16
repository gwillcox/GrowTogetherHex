using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimeController))]
public class TimeControllerEditor : Editor
{
    TimeController timeController;

    public override void OnInspectorGUI()
    {
        timeController = (TimeController)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("Init"))
        {
            timeController.InitWorld();
        }
        if (GUILayout.Button("Reinitialize"))
        {
            timeController.ResetWorld();
            timeController.InitWorld();
        }
        if (GUILayout.Button("Populate"))
        {
            timeController.PopulateWorld();
        }
        if (GUILayout.Button("Tick"))
        {
            timeController.Tick();
        }
    }

    void DrawSettingsEditor(Object settings, ref bool foldout)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            if (foldout)
            {
                Editor editor = CreateEditor(settings);
                editor.OnInspectorGUI();
            }
        }
    }
}