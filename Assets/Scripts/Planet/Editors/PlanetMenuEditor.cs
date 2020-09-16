using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MakePlanet))]
public class CreatePlanetEditor : Editor
{
    MakePlanet planet;

    public override void OnInspectorGUI()
    {
        planet = (MakePlanet)target;
        base.OnInspectorGUI();

        DrawSettingsEditor(planet.terrainNoiseSettings, ref planet.terrainNoiseSettings.foldout); ;
        DrawSettingsEditor(planet.terrainNoiseSettings.ridgeNoise, ref planet.terrainNoiseSettings.ridgeNoise.foldout);
        DrawSettingsEditor(planet.rainNoiseSettings, ref planet.rainNoiseSettings.foldout);
        DrawSettingsEditor(planet.tempSettings, ref planet.tempSettings.foldout);

        if (GUILayout.Button("Reinitialize"))
        {
            planet.ResetMesh();
        }
        if (GUILayout.Button("Restart"))
        {
            planet.Restart();
        }
        if (GUILayout.Button("Redraw"))
        {
            planet.UpdateMesh();
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
