using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SocialPlatforms.GameCenter;

[ExecuteInEditMode]
public class CreatePlanet : MonoBehaviour
{
    [Range(1, 20)]
    public int numPoints;

    public bool projectMesh;

    public GameObject planet;
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;

    private Vector3[] positionPolar;
    private Vector3[] positionWorld;
    private Vector3[] positionBackToPolar;

    private List<Vector3> interpPoints = new List<Vector3>();
    private List<int> interpTris = new List<int>();

    [Range(0.001f, 2f)]
    public float noiseSize;

    [Range(0.1f, 2f)]
    public float noiseFrequency;

    [Range(-10f, 10f)]
    public float noiseOffsetX;
    [Range(-10f, 10f)]
    public float noiseOffsetY;
    [Range(-10f, 10f)]
    public float noiseOffsetZ;

    [Range(1, 20)]
    public int noiseOctaves;

    [Range(0.1f, 1)]
    public float noisePersistence = 0.5f;

    private void Update()
    {
        CreateIcosahedron();
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = planet.GetComponent<MeshFilter>().mesh;

        // Create an icosahedron
        CreateIcosahedron();
    }

    public void CreateIcosahedron()
    {
        
        Vector3[] positionPolar = new[] {
            new Vector3(1f, 0f, 0f*Mathf.PI/180),
            new Vector3(1f, 0f, 90*Mathf.PI/180),
            new Vector3(1f, 90f*Mathf.PI/180, 90f*Mathf.PI/180),
            new Vector3(1f, 180f*Mathf.PI/180, 90f*Mathf.PI/180),
            new Vector3(1f, 270f*Mathf.PI/180, 90f*Mathf.PI/180),
            new Vector3(1f, 0f*Mathf.PI/180, 180f*Mathf.PI/180)};

        vertices = ConvertPolarToWorld(positionPolar);

        InterpolateWorldPoints();

    }

    private void InterpolateWorldPoints()
    {
        interpPoints.Clear();
        interpTris.Clear();

        // First, get all vertex positons
        InterpolateTriangle(positionWorld[0], positionWorld[1], positionWorld[2]);
        InterpolateTriangle(positionWorld[0], positionWorld[2], positionWorld[3]);
        InterpolateTriangle(positionWorld[0], positionWorld[3], positionWorld[4]);
        InterpolateTriangle(positionWorld[0], positionWorld[4], positionWorld[1]);
        InterpolateTriangle(positionWorld[5], positionWorld[2], positionWorld[1]);
        InterpolateTriangle(positionWorld[5], positionWorld[3], positionWorld[2]);
        InterpolateTriangle(positionWorld[5], positionWorld[4], positionWorld[3]);
        InterpolateTriangle(positionWorld[5], positionWorld[1], positionWorld[4]);

        if (projectMesh)
        {
            ProjectMesh();
        }

        ApplyNoise();

        UpdateMesh();
    }

    public void ApplyNoise()
    {
        // Apply noise to the radius of the planet using spherical Perlin Noise

        for (int i = 0; i<interpPoints.Count; i++)
        {
            Vector3 sphericalComponents = worldToPolar(interpPoints[i]);
            float radiusChange =
                calcNoise(interpPoints[i].x + noiseOffsetX, interpPoints[i].z + noiseOffsetZ) +
                calcNoise(interpPoints[i].y + noiseOffsetY, interpPoints[i].z + noiseOffsetZ) +
                calcNoise(interpPoints[i].x + noiseOffsetX, interpPoints[i].y + noiseOffsetY);
            
            Vector3 newSphericalPosition = sphericalComponents + new Vector3(radiusChange, 0, 0);
/*
            Debug.Log($"CHANGED RADIUS BY: {radiusChange}, {newSphericalPosition}");*/
            interpPoints[i] = PolarToWorld(newSphericalPosition);
        }
    }

