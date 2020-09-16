using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        Plant plant = Instantiate(_plantPrefab,
            _planet.transform.localToWorldMatrix.MultiplyPoint(position),
            Quaternion.LookRotation(position),
            transform);

        plant.Init(_planet, this, position, plantSettings);
        
        _plants.Add(plant);
    }

    public void KillPlant(Plant plant)
    {
        _plants.Remove(plant); 
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
        Plant[] _plantsCopy = new Plant[_plants.Count];
        _plants.CopyTo(_plantsCopy);
        foreach (var plant in _plantsCopy)
        {
            plant.ProcessBiomeConditions(_conditions);
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