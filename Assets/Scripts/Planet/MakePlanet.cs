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
        Debug.Log($"Made biomes: {planet.biomes.Count}");
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

    public void CreatePlantAtPoint()
    {
        CreatePlant(newPlantPolarLocation);
    }

    public void CreatePlant(Vector2 polarLocation)
    {
        int closestIndex = FindClosestBiome(planet.biomes, polarLocation);

        Biome closestBiome = planet.biomes[closestIndex];

        // Instead of instantiating at the vertex, instantiate at the polar position plus altitude. Then, interpolate altitude. 
        List<Biome> neighbors = new List<Biome>();
        for (int i = 0; i < closestBiome.neighbors.Count; i++) { neighbors.Add(closestBiome.neighbors[i]); }

        int secondClosestIndex = FindClosestBiome(neighbors, polarLocation);
        Biome secondClosestBiome = closestBiome.neighbors[secondClosestIndex];
        neighbors.Remove(secondClosestBiome);

        int thirdClosestIndex = FindClosestBiome(neighbors, polarLocation);
        Biome thirdClosestBiome = closestBiome.neighbors[thirdClosestIndex];

        Vector3 plantPosWorld = InterpolatePosition(polarLocation, closestBiome, secondClosestBiome, thirdClosestBiome);

        Quaternion plantQuaternion = Quaternion.LookRotation(plantPosWorld);

        GameObject plant = GameObject.Instantiate(plantObject, planetObject.transform.localToWorldMatrix.MultiplyPoint(plantPosWorld), plantQuaternion, planetObject.transform);
        closestBiome.AddPlant(plant);

    }

    private Vector3 InterpolatePosition(Vector3 polarLocation, Biome biome1, Biome biome2, Biome biome3)
    {
        float[] distanceContribution = new float[3];
        Biome[] biomes = { biome1, biome2, biome3 };

        Vector3 plantUnitProjection = SphericalGeometry.PolarToWorld(new Vector3(1f, polarLocation[0], polarLocation[1]));

        for (int i = 0; i < biomes.Length; i++)
        {
            Vector3 currentPolar = new Vector3(1.0f, biomes[i]._polarcoordinates[1], biomes[i]._polarcoordinates[2]);
            Vector3 currentUnitProjection = SphericalGeometry.PolarToWorld(currentPolar);
            distanceContribution[i] = 1f/(Vector3.Distance(currentUnitProjection, plantUnitProjection)+0.00000001f);
        }

        float[] weights = new float[3];
        Vector3 locationWorld = new Vector3();
        for (int i = 0; i < biomes.Length; i++)
        {
            weights[i] = distanceContribution[i] / distanceContribution.Sum();
            locationWorld += biomes[i]._worldcoordinates * weights[i];
        }

        return locationWorld;
    }

    private int FindClosestBiome(List<Biome> biomes, Vector2 polarLocation)
    {
        // find the nearest vertex, and the second two closest neighbors.
        int closestIndex = -1;
        float closestDist = Mathf.Infinity;

        Vector3 plantUnitProjection = SphericalGeometry.PolarToWorld(new Vector3(1f, polarLocation[0], polarLocation[1]));

        for (int i = 0; i < biomes.Count; i++)
        {
            Vector3 currentPolar = new Vector3(1.0f, biomes[i]._polarcoordinates[1], biomes[i]._polarcoordinates[2]);

            Vector3 currentUnitProjection = SphericalGeometry.PolarToWorld(currentPolar);

            float distance = Vector3.Distance(currentUnitProjection, plantUnitProjection);

            if (distance < closestDist)
            {
                closestIndex = i;
                closestDist = distance;
            }
        }

        if (closestIndex == -1)
        {
            Debug.LogError("NO BIOME SELECTED");
        }

        return closestIndex;
    }

    private void OnDrawGizmos()
    {
        for (int i=0; i<planet.biomes.Count; i++)
        {
            if (planet.biomes[i]._plantGameObjects.Count > 0)
            {
                Gizmos.DrawWireSphere(planet.biomes[i]._worldcoordinates, 1f);
            }
        }
    }

    public void CreateRandomPlants()
    {
        for (int i=0; i<100; i++)
        {
            Vector2 randomPolar = new Vector2(UnityEngine.Random.Range(-3.14f, 3.14f), UnityEngine.Random.Range(-20f, 3.14f));
            CreatePlant(randomPolar);
        }
    }
}
