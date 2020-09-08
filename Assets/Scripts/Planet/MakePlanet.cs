using System;
using System.Linq;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;

[ExecuteInEditMode, Serializable]
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

    public Planet planet = new Planet();

    [Range(1, 250)]
    public int numPoints;

    [Range(1, 100)]
    public float planetRadius;

    public GameObject planetObject;
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    private List<Vector3> interpPoints = new List<Vector3>();
    private List<int> interpTris = new List<int>();

    public NoiseSettings terrainNoiseSettings;
    public bool multiplicativeNoise;
    public NoiseSettings terrainM_NoiseSettings;
    public NoiseSettings rainNoiseSettings;
    public NoiseSettings tempNoiseSettings;

    public Vector3 temp_offset;

    public Vector3 temp_contributions;

    public Vector2 newPlantPolarLocation;
    public GameObject plantObject;

    private void Update()
    {
        if (autoUpdate)
        {
            UpdateMesh();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        planetObject.GetComponent<MeshFilter>().sharedMesh = mesh;

        (interpPoints, interpTris) = CreateSphere.CreateIcosahedron(numPoints);

        UpdateMesh();
    }

    public void Restart()
    {
        mesh = new Mesh();
        planetObject.GetComponent<MeshFilter>().sharedMesh = mesh;
        (interpPoints, interpTris) = CreateSphere.CreateIcosahedron(numPoints);
        UpdateMesh();
    }

    public void ApplyNoise()
    {
        // Apply noise to the radius of the planet using spherical Perlin Noise

        for (int i = 0; i < interpPoints.Count; i++)
        {
            Vector3 sphericalComponents = SphericalGeometry.WorldToPolar(interpPoints[i]);
            float radiusChange = terrainNoiseSettings.calcNoise(interpPoints[i] / planetRadius);
            if (multiplicativeNoise)
            {
                radiusChange = radiusChange + radiusChange * terrainM_NoiseSettings.calcNoise(interpPoints[i] / planetRadius);
            }

            Vector3 newSphericalPosition = sphericalComponents + new Vector3(radiusChange*planetRadius+0.01f, 0, 0);
            interpPoints[i] = SphericalGeometry.PolarToWorld(newSphericalPosition);
        }
    }

    private void ProjectMesh()
    {
        // Projects each point onto a unit sphere by calculating that point's theta and phi. 
        for (int i = 0; i < interpPoints.Count; i++)
        {
            Vector3 pointInPolar = SphericalGeometry.WorldToPolar(interpPoints[i]);
            pointInPolar[0] = planetRadius;  // set the radius to the planet's radius. 
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

        if (colorIt && vertices.Length > 0) {
            UpdateWeather();
            UpdateShading();

            SetPlanetConditions();
        }

        SetMeshUpdate();
    }

    void UpdateWeather()
    {
        float[] altitude = new float[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            altitude[i] = vertices[i].magnitude;
        }
        planet.planetConditions.Altitude = altitude;

        float[] temperature = new float[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            var latitude = SphericalGeometry.WorldToPolar(vertices[i])[2];
            bool isOcean = (vertices[i].magnitude > planetRadius);
            temperature[i] = 385 + 
                -temp_contributions[0] * (float)Math.Abs(Math.Cos(latitude)) +
                - temp_contributions[1] * (Mathf.Max(vertices[i].magnitude, planetRadius)) +
                -temp_contributions[2] * tempNoiseSettings.calcNoise(vertices[i] / planetRadius) +
                - (isOcean ? 0f : 1f) * 1000f;
        }
        planet.planetConditions.Temperature = temperature;

        float[] rainfall = new float[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            rainfall[i] = (rainNoiseSettings.calcNoise(vertices[i] / planetRadius) + 1) * 50;
        }
        planet.planetConditions.Rainfall = rainfall;

        planet.planetConditions.updateBiomes();
    }

    void UpdateShading()
    {
        switch (colorBy)
        {
            case ColorBy.None:
                StandardShading();
                break;
            case ColorBy.Altitude:
                CalculateShading(altitudeGradient, planet.planetConditions.Altitude);
                break;
            case ColorBy.Terrain:
                CalculateShading(terrainGradient, planet.planetConditions.Altitude);
                break;
            case ColorBy.Rainfall:
                CalculateShading(rainfallGradient, planet.planetConditions.Rainfall);
                break;
            case ColorBy.Temperature:
                CalculateShading(temperatureGradient, planet.planetConditions.Temperature);
                break;
            case ColorBy.Biome:
                CalculateShading(biomeGradient, planet.planetConditions.BiomeID);
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
        planet.CreateBiomes(vertices, triangles);
        Debug.Log($"Made biomes: {planet.biomes.Length}, Neighbors: {planet.biomes[0].neighbors.Count}");
        Debug.Log($"Check alt: {vertices[201]}, Neighbors: {planet.biomes[201]._worldcoordinates}");

    }


    public void SetPlanetConditions()
    {
        planet.SetBiomeConditions();
        Debug.Log($"Set Conditions: {planet.biomes[0].Conditions}");
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

    public void CreatePlant()
    {
        // find the nearest vertex, and the second two closest neighbors.
        int closestIndex = -1;
        float closestDist = Mathf.Infinity;

        for (int i=0; i<planet.biomes.Length; i++)
        {
            Vector2 currentPolar = new Vector2(planet.biomes[i]._polarcoordinates[1], planet.biomes[i]._polarcoordinates[2]);
            float distance = Vector2.Distance(currentPolar, newPlantPolarLocation);

            if (distance<closestDist)
            {
                closestIndex = i;
                closestDist = distance;
            }
        }

        Debug.Log($"CLOSEST INDEX: {closestIndex}, {closestDist}, {newPlantPolarLocation}, {planet.biomes[closestIndex]._polarcoordinates[1]} {planet.biomes[closestIndex]._polarcoordinates[2]}");

        Biome closestBiome = planet.biomes[closestIndex];

        Vector3 plantPos = closestBiome._worldcoordinates;
        Debug.Log($"Polar: {closestBiome._polarcoordinates}, {closestBiome._worldcoordinates}");

        for (int i=0; i<closestBiome.neighbors.Count; i++)
        {
            Debug.Log($"NEIGHBOR: {i} {closestBiome.neighbors[i]._polarcoordinates}, {closestBiome.neighbors[i]._worldcoordinates}");
        }

        Quaternion plantQuaternion = Quaternion.LookRotation(plantPos);

        GameObject.Instantiate(plantObject, plantPos, plantQuaternion, planetObject.transform);

/*
        // find the closest two of this neighbor's vertexes
        int[] closest = new int[2];
        float[] proximity = new float[closestBiome.neighbors.Count];
        for (int i=0; i<closest.Length; i++)
        {
            proximity[i] = 
        }*/

    }
}
