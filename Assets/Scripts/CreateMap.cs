using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CreateMap : MonoBehaviour
{
    public string name;
    public int worldSize;
    public MapData map;
    public Mesh mapMesh;

    [Range(0.1f, 20.0f)]
    public float heightScale;

    [Range(0.0f, 10.0f)]
    public float noiseFrequency;

    [Range(1, 10)]
    public int octaves;

    [Range(0.0f, 1f)]
    public float persistence;

    [Range(0.0f, 100f)]
    public float xBias;

    [Range(0.0f, 100f)]
    public float yBias;

    // Start is called before the first frame update
    void Start()
    {
        RecreateMap();
    }

    public void RecreateMap()
    {
        ClearMap();

        InitializeMap();

        CreateMesh();
    }

    void InitializeMap()
    {
        // Create Tiles at each position, with no plants, and with a height. 
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                map.tileList.Add(new Vector3Int(x, y, 0));
                map.hasPlant.Add(false);
                map.height.Add(CalcHeight(x, y));
            }
        }

        // Place a plant randomly
        map.hasPlant[UnityEngine.Random.Range(0, map.hasPlant.Count)] = true;
    }

    void CreateMesh()
    {
        mapMesh.Clear();

        int[] tris = CalculateTris();
        Vector3[] verts = CalculateVertexes();

        mapMesh.vertices = verts;
        mapMesh.triangles = tris;
        mapMesh.RecalculateNormals();

    }

    int[] CalculateTris()
    {
        int[] tris = new int[2 * 3 * (worldSize - 1) * (worldSize - 1)];
        int index = 0;
        for (int x = 0; x < worldSize - 1; x++)
        {
            for (int y = 0; y < worldSize - 1; y++)
            {
                int pos = y * worldSize + x;
                tris[index] = pos;
                tris[index + 1] = pos + worldSize;
                tris[index + 2] = pos + 1;

                tris[index + 3] = pos + worldSize;
                tris[index + 4] = pos + worldSize + 1;
                tris[index + 5] = pos + 1;

                index += 6;
            }
        }
        return tris;
    }

    Vector3[] CalculateVertexes()
    {
        Vector3[] verts = new Vector3[worldSize * worldSize];
        // Iterate over each vertex, and create a set of triangles at that point. 
        int index = 0;
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                float height = map.height[map.tileList.FindIndex(v => v == new Vector3Int(x, y, 0))];
                verts[index] = new Vector3(x, y, height);
                index += 1;
            }
        }

        return verts;
    }

    void ClearMap()
    {
        map.tileList.Clear();
        map.hasPlant.Clear();
        map.height.Clear();

        mapMesh.Clear();
    }

    float CalcHeight(int x, int y)
    {

        float height = 0;
        for (int n = 1; n<=octaves; n++)
        {
            float freq = noiseFrequency * (float)Math.Pow(2, n);
            float amp = heightScale * (float)Math.Pow(persistence, n);

            float scaledX = (x + xBias) * freq / worldSize;
            float scaledY = (y + yBias) * freq / worldSize;
            height += amp * Mathf.PerlinNoise(scaledX, scaledY);
        }

        Debug.Log($"{height}, {x}, {y}, {x / worldSize}");
        return height;
    }

    void RecalculateHeights()
    {
        for (int i = 0; i<map.height.Count; i++)
        {
            var tileLocation = map.tileList[i];
            map.height[i] = CalcHeight(tileLocation.x, tileLocation.y);
        }
    }

    private void Update()
    {
        RecalculateHeights();
        mapMesh.vertices = CalculateVertexes();
        mapMesh.RecalculateNormals();
    }

    public void SaveMap()
    {
        Mesh mCopy = new Mesh();
        mCopy.vertices = mapMesh.vertices;
        mCopy.normals = mapMesh.normals;
        mCopy.name = name;
        mCopy.colors = mapMesh.colors;
        mCopy.triangles = mapMesh.triangles;

        mCopy.RecalculateNormals();

        AssetDatabase.CreateAsset(mCopy, "Assets/Prefabs/map" + name);
    }

}
