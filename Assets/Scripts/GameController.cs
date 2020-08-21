using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GameController : MonoBehaviour
{
    public Tilemap tilemap;
    public MapData mapData;
    public List<TileController.Tile> tiles = new List<TileController.Tile>();
    public TileBase illuminatedTile;
    public TileBase shadedTile;
    public GameObject plant;
    public int sunDirectionInt;

    // Start is called before the first frame update
    void Start()
    {
        CreateMap();
    }

    private void ClearMap()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].DeletePlant();
        }
        tiles.Clear();
    }

    /// Load data from the MapData by iterating through the tiles, creating a tile instance at each, loading plants, and shading. 
    public void CreateMap()
    {
        ClearMap();

        for (int i = 0; i < mapData.tileList.Count; i++)
        {
            Vector3Int tilePosition = mapData.tileList[i];
            TileController.Tile newTile = new TileController.Tile(tilePosition, tilemap);
            tiles.Add(newTile);
            if (mapData.hasPlant[i])
            {
                tiles[i].AddPlant(plant);
            }

        }

        CalculateShade();
    }

    public Vector3Int[] CalculateSunDirection(int direction)
    {
        Vector3Int[] SunDirection;

        switch (direction)
        {
            case 0:
                SunDirection = new Vector3Int[] { new Vector3Int(1, 1, 0), new Vector3Int(0, 1, 0) };
                break;
            case 1:
                SunDirection = new Vector3Int[] { new Vector3Int(1, 0, 0), new Vector3Int(1, 0, 0) };
                break;
            case 2:
                SunDirection = new Vector3Int[] { new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 0) };
                break;
            case 3:
                SunDirection = new Vector3Int[] { new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, 0) };
                break;
            case 4:
                SunDirection = new Vector3Int[] { new Vector3Int(-1, 0, 0), new Vector3Int(-1, 0, 0) };
                break;
            default:
                SunDirection = new Vector3Int[] { new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, 0) };
                break;

        }
        return SunDirection;
    }

    // Calculates the shaded region of the graph by defining the direction the sun is facing, and coloring all cells behind with a plant on them darker. 
    public void CalculateShade()
    {
        Vector3Int[] sunDirection = CalculateSunDirection(sunDirectionInt);

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].illuminated = true;
        }    

        for (int i = 0; i < tiles.Count; i++)
        {
            Vector3Int usedDirection = new Vector3Int();
            if (tiles[i].position[1] % 2 == 0) 
            {
                usedDirection = sunDirection[1];
            }
            else { usedDirection = sunDirection[0];  }

            if (tiles[i].hasPlant && tilemap.HasTile(tiles[i].position+usedDirection))
            {
                var shadedTileID = tiles.FindIndex(x => (x.position == tiles[i].position + usedDirection));
                tiles[shadedTileID].illuminated = false;
            }
        }

        DrawShade();
    }

    public void DrawShade()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tilemap.SetTile(tiles[i].position, tiles[i].illuminated ? illuminatedTile : shadedTile);
        }
    }

    public void GrowPlants()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].illuminated)
            {
                tiles[i].GrowPlant();
            }

            if (tiles[i].plantHeight > 1 && UnityEngine.Random.value > 0.0)
            {
                CreateSeedling(tiles[i]);
            }
        }
    }

    public void CreateSeedling(TileController.Tile tile)
    {
        /// Select a random neighbor plant
        int xrandom = (int)Math.Round(UnityEngine.Random.value*2f-1);
        int yrandom = (int)Math.Round(UnityEngine.Random.value*2f-1);
        var seededTilePosition = tile.position + new Vector3Int(xrandom, yrandom, 0);

        if (tilemap.HasTile(seededTilePosition))
        {
            var seededTileID = tiles.FindIndex(x => (x.position == seededTilePosition));

            tiles[seededTileID].AddPlant(plant);
            Debug.Log($"made a plant: {seededTilePosition}, {xrandom}, {yrandom}");
        }
        else
        {
            Debug.Log($"Position not found: {seededTilePosition}");
        }
            
    }

    // steps time by  changing the sun's position and having each plant grow or die. 
    public void StepTime()
    {
        sunDirectionInt = (sunDirectionInt + 1) % 6;
        CalculateShade();
        GrowPlants();

    }

}
