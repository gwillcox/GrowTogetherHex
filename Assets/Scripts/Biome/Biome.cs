using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeightedBiomes = System.Collections.Generic.Dictionary<Biome, float>;

/*[Serializable]*/
public class Biome
{
    public Vector3 _worldcoordinates;
    public Vector3 _polarcoordinates;
    public List<Biome> neighbors = new List<Biome>(); 

    public BiomeConditions Conditions { get; private set; }
    private List<Plant> _plants;
    public List<GameObject> _plantGameObjects = new List<GameObject>();

    public Biome(Vector3 worldCoordinates)
    {
        _worldcoordinates = worldCoordinates;
        _polarcoordinates = SphericalGeometry.WorldToPolar(_worldcoordinates);
        Conditions = new BiomeConditions();
    }

    public void AddNeighbor(Biome neighborBiome)
    {
        if (!neighbors.Contains(neighborBiome))
        {
            neighbors.Add(neighborBiome);
        }
    }

    public void AddPlant(GameObject plant)
    {
        _plantGameObjects.Add(plant);
    }

    public void Tick()
    {
        var neighbors = GetWeightedNeighbors();
        ProcessConditions(neighbors);
    }

    public void SetConditions(BiomeConditions biomeConditions)
    {
        Conditions = biomeConditions;
    }

    private void ProcessConditions(WeightedBiomes neighborConditions)
    {
        // First calculate the biome's internal conditions based on its plants
        var currentConditions = Conditions;
        var postPlantConditions = new List<BiomeConditions>();
        foreach (var plant in _plants)
        {
            postPlantConditions.Add(plant.ProcessBiomeConditions(currentConditions));
        }

        Conditions = AverageConditions(postPlantConditions);
        WeightedBiomes allConditions = neighborConditions
            .Concat(new WeightedBiomes {{this, 1f}})
            .ToDictionary(x => x.Key, x => x.Value);

        // Next take into account the neighboring conditions
        Conditions = ProcessWeightedBiomes(allConditions);
    }

    private BiomeConditions AverageConditions(List<BiomeConditions> postPlantConditions)
    {
        return null;
    }

    private BiomeConditions ProcessWeightedBiomes(WeightedBiomes neighborConditions)
    {
        return null;
    }

    private WeightedBiomes GetWeightedNeighbors()
    {
        return new WeightedBiomes();
    }
}

internal class Plant
{
    public BiomeConditions ProcessBiomeConditions(BiomeConditions currentConditions)
    {
        return null;
    }
}