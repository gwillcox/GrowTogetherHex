using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MakePlanet))]
public class CreatePlanetEditor : Editor
{
    MakePlanet planet;

    public override void OnInspectorGUI()
    {
        planet = (MakePlanet)target;
        base.OnInspectorGUI();/*

        DrawDefaultInspector();*/

        DrawSettingsEditor(planet.terrainNoiseSettings, planet.UpdateMesh, ref planet.terrainNoiseSettings.foldout); ;
        DrawSettingsEditor(planet.terrainM_NoiseSettings, planet.UpdateMesh, ref planet.terrainM_NoiseSettings.foldout);
        DrawSettingsEditor(planet.rainNoiseSettings, planet.UpdateMesh, ref planet.rainNoiseSettings.foldout);
        DrawSettingsEditor(planet.tempNoiseSettings, planet.UpdateMesh, ref planet.tempNoiseSettings.foldout);

        if (GUILayout.Button("Redraw"))
        {
            planet.UpdateMesh();
        }
        if (GUILayout.Button("Restart"))
        {
            planet.Restart();
        }
        if (GUILayout.Button("Save"))
        {
            planet.SaveAsAsset();
        }
        if (GUILayout.Button("Make Biomes"))
        {
            planet.SetPlanetBiomes();
            planet.SetPlanetConditions();
        }
        if (GUILayout.Button("Create Plant"))
        {
            planet.CreatePlantAtPoint();
        }
        if (GUILayout.Button("Create Random Plants"))
        {
            planet.CreateRandomPlants();
        }

    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout)
    {
        if (settings != null)
        {
            var check = new EditorGUI.ChangeCheckScope();

            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            if (foldout)
            {
                Editor editor = CreateEditor(settings);
                editor.OnInspectorGUI();
            }

            if (check.changed)
            {
                if (onSettingsUpdated != null)
                {
                    onSettingsUpdated();
                }
            }
        }
    }
}
