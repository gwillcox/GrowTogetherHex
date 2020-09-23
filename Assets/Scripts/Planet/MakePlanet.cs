using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Net.Mime;

[Serializable, ExecuteInEditMode()]
public class MakePlanet : MonoBehaviour
{
    public string planetName;
    public bool autoUpdate;
    public bool colorIt;
    public bool structureIt;
    public bool projectMesh;
    public enum ColorBy { None, Terrain, Altitude, Temperature, Rainfall, Biome};
    public ColorBy colorBy;

    public Gradient terrainGradient;
    public Gradient altitudeGradient;
    public Gradient temperatureGradient;
    public Gradient rainfallGradient;
    public Gradient biomeGradient;

    public GameObject world;
    public Planet planetPrefab;
    private Planet planet;

    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    private List<Vector3> interpPoints = new List<Vector3>();
    private List<int> interpTris = new List<int>();

    public PlanetSettings planetSettings;

    public Vector3 newPlantPolarLocation;   
    public PlantSettings plantSettings;

    private void Update()
    {
        if (autoUpdate) { UpdateMesh(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        planet = Instantiate(planetPrefab, world.transform);
        planet.GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    
    public void ResetMesh()
    {
        GameObject.DestroyImmediate(planet);
        mesh = new Mesh();      
        planet = Instantiate(planetPrefab, world.transform);
        planet.GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    public void Restart()
    {
        (interpPoints, interpTris) = CreateSphere.CreateIcosahedron(planetSettings.numTerrainPoints);
        UpdateMesh();
    }

    public void ApplyNoise()
    {
        // Apply noise to the radius of the planet using spherical Perlin Noise

        for (int i = 0; i < interpPoints.Count; i++)
        {
            Vector3 sphericalComponents = SphericalGeometry.WorldToPolar(interpPoints[i]);
            float radiusChange = planetSettings.terrainSettings.calcTerrainNoise(interpPoints[i] / planetSettings.planetRadius);

            Vector3 newSphericalPosition = sphericalComponents + new Vector3(radiusChange * planetSettings.planetRadius + 0.01f, 0, 0);
            interpPoints[i] = SphericalGeometry.PolarToWorld(newSphericalPosition);
        }
    }

    private void ProjectMesh()
    {
        // Projects each point onto a unit sphere by calculating that point's theta and phi. 
        for (int i = 0; i < interpPoints.Count; i++)
        {
            Vector3 pointInPolar = SphericalGeometry.WorldToPolar(interpPoints[i]);
            pointInPolar[0] = planetSettings.planetRadius;  // set the radius to the planet's radius. 
            interpPoints[i] = SphericalGeometry.PolarToWorld(pointInPolar);
        }

    }

    public void UpdateMesh()
    {
        if (structureIt)
        {
            if (projectMesh)
            {
                ProjectMesh();
            }

            ApplyNoise();

            CalcVertsTris();

            SetPlanetBiomes();
        }

        if (colorIt && vertices.Length > 0)
        {
            SetPlanetConditions();
            UpdateShading();
        }

        SetMeshUpdate();
    }

    void UpdateShading()
    {
        switch (colorBy)
        {
            case ColorBy.None:
                StandardShading();
                break;

            case ColorBy.Altitude:
                float[] altitudeArray = new float[planet.biomes.Count];
                for (int i=0; i<planet.biomes.Count; i++)
                {
                    altitudeArray[i] = planet.biomes[i]._conditions.Altitude;
                }
                CalculateShading(altitudeGradient, altitudeArray);
                break;

            case ColorBy.Terrain:
                float[] terrainArray = new float[planet.biomes.Count];
                for (int i = 0; i < planet.biomes.Count; i++)
                {
                    terrainArray[i] = planet.biomes[i]._conditions.Altitude;
                }
                CalculateShading(terrainGradient, terrainArray);
                break;

            case ColorBy.Rainfall:
                float[] rainfallArray = new float[planet.biomes.Count];
                for (int i = 0; i < planet.biomes.Count; i++)
                {
                    rainfallArray[i] = planet.biomes[i]._conditions.Rainfall;
                }
                CalculateShading(rainfallGradient, rainfallArray);
                break;

            case ColorBy.Temperature:
                float[] temperatureArray = new float[planet.biomes.Count];
                for (int i = 0; i < planet.biomes.Count; i++)
                {
                    temperatureArray[i] = planet.biomes[i]._conditions.Temperature;
                }
                CalculateShading(temperatureGradient, temperatureArray);
                break;

            case ColorBy.Biome:
                float[] biomeArray = new float[planet.biomes.Count];
                for (int i = 0; i < planet.biomes.Count; i++)
                {
                    biomeArray[i] = planet.biomes[i]._conditions.BiomeID;
                }
                CalculateShading(biomeGradient, biomeArray);
                break;
        }
    }

    void StandardShading()
    {
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(0.5f, 0.5f, 0.5f);
        }
    }

    void CalculateShading(Gradient gradient, float[] conditions)
    {
        colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            float colorPercent = Mathf.InverseLerp(conditions.Min(), conditions.Max(), conditions[i]);
            colors[i] = gradient.Evaluate(colorPercent);
        }
    }

    private void CalcVertsTris()
    {
        vertices = new Vector3[interpPoints.Count];
        for (int i = 0; i < interpPoints.Count; i++)
        {
            vertices[i] = interpPoints[i];
        }

        triangles = new int[interpTris.Count];
        for (int i = 0; i < interpTris.Count; i++)
        {
            triangles[i] = interpTris[i];
        }
    }

    public void SetPlanetBiomes()
    {
        planet.CreateBiomes(interpPoints.ToArray(), interpTris.ToArray());
        Debug.Log($"Made biomes: {planet.biomes.Count}");
    }


    public void SetPlanetConditions()
    {  
        // TODO : Define a method to interpolate between the biome-level traits and 
        planet.SetBiomeConditions();
        Debug.Log($"Set Conditions: {planet.biomes[0]._conditions}");
    }

    private void SetMeshUpdate()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
    }

    public void SaveAsAsset()
    {
        Mesh tempMesh = (Mesh)UnityEngine.Object.Instantiate(mesh);
        AssetDatabase.CreateAsset(tempMesh, "Assets/Scripts/Planet/" + planetName + ".asset");
    }

    public void CreatePlantAtPoint()
    {
        CreatePlant(newPlantPolarLocation);
    }

    public void CreatePlant(Vector3 polarLocation)
    {
        PlacePlant.placeNew(polarLocation, planet, plantSettings);
    }

    public void CreateRandomPlants()
    {
        for (int i=0; i<100; i++)
        {
            Vector3 randomPolar = new Vector3(1f, UnityEngine.Random.Range(-3.14f, 3.14f), UnityEngine.Random.Range(-20f, 3.14f));
            CreatePlant(randomPolar);
        }
    }
}
