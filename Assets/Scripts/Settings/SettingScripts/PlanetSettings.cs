using UnityEngine;

[CreateAssetMenu()]
public class PlanetSettings : ScriptableObject {

    [HideInInspector]
    public bool foldout = true; 

    public int numTerrainPoints;
    public int numBiomePoints;
    public float planetRadius;
    public float oceanLevel;

    // DO WE NEED THIS--i.e. do we need the numBIOMEPOINTS? If not, then deprecate this structure.

    public TempSettings tempSettings;
    public TerrainSettings terrainSettings;
    public RainSettings rainSettings;

    public BiomeConditions CalcBiomeConditions(Biome biome)
    {
        // TODO: move into BiomeConditions
        BiomeConditions biomeConditions = new BiomeConditions();

        biomeConditions.Altitude = biome._polarcoordinates[0];
        biomeConditions.Rainfall = this.rainSettings.calcRain(biome._worldcoordinates, planetRadius);
        biomeConditions.Temperature = tempSettings.calcTemp(biome._worldcoordinates, planetRadius);
        biomeConditions.BiomeID = biomeConditions.CalcBiome(biomeConditions.Temperature, biomeConditions.Rainfall);
        biomeConditions.Biome = biomeConditions.LookupBiome(biomeConditions.BiomeID);

        return biomeConditions;
    }
}

