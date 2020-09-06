using System;
using System.Linq;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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
        }

        if (colorIt && vertices.Length > 0) {
            UpdateWeather();
            UpdateShading(); 
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
            temperature[i] = 385 +
                -temp_contributions[0] * (float)Math.Abs(Math.Cos(latitude)) -
                temp_contributions[1] * (Mathf.Max(vertices[i].magnitude, planetRadius)) +
                temp_contributions[2] * tempNoiseSettings.calcNoise(vertices[i] / planetRadius);
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
}
