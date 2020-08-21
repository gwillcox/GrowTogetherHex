using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileController : MonoBehaviour
{

    public class Tile
    {
        public Vector3Int position;
        public Tilemap tileMapParent;
        public bool illuminated;
        public bool hasPlant;
        public GameObject plantObject;
        public int plantHeight = 0;

        public Tile(Vector3Int pos, Tilemap tileM)
        {
            position = pos;
            illuminated = true;
            hasPlant = false;
            plantHeight = 0;
            tileMapParent = tileM;
            Debug.Log($"Made a tile! {position}");
        }

        public void AddPlant(GameObject plant)
        {
            if (!hasPlant)
            {
                Debug.Log("Made a plant!");
                hasPlant = true;
                plantHeight = 1;
                plantObject = Instantiate(plant, tileMapParent.CellToWorld(position), Quaternion.Euler(0, 0, 0), tileMapParent.transform);
            }
        }

        public void GrowPlant()
        {
            if (hasPlant)
            {
                if (plantHeight < 3)
                {
                    plantHeight += 1;
                    plantObject.transform.localScale = plantObject.transform.localScale * 1.3f;
                }
                else
                {
                    DeletePlant();
                }
            }
        }


        public void DeletePlant()
        {
            hasPlant = false;
            plantHeight = 0;
            DestroyImmediate(plantObject);
        }

    }

}