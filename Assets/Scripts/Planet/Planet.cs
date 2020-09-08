using System;
using System.Collections.Generic;
using UnityEngine;

// A planet holds a mesh, a set of temperatures, elevations, and rainfalls.
[Serializable]
public class Planet
{
    public PlanetConditions planetConditions;
    public List<Biome> biomes = new List<Biome>();

    public void CreateBiomes(Vector3[] vertices, int[] triangles)
    {
        biomes = new List<Biome>(); 
        
        for (int i = 0; i<vertices.Length; i++)
        {
            biomes.Add(new Biome(vertices[i]));
        }

        for (int i=0; i<triangles.Length; i+=3)
        {
            Biome biome1 = biomes[triangles[i]];
            Biome biome2 = biomes[triangles[i + 1]];
            Biome biome3 = biomes[triangles[i + 2]];

            biome1.AddNeighbor(biome2);
            biome1.AddNeighbor(biome3);
            biome2.AddNeighbor(biome1); 
            biome2.AddNeighbor(biome3);
            biome3.AddNeighbor(biome1);
            biome3.AddNeighbor(biome2);
        }
    }

    public void SetBiomeConditions()
    {
        for (int i=0; i<biomes.Count; i++)
        {
            BiomeConditions biomeConditions = new BiomeConditions();
            biomeConditions.Altitude = planetConditions.Altitude[i];
            biomeConditions.Rainfall = planetConditions.Rainfall[i];
            biomeConditions.Temperature = planetConditions.Temperature[i];
            biomeConditions.Biome = planetConditions.Biome[i];
            biomeConditions.BiomeID = planetConditions.BiomeID[i];

            biomes[i].SetConditions(biomeConditions);
        }

    }

    public void ResetBiomes()
    {

    }
}
