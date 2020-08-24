using System;
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
        public GameObject mapObj;
        public Mesh mapMesh;
        public bool illuminated;
        public bool hasPlant;
        public GameObject plantObject;
        public int plantHeight = 0;

        public Tile(Vector3Int pos, Tilemap tileM, GameObject mapO, Mesh mapM)
        {
            position = pos;
            illuminated = true;
            hasPlant = false;
            plantHeight = 0;
            tileMapParent = tileM;
            mapObj = mapO;
            mapMesh = mapM;
            Debug.Log($"Made a tile! {position}");
        }

        public void AddPlant(GameObject plant)
        {
            if (!hasPlant)
            {
                Debug.Log($"Made a plant! {position}");
                hasPlant = true;
                plantHeight = 1;
                float meshHeight = mapMesh.vertices[position.y + position.x * (int)Math.Sqrt(mapMesh.vertices.Length)].z;
                plantObject = Instantiate(plant, mapObj.transform.position + new Vector3(position.x, meshHeight - 0.1f, -position.y), Quaternion.Euler(0, 0, 0), mapObj.transform);
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
                    return;
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