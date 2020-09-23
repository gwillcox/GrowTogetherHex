using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TempSettings : NoiseSettings {

    public float altitudeContribution;
    public float latitudeContribution;
    public float noiseContribution;

    public float calcTemp(Vector3 vertex, float planetRadius)
    {

        var latitude = SphericalGeometry.WorldToPolar(vertex)[2];
        float temp = 30 +
            -latitudeContribution * (float)Mathf.Abs(Mathf.Cos(latitude)) +
            altitudeContribution * (1-vertex.magnitude / planetRadius) +
            -noiseContribution * this.calcNoise(vertex / planetRadius);
        
        return temp;
    }

}
