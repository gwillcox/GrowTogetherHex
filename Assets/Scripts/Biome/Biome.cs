using System.Collections.Generic;
using UnityEngine;

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
    public List<Plant> _plantsToKill = new List<Plant>();
    public List<Plant> _plantsToAdd = new List<Plant>();

    public float squareDistanceToPlayer = 0f;

    void Start() { }

    void Update() 
    {
        Tick();
    }

    public void CalcDistanceToPlayer(Vector3 playerPosition)
    {
        squareDistanceToPlayer = (transform.position - playerPosition).sqrMagnitude;
    }
    
    public void Init(Planet planet, Vector3 worldCoordinates)
    {
        _planet = planet;
        _worldcoordinates = worldCoordinates;
        _polarcoordinates = SphericalGeometry.WorldToPolar(_worldcoordinates);
        _conditions = new BiomeConditions();
        transform.position = worldCoordinates;
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

        _plantsToAdd.Add(plant);
    }

    public void KillPlant(Plant plant)
    {
        _plantsToKill.Add(plant);
    }

    public void KillAllPlants()
    {
        foreach (var plant in _plants)
        {
            plant.KillPlant();
        }
        DestroyDeadPlants();
    }

    public void DestroyDeadPlants()
    {
        foreach (var deadPlant in _plantsToKill)
        {
            _plants.Remove(deadPlant);
            Destroy(deadPlant);
        }
        // Reset the plantsToKill array
        _plantsToKill = new List<Plant>();
    }

    public void AddNewPlants()
    {
        foreach (var newPlant in _plantsToAdd)
        {
            _plants.Add(newPlant);
        }
        // Reset the plantsToAdd array
        _plantsToAdd = new List<Plant>();
    }

    public void Tick()
    {
        if (_plantsToAdd.Count > 0)
        { // If there are any new plants, add them to our plants list. 
            AddNewPlants();
        }

        if (_plants.Count > 0 && Random.Range(0f,1f) < 1000f/squareDistanceToPlayer)
        {
            foreach (var plant in _plants)
            {
                plant.ProcessBiomeConditions(_conditions);
            }
        }

        if (_plantsToKill.Count > 0)
        {
            DestroyDeadPlants();
        }
    }

    public void SetConditions(BiomeConditions biomeConditions)
    {
        _conditions = biomeConditions;
    }
}