using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    private Planet parentPlanet;
    // Start is called before the first frame update
    void Start()
    {
        parentPlanet = transform.GetComponentInParent<Planet>();
        transform.localScale = Vector3.one * parentPlanet.planetSettings.oceanLevel * 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
