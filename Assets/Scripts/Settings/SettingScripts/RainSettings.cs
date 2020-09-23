using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RainSettings : NoiseSettings {

    public float rainBias;
    public float rainScale;

    public float calcRain(Vector3 vertex, float planetRadius)
    {
        float rain = rainScale * (this.calcNoise(vertex / planetRadius)) + rainBias;

        return rain;
    }

}
