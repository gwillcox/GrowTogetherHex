using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public Planet _planet;

    private Mesh _planetMesh;

    public PlantSettings _plantSettings;

    void Start()
    {
        InitWorld();
    }

    public void InitWorld()
    {
        _planetMesh = _planet.planetObject.GetComponent<MeshFilter>().sharedMesh;
        _planet.CreateBiomes(_planetMesh.vertices, _planetMesh.triangles);

        for (int i = 0; i < 5; i++)
        {
            Vector3 randomPolar = new Vector3(1f, Random.Range(-3.14f, 3.14f), Random.Range(-20f, 3.14f));
            Debug.Log($"Trying to place at: {randomPolar}");
            PlacePlant.placeNew(randomPolar, _planet, _plantSettings);
        }
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
            _planet.planetConditions.tempSettings.offset += Random.insideUnitSphere / 10f;
            _planet.UpdateBiomeTemperatures();
            biome.Tick();
        }
    }
}
