using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A planet holds a mesh, a set of temperatures, elevations, and rainfalls.
[Serializable]
public class PlanetConditions
{

    public TerrainSettings terrainNoiseSettings;
    public RainSettings rainNoiseSettings;
    public TempSettings tempSettings;

    public float planetRadius = 75f;

    // Define the biomes based on temp and rainfall data; 
    // temp in Average deg. C, 
    // rainfall in Annaul Precipitation (cm)

    public BiomeConditions CalcBiomeConditions(Biome biome)
    {
        // TODO: move into BiomeConditions
        BiomeConditions biomeConditions = new BiomeConditions();

        biomeConditions.Altitude = biome._polarcoordinates[0];
        biomeConditions.Rainfall = rainNoiseSettings.calcRain(biome._worldcoordinates, planetRadius);
        biomeConditions.Temperature = tempSettings.calcTemp(biome._worldcoordinates, planetRadius);
        biomeConditions.BiomeID = biomeConditions.CalcBiome(biomeConditions.Temperature, biomeConditions.Rainfall);
        biomeConditions.Biome = biomeConditions.LookupBiome(biomeConditions.BiomeID);

        return biomeConditions;
    }
}

