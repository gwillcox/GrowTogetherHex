using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor.Experimental.GraphView;

using UnityEngine;

// A planet holds a mesh, a set of temperatures, elevations, and rainfalls.
[Serializable]
public class Planet : MonoBehaviour
{
    public PlanetConditions planetConditions;
    public List<Biome> _biomes = new List<Biome>();

    private float _planetRadius;
    private TerrainSettings _terrainNoise;
    private List<Vector3> _interpPoints;
    private List<int> _interpTris;
    private Mesh _mesh;
    private List<Vector3> _polarWorldPositions = new List<Vector3>();

    private void Awake()
    {
        _mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    public void Init(int numPoints,
        float planetRadius,
        TerrainSettings terrainNoise)
    {
        planetConditions = new PlanetConditions();
        (_interpPoints, _interpTris) = CreateSphere.CreateIcosahedron(numPoints);
        _biomes = ProjectMeshAndCreateBiomes();
        SetBiomeNeighbors(_interpTris.ToArray());
        _planetRadius = planetRadius;
        _terrainNoise = terrainNoise;
        _mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    public List<Biome> ProjectMeshAndCreateBiomes()
    {
        var biomes = new List<Biome>();
        // Projects each point onto a unit sphere by calculating that point's theta and phi. 
        foreach (var point in _interpPoints)
        {
            Vector3 pointInPolar = SphericalGeometry.WorldToPolar(point);
            pointInPolar[0] = _planetRadius; // set the radius to the planet's radius. 
            var polarWorldPoint = SphericalGeometry.PolarToWorld(pointInPolar);
            biomes.Add(new Biome(this, polarWorldPoint));
        }

        return biomes;
    }

    public void ApplyNoise()
    {
        foreach (var biome in _biomes)
        {
            Vector3 sphericalComponents = SphericalGeometry.WorldToPolar(biome.Position);
            float radiusChange = _terrainNoise.calcTerrainNoise(biome.Position / _planetRadius);

            Vector3 newSphericalPosition =
                sphericalComponents + new Vector3(radiusChange * _planetRadius + 0.01f, 0, 0);
            biome.Position = SphericalGeometry.PolarToWorld(newSphericalPosition);
            _polarWorldPositions.Add(biome.Position);
        }
    }

    private void SetBiomeNeighbors(int[] triangles)
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Biome biome1 = _biomes[triangles[i]];
            Biome biome2 = _biomes[triangles[i + 1]];
            Biome biome3 = _biomes[triangles[i + 2]];

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
        for (int i = 0; i < _biomes.Count; i++)
        {
            // BiomeConditions biomeConditions = planetConditions.CalcBiomeConditions(_biomes[i]);
            //
            // _biomes[i].SetConditions(biomeConditions);
        }
    }

    public void UpdateBiomeTemperatures()
    {
        for (int i = 0; i < _biomes.Count; i++)
        {
            float temp =
                planetConditions.tempSettings.calcTemp(_biomes[i]._worldcoordinates, planetConditions.planetRadius);

            _biomes[i]._conditions.Temperature = temp;
        }
    }

    public void UpdateMesh()
    {
        _mesh.Clear();
        // print($"Got this stuff: {_biomes.Select(b => b.Position).ToArray().Length}");
        _mesh.vertices = _polarWorldPositions.ToArray();
        _mesh.triangles = _interpTris.ToArray();
        // _mesh.colors = colors;
        _mesh.RecalculateTangents();
        _mesh.RecalculateNormals();
    }
}