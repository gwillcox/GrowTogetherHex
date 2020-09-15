using System;
using System.Linq;

using UnityEngine;

using UnityEditor;

using System.Collections.Generic;
using System.Net.Mime;

[ExecuteInEditMode, Serializable]
public class MakePlanet : MonoBehaviour
{
    public string planetName;
    public bool autoUpdate;
    public bool colorIt;
    public bool structureIt;
    public bool projectMesh;

    public enum ColorBy
    {
        None,
        Terrain,
        Altitude,
        Temperature,
        Rainfall,
        Biome
    };

    public ColorBy colorBy;

    public Gradient terrainGradient;
    public Gradient altitudeGradient;
    public Gradient temperatureGradient;
    public Gradient rainfallGradient;
    public Gradient biomeGradient;

    // public Planet planet = new Planet();

    [Range(1, 250)] public int numPoints;

    [Range(1, 100)] public float planetRadius;

    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    // private List<Vector3> interpPoints = new List<Vector3>();
    // private List<int> interpTris = new List<int>();

    public TerrainSettings terrainNoiseSettings;
    public RainSettings rainNoiseSettings;
    public TempSettings tempSettings;

    public Vector3 newPlantPolarLocation;
    public PlantSettings plantSettings;


    private List<Biome> _biomes;
    [SerializeField] private GameObject _planetPrefab;
    private Planet _planet;

    private void Update()
    {
        if (autoUpdate)
        {
            CreatePlanet();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreatePlanet();
    }

    public void Restart()
    {
        CreatePlanet();
    }

    public void CreatePlanet()
    {
        // List<Vector3> interpPoints = new List<Vector3>();
        // List<int> interpTris = new List<int>();

        // (interpPoints, interpTris) = CreateSphere.CreateIcosahedron(numPoints);


        if (structureIt)
        {
            if (projectMesh)
            {
                _planetPrefab = Instantiate(_planetPrefab, Vector3.zero, Quaternion.identity);
                _planet = _planetPrefab.GetComponent<Planet>();
                _planet.Init(numPoints, planetRadius, terrainNoiseSettings);
                _biomes = _planet.ProjectMeshAndCreateBiomes();
            }

            _planet.ApplyNoise();
        }

        if (colorIt && vertices.Length > 0)
        {
            _planet.SetBiomeConditions();
            UpdateShading();
        }

        _planet.UpdateMesh();
    }

    void UpdateShading()
    {
        switch (colorBy)
        {
            case ColorBy.None:
                StandardShading();
                break;

            case ColorBy.Altitude:
                float[] altitudeArray = new float[_planet._biomes.Count];
                for (int i = 0; i < _planet._biomes.Count; i++)
                {
                    altitudeArray[i] = _planet._biomes[i]._conditions.Altitude;
                }

                CalculateShading(altitudeGradient, altitudeArray);
                break;

            case ColorBy.Terrain:
                float[] terrainArray = new float[_planet._biomes.Count];
                for (int i = 0; i < _planet._biomes.Count; i++)
                {
                    terrainArray[i] = _planet._biomes[i]._conditions.Altitude;
                }

                CalculateShading(terrainGradient, terrainArray);
                break;

            case ColorBy.Rainfall:
                float[] rainfallArray = new float[_planet._biomes.Count];
                for (int i = 0; i < _planet._biomes.Count; i++)
                {
                    rainfallArray[i] = _planet._biomes[i]._conditions.Rainfall;
                }

                CalculateShading(rainfallGradient, rainfallArray);
                break;

            case ColorBy.Temperature:
                float[] temperatureArray = new float[_planet._biomes.Count];
                for (int i = 0; i < _planet._biomes.Count; i++)
                {
                    temperatureArray[i] = _planet._biomes[i]._conditions.Temperature;
                }

                CalculateShading(temperatureGradient, temperatureArray);
                break;

            case ColorBy.Biome:
                float[] biomeArray = new float[_planet._biomes.Count];
                for (int i = 0; i < _planet._biomes.Count; i++)
                {
                    biomeArray[i] = _planet._biomes[i]._conditions.BiomeID;
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
        Mesh tempMesh = (Mesh) UnityEngine.Object.Instantiate(mesh);
        AssetDatabase.CreateAsset(tempMesh, "Assets/Scripts/Planet/" + planetName + ".asset");
    }

    public void CreatePlantAtPoint()
    {
        CreatePlant(newPlantPolarLocation);
    }

    public void CreatePlant(Vector3 polarLocation)
    {
        PlacePlant.placeNew(polarLocation, _planet, plantSettings);
    }

    public void CreateRandomPlants()
    {
        for (int i = 0; i < 100; i++)
        {
            Vector3 randomPolar = new Vector3(1f, UnityEngine.Random.Range(-3.14f, 3.14f),
                UnityEngine.Random.Range(-20f, 3.14f));
            CreatePlant(randomPolar);
        }
    }
}