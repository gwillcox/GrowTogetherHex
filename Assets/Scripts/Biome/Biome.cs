using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.UI;
using WeightedBiomes = System.Collections.Generic.Dictionary<Biome, float>;

/*[Serializable]*/
public class Biome : MonoBehaviour
{
    [SerializeField] Plant _plantPrefab;
    public Vector3 _worldcoordinates;
    public Vector3 _polarcoordinates;
    public List<Biome> neighbors = new List<Biome>();
    private Planet _planet;

    public BiomeConditions _conditions { get; private set; }
    public List<Plant> _plants = new List<Plant>();

    void Start() { }

    void Update() 
    {
        Tick();
    }
    
    public void Init(Planet planet, Vector3 worldCoordinates)
    {
        _planet = planet;
        _worldcoordinates = worldCoordinates;
        _polarcoordinates = SphericalGeometry.WorldToPolar(_worldcoordinates);
        _conditions = new BiomeConditions();
    }

    public void AddNeighbor(Biome neighborBiome)
    {
        if (!neighbors.Contains(neighborBiome))
        {
            neighbors.Add(neighborBiome);
        }
    }

    public void AddPlant(PlantSettings plantSettings, Vector3 position)
    {
        GameObject plantObject = Instantiate(plantSettings.plantObject, 
            position,
            Quaternion.LookRotation(position),
            transform);

        Plant plant = plantObject.GetComponent<Plant>();
        plant.Init(_planet, this, position, plantSettings);

        _plants.Add(plant);
        Debug.Log($"Added a plant: {_plants.Count}");
    }

    public void KillPlant(Plant plant)
    {
        _plants.Remove(plant); 
        Debug.Log("Removed a plant");
    }

    public void KillAllPlants()
    {
        Plant[] _plantCopy = new Plant[_plants.Count];
        _plants.CopyTo(_plantCopy);

        foreach (var plant in _plantCopy)
        {
            plant.KillPlant();
        }
    }

    public void Tick()
    {
/*        CalculateShades();*/

        Plant[] _plantsCopy = new Plant[_plants.Count];
        _plants.CopyTo(_plantsCopy);
        foreach (var plant in _plantsCopy)
        {
            plant.ProcessBiomeConditions(_conditions);
        }
    }

    public void CalculateShades()
    {
        // Calculate the shade that each plant throws on each other plant. 
        var plantList = new List<Plant>();
        foreach (var plant in _plants)
        {
            plantList.Add(plant);
        }
        foreach (var biome in neighbors)
        {
            foreach (var plant in biome._plants)
            {
                plantList.Add(plant);
            }
        }

        foreach (var plant in plantList)
        {
            float totalShade = 0f;
            foreach (var plant2 in plantList)
            {
                if (plant2 != plant)
                {
                    totalShade += plant2.CalcSentShade(plant.GetShadeCircleCenter());
                }
            }

            plant.shade = totalShade;
        }
    }

    public void SetConditions(BiomeConditions biomeConditions)
    {
        _conditions = biomeConditions;
    }

    private void ProcessConditions(WeightedBiomes neighborConditions)
    {


        // First calculate the biome's internal conditions based on its plants
        var currentConditions = _conditions;
        var postPlantConditions = new List<BiomeConditions>();
        foreach (var plant in _plants)
        {
            postPlantConditions.Add(plant.ProcessBiomeConditions(currentConditions));
        }

        _conditions = AverageConditions(postPlantConditions);
        WeightedBiomes allConditions = neighborConditions
            .Concat(new WeightedBiomes {{this, 1f}})
            .ToDictionary(x => x.Key, x => x.Value);

        // Next take into account the neighboring conditions
        _conditions = ProcessWeightedBiomes(allConditions);
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