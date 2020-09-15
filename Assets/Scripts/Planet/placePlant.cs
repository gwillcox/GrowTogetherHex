using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacePlant
{
    // Todo: maybe refactor so that we deal with world, rather than polar coords?
    public static void placeNew(Vector3 polarLocation, Planet planet, PlantSettings plantSettings)
    {
        int closestIndex = PlacePlant.FindClosestBiome(planet._biomes, polarLocation);
        Biome closestBiome = planet._biomes[closestIndex];

        // Instead of instantiating at the vertex, instantiate at the polar position plus altitude. Then, interpolate altitude. 
        List<Biome> closestBiomeList = PlacePlant.FindNearestTwoBiomes(closestBiome, polarLocation);

        Vector3 plantPosWorld = PlacePlant.InterpolatePosition(polarLocation, closestBiome, closestBiomeList[0], closestBiomeList[1]);

        if (PlacePlant.IsPlantable(closestBiome, plantPosWorld))
        {
            closestBiome.AddPlant(plantSettings, plantPosWorld);
        }
        else { Debug.Log("Not Planted"); }
    }

    public static bool IsPlantable(Biome biome, Vector3 worldLocation)
    {
        // Discovers whether the plant has any overlap with other plants. Assumes a radius of 1. 
        float radiusPlant1 = 3f;
        float radiusPlant2 = 3f;

        bool plantable = true;

        for (int i = 0; i < biome._plants.Count; i++)
        {
            Vector3 plant2Pos = biome._plants[i]._plantObject.transform.position;
            if (Vector3.Distance(worldLocation, plant2Pos) < (radiusPlant1 + radiusPlant2))
            {
                plantable = false;
            }
        }

        if (plantable)
        {
            for (int j = 0; j < biome.neighbors.Count; j++)
            {
                for (int i = 0; i < biome._plants.Count; i++)
                {
                    Vector3 plant2Pos = biome._plants[i]._plantObject.transform.position;
                    if (Vector3.Distance(worldLocation, plant2Pos) < (radiusPlant1 + radiusPlant2))
                    {
                        plantable = false;
                    }
                }
            }
        }

        return plantable;
    }


    public static Vector3 InterpolatePosition(Vector3 polarLocation, Biome biome1, Biome biome2, Biome biome3)
    {
        float[] distanceContribution = new float[3];
        Biome[] biomes = { biome1, biome2, biome3 };

        Vector3 polarUnitLocation = new Vector3(1f, polarLocation[1], polarLocation[2]);

        Vector3 plantUnitProjection = SphericalGeometry.PolarToWorld(polarUnitLocation);

        for (int i = 0; i < biomes.Length; i++)
        {
            Vector3 currentPolar = new Vector3(1.0f, biomes[i]._polarcoordinates[1], biomes[i]._polarcoordinates[2]);
            Vector3 currentUnitProjection = SphericalGeometry.PolarToWorld(currentPolar);
            distanceContribution[i] = 1f / (Vector3.Distance(currentUnitProjection, plantUnitProjection) + 0.00000001f);
        }

        float[] weights = new float[3];
        Vector3 locationWorld = new Vector3();
        for (int i = 0; i < biomes.Length; i++)
        {
            weights[i] = distanceContribution[i] / distanceContribution.Sum();
            locationWorld += biomes[i]._worldcoordinates * weights[i];
        }

        return locationWorld;
    }

    public static List<Biome> FindNearestTwoBiomes(Biome closestBiome, Vector3 polarLocation)
    {

        // Instead of instantiating at the vertex, instantiate at the polar position plus altitude. Then, interpolate altitude. 
        List<Biome> neighbors = new List<Biome>();
        for (int i = 0; i < closestBiome.neighbors.Count; i++) { neighbors.Add(closestBiome.neighbors[i]); }

        int secondClosestIndex = PlacePlant.FindClosestBiome(neighbors, polarLocation);
        Biome secondClosestBiome = closestBiome.neighbors[secondClosestIndex];
        neighbors.Remove(secondClosestBiome);

        int thirdClosestIndex = PlacePlant.FindClosestBiome(neighbors, polarLocation);
        Biome thirdClosestBiome = closestBiome.neighbors[thirdClosestIndex];

        return new List<Biome> { secondClosestBiome, thirdClosestBiome };
    }

    public static int FindClosestBiome(List<Biome> biomes, Vector3 polarLocation)
    {
        // find the nearest vertex, and the second two closest neighbors.
        int closestIndex = -1;
        float closestDist = Mathf.Infinity;
        polarLocation[0] = 1f;

        Vector3 plantUnitProjection = SphericalGeometry.PolarToWorld(polarLocation);

        for (int i = 0; i < biomes.Count; i++)
        {
            Vector3 currentPolar = new Vector3(1.0f, biomes[i]._polarcoordinates[1], biomes[i]._polarcoordinates[2]);

            Vector3 currentUnitProjection = SphericalGeometry.PolarToWorld(currentPolar);

            float distance = Vector3.Distance(currentUnitProjection, plantUnitProjection);

            if (distance < closestDist)
            {
                closestIndex = i;
                closestDist = distance;
            }
        }

        if (closestIndex == -1)
        {
            Debug.LogError("NO BIOME SELECTED");
        }

        return closestIndex;
    }

}
