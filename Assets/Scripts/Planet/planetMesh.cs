using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class planetMesh : MonoBehaviour
{
    Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
