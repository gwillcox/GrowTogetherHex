using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public GameObject world;
    public Planet _planet { get; private set; }
    public Planet planetPrefab;

    private Mesh _planetMesh;

    public int n_plants;
    public PlantSettings populatePlant;

    public PlantSettings[] plantOptions;
    public int selectedPlantInt = 0;

    void Start()
    {
        InitWorld();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // TODO: Make a plant appear where eggboy is standing. 
            Debug.Log("Placed a seed");
            Vector3 plantLocation = new Vector3(-0.015f, 1,0);
            Vector3 newLoc = world.transform.InverseTransformDirection(plantLocation);
            PlaceSeedOnWorld(newLoc);
        }
    }

    public void InitWorld()
    {
        _planet = Instantiate(planetPrefab, transform);
        _planetMesh = _planet.GetComponent<MeshFilter>().sharedMesh;
        _planet.CreateBiomes(_planetMesh.vertices, _planetMesh.triangles);
    }

    public void PopulateWorld()
    {
        for (int i = 0; i < n_plants; i++)
        {
            Vector3 randomPolar = new Vector3(1f, Random.Range(-3.14f, 3.14f), Random.Range(-20f, 3.14f));

            int randomPlant = Mathf.FloorToInt(Random.Range(0f, plantOptions.Length));
            Debug.Log(randomPlant);
            PlacePlant.placeNew(randomPolar, _planet, plantOptions[randomPlant]);
        }
    }

    public void PlaceSeedOnWorld(Vector3 worldCoordinates)
    {
        PlacePlant.placeNew(SphericalGeometry.WorldToPolar(worldCoordinates), _planet, plantOptions[selectedPlantInt]);
    }

    public void SetSelectedPlant(int selectionInt)
    {
        selectedPlantInt = selectionInt;
        Debug.Log($"New Selected plant: {selectedPlantInt}, {plantOptions[selectedPlantInt]}");
    }

    public void ResetWorld()
    {
        Biome[] biomeCopy = new Biome[_planet.biomes.Count];
        _planet.biomes.CopyTo(biomeCopy);
        foreach (var biome in biomeCopy)
        {
            biome.KillAllPlants();
        }
    }

    public void Tick()
    {
        foreach (var biome in _planet.biomes)
        {
            biome.Tick();
        }
    }
}
