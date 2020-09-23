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

        DrawSettingsEditor(planet.planetSettings, ref planet.planetSettings.foldout); ;
        DrawSettingsEditor(planet.planetSettings.terrainSettings, ref planet.planetSettings.terrainSettings.foldout); ;
        DrawSettingsEditor(planet.planetSettings.terrainSettings.ridgeNoise, ref planet.planetSettings.terrainSettings.ridgeNoise.foldout);
        DrawSettingsEditor(planet.planetSettings.rainSettings, ref planet.planetSettings.rainSettings.foldout);
        DrawSettingsEditor(planet.planetSettings.tempSettings, ref planet.planetSettings.tempSettings.foldout);

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
