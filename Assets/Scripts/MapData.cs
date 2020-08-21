using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MapDataScriptableObject", order = 1)]
public class MapData : ScriptableObject
{
    public string mapName;
    public int numTiles = 10;
    public List<Vector3Int> tileList;
    public List<bool> hasPlant;
    public List<GameObject> plantObject;
}
