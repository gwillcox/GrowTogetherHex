using System;
using System.Collections.Generic;
using UnityEngine;

// A planet holds a mesh, a set of temperatures, elevations, and rainfalls.
[Serializable, RequireComponent(typeof(MeshFilter))]
public class Planet : MonoBehaviour
{
    public Biome biomePrefab;
    public PlanetSettings planetSettings;
    public List<Biome> biomes = new List<Biome>();
    public GameObject planetObject { get; private set; }
    private Mesh mesh;
    public Light sun;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        planetObject = GetComponent<GameObject>();
        sun = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateBiomes(PlanetSettings planetSettings)
    {
        if (biomes.Count > 0 && biomes != null)
        {
            foreach (var biome in biomes)
            {
                GameObject.DestroyImmediate(biome?.gameObject);
            }
            biomes.Clear();
        }

        biomes = new List<Biome>();
        List<Vector3> biomePoints;
        List<int> biomeTris;

        (biomePoints, biomeTris) = CreateSphere.CreateIcosahedron(planetSettings.numBiomePoints);

        for (int i = 0; i < biomePoints.Count; i++)
        {
            Biome newBiome = Instantiate(biomePrefab, transform);
            newBiome.Init(this, biomePoints[i]);
            biomes.Add(newBiome);
        }

        for (int i = 0; i < biomeTris.Count; i += 3)
        {
            Biome biome1 = biomes[biomeTris[i]];
            Biome biome2 = biomes[biomeTris[i + 1]];
            Biome biome3 = biomes[biomeTris[i + 2]];

            biome1.AddNeighbor(biome2);
            biome1.AddNeighbor(biome3);
            biome2.AddNeighbor(biome1);
            biome2.AddNeighbor(biome3);
            biome3.AddNeighbor(biome1);
            biome3.AddNeighbor(biome2);
        }

        this.SetBiomeConditions();
    }

    public void CreateBiomes(Vector3[] vertices, int[] triangles)
    {
        if (biomes.Count >0 && biomes != null)
        {
            foreach (var biome in biomes)
            {
                GameObject.DestroyImmediate(biome?.gameObject);
            }
            biomes.Clear();
        }

        biomes = new List<Biome>();
        
        for (int i = 0; i<vertices.Length; i++)
        {
            Biome newBiome = Instantiate(biomePrefab, transform);
            newBiome.Init(this, vertices[i]);
            biomes.Add(newBiome); 
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

        this.SetBiomeConditions();
    }

    // Sets the biome conditions from the Planet Conditions savestate. 
    public void SetBiomeConditions()
    {
        for (int i=0; i<biomes.Count; i++)
        {
            BiomeConditions biomeConditions = planetSettings.CalcBiomeConditions(biomes[i]);

            biomes[i].SetConditions(biomeConditions);
        }
    }

    public void UpdateBiomeTemperatures()
    {
        for (int i = 0; i < biomes.Count; i++)
        {
            float temp = planetSettings.tempSettings.calcTemp(biomes[i]._worldcoordinates, planetSettings.planetRadius);

            biomes[i]._conditions.Temperature = temp;
        }
    }
}
