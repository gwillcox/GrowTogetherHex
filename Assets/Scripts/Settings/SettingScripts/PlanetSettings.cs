using UnityEngine;

[CreateAssetMenu()]
public class PlanetSettings : ScriptableObject {

    [HideInInspector]
    public bool foldout = true; 

    public int numTerrainPoints;
    public int numBiomePoints;
    public float planetRadius;

    public TempSettings tempSettings;
    public TerrainSettings terrainSettings;
    public RainSettings rainSettings;

}
