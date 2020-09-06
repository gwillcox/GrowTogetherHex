using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A planet holds a mesh, a set of temperatures, elevations, and rainfalls.
[Serializable]
public class PlanetConditions
{
    public float[] Temperature { get; set; }
    public float[] Altitude { get; set; }
    public float[] Rainfall { get; set; }
    public string[] Biome { get; private set; }
    public float[] BiomeID { get; private set; }

    // Define the biomes based on temp and rainfall data; 
    // temp in Average deg. C, 
    // rainfall in Annaul Precipitation (cm)
    public void updateBiomes()
    {
        BiomeID = new float[Temperature.Length];
        for (int i=0; i<Temperature.Length; i++)
        {
            if (Temperature[i] < -5) { BiomeID[i] = 0; }
            else if (Temperature[i] < 5  && Rainfall[i] > 50) { BiomeID[i] = 1; }
            else if (Rainfall[i] <30) { BiomeID[i] = 2; }
            else if (Temperature[i] < 20 && Rainfall[i] < 75) { BiomeID[i] = 3; }
            else if (Temperature[i] < 20 && Rainfall[i] < 200) { BiomeID[i] = 4; }
            else if (Temperature[i] < 20) { BiomeID[i] = 5; }
            else if (Rainfall[i] < 100) { BiomeID[i] = 6; }
            else if (Rainfall[i] < 250) { BiomeID[i] = 7; }
            else { BiomeID[i] = 8; }
        }

        lookupBiomes();
    }

    void lookupBiomes()
    {
        Biome = new string[BiomeID.Length];
        for (int i = 0; i < BiomeID.Length; i++)
        {
            if (BiomeID[i] == 0) { Biome[i] = "Tundra"; }
            else if (BiomeID[i] == 1) { Biome[i] = "Taiga"; }
            else if (BiomeID[i] == 2) { Biome[i] = "Subtropical Desert"; }
            else if (BiomeID[i] == 3) { Biome[i] = "Temperate Desert"; }
            else if (BiomeID[i] == 4) { Biome[i] = "Temperate Deciduous Forest"; }
            else if (BiomeID[i] == 5) { Biome[i] = "Temperate Rain Forest"; }
            else if (BiomeID[i] == 6) { Biome[i] = "Savanna"; }
            else if (BiomeID[i] == 7) { Biome[i] = "Tropical Seasonal Forest"; }
            else if (BiomeID[i] == 8) { Biome[i] = "Tropical Rain Forest"; }
            else { Debug.LogError("NO BIOME FOUND"); }

        }
    }
}

