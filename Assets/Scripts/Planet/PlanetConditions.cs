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
    public int CalcBiome(float temperature, float rainfall)
    {
        int BiomeID;

        if (temperature < -5) { BiomeID = 0; }
        else if (temperature < 5 && rainfall > 50) { BiomeID = 1; }
        else if (rainfall < 30) { BiomeID = 2; }
        else if (temperature < 20 && rainfall < 75) { BiomeID = 3; }
        else if (temperature < 20 && rainfall < 200) { BiomeID = 4; }
        else if (temperature < 20) { BiomeID = 5; }
        else if (rainfall < 100) { BiomeID = 6; }
        else if (rainfall < 250) { BiomeID = 7; }
        else { BiomeID = 8; }

        return BiomeID;
    }

    string LookupBiome(int biomeID)
    {
        string biome; 

        if (biomeID == 0) { biome = "Tundra"; }
        else if (biomeID == 1) { biome = "Taiga"; }
        else if (biomeID == 2) { biome = "Subtropical Desert"; }
        else if (biomeID == 3) { biome = "Temperate Desert"; }
        else if (biomeID == 4) { biome = "Temperate Deciduous Forest"; }
        else if (biomeID == 5) { biome = "Temperate Rain Forest"; }
        else if (biomeID == 6) { biome = "Savanna"; }
        else if (biomeID == 7) { biome = "Tropical Seasonal Forest"; }
        else if (biomeID == 8) { biome = "Tropical Rain Forest"; }
        else { biome = "NONE"; Debug.LogError("NO BIOME FOUND"); }

        return biome;
    }

    public BiomeConditions CalcBiomeConditions(Biome biome)
    {
        BiomeConditions biomeConditions = new BiomeConditions();

        biomeConditions.Altitude = biome._polarcoordinates[0];
        biomeConditions.Rainfall = rainNoiseSettings.calcRain(biome._worldcoordinates, planetRadius);
        biomeConditions.Temperature = tempSettings.calcTemp(biome._worldcoordinates, planetRadius);
        biomeConditions.BiomeID = CalcBiome(biomeConditions.Temperature, biomeConditions.Rainfall);
        biomeConditions.Biome = LookupBiome(biomeConditions.BiomeID);

        return biomeConditions;
    }
}