    public float calcNoise(float x, float y)
    {
        float noise = 0;

        for (int i = 0; i<noiseOctaves; i++)
        {
            float amplitude = noiseSize * Mathf.Pow(noisePersistence, i);
            float frequency = noiseFrequency*Mathf.Pow(2, i);
            noise += amplitude * Mathf.PerlinNoise(x * frequency, y * frequency);
        }
        return noise;

    }

    private void ProjectMesh()
    {
        // Projects each point onto a unit sphere by calculating that point's theta and phi. 
        for (int i=0; i<interpPoints.Count; i++)
        {
            Vector3 pointInPolar = worldToPolar(interpPoints[i]);
            pointInPolar[0] = 1;  // set the radius to 1. 
            interpPoints[i] = PolarToWorld(pointInPolar);
        }

    }

    private void UpdateMesh()
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


        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
    }

    int AddVertex(Vector3 newVertex)
    {
        int index;
        if (!interpPoints.Contains(newVertex))
        {
            interpPoints.Add(newVertex);
            index = interpPoints.Count - 1;
        }
        else
        {
            index = interpPoints.FindIndex(x => x == newVertex);
        }
        return index;
    }

    void InterpolateTriangle(Vector3 X1, Vector3 X2, Vector3 X3)
    {
        for (int k = 0; k <= numPoints; k++)
        {
            float frac1 = (float)(k) / (numPoints);

            for (int l = 0; l <= numPoints-k; l++)
            {
                int index = 0;
                float frac2 = (float)(l) / (numPoints);

                Vector3 newVertex = X1 * (1 - frac1 - frac2) + X2 * frac1 + X3 * frac2;

                // TODO: add a function that defines the desired triangles, and another that maps these to the existing vertexes. 
                /*AddVertex(newVertex);*/

                interpPoints.Add(newVertex);
                index = interpPoints.Count - 1;

                // Set upwards-facing triangles
                if (k != numPoints && l!=numPoints-k)
                {
                    interpTris.Add(index + 1);
                    interpTris.Add(index);
                    interpTris.Add(index+numPoints-k+1);
                }

                // Sets downwards-facing traingles
                if (k < numPoints - 1 && l < numPoints-k && l > 0)
                {
                    interpTris.Add(index);
                    interpTris.Add(index + numPoints - k);
                    interpTris.Add(index + numPoints - k + 1);
                }

            }

        }

    }

    public int getVertexIndex(Vector3 vertex)
    {
        return interpPoints.FindIndex(x => x == vertex);
    }

    Vector3[] ConvertPolarToWorld(Vector3[]positionPolar)
    {
        positionWorld = new Vector3[positionPolar.Length];
        positionBackToPolar = new Vector3[positionPolar.Length];

        for (int i=0; i<positionPolar.Length; i++)
        {
            positionWorld[i] = (PolarToWorld(positionPolar[i]));
            positionBackToPolar[i] = (worldToPolar(positionWorld[i]));
        }

        return positionWorld;
    }

    Vector3 PolarToWorld(Vector3 positionPolar)
    {
        float radius = positionPolar.x;
        float phi = positionPolar.y;
        float theta = positionPolar.z;

        return new Vector3(
            radius * Mathf.Sin(theta) * Mathf.Cos(phi),
            positionPolar.x * Mathf.Sin(theta) * Mathf.Sin(phi),
            positionPolar.x * Mathf.Cos(theta)
            );
    }

    Vector3 worldToPolar(Vector3 worldCoordinates)
    {

        float radius = worldCoordinates.magnitude;
        float phi = Mathf.Atan2(worldCoordinates.y, worldCoordinates.x);
        float theta = Mathf.Acos(worldCoordinates.z/radius);

        return new Vector3(radius, phi, theta);
    }


    private void OnDrawGizmosSelected()
    {

        for (int i = 0; i < interpPoints.Count; i++)
        {
            Gizmos.DrawSphere(interpPoints[i], 0.01f);
        }
        Debug.Log("made a gizmo");

    }
}
