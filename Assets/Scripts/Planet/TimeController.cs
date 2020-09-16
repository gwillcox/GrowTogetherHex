using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public Planet _planet { get; private set; }
    public Planet planetPrefab;

    private Mesh _planetMesh;

    public PlantSettings _plantSettings;

    void Start()
    {
        InitWorld();
    }

    public void InitWorld()
    {
        _planet = Instantiate(planetPrefab, transform);
        _planetMesh = _planet.GetComponent<MeshFilter>().sharedMesh;
        _planet.CreateBiomes(_planetMesh.vertices, _planetMesh.triangles);

        for (int i = 0; i < 500; i++)
        {
            Vector3 randomPolar = new Vector3(1f, Random.Range(-3.14f, 3.14f), Random.Range(-20f, 3.14f));
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
        {/*
            _planet.planetConditions.tempSettings.offset += Random.insideUnitSphere / 10f;*/
            biome.Tick();
        }
    }
}
