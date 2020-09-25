using UnityEngine;

[CreateAssetMenu()]
public class TerrainSettings : NoiseSettings {

    public NoiseSettings ridgeNoise;
    // TODO: change to altitude settings
    public float calcTerrainNoise(Vector3 vertex)
    {
        float multiplicativeNoise = this.calcNoise(vertex);

        float ridgeNoise = Mathf.Pow(Mathf.Sin(this.ridgeNoise.calcNoise(vertex)),7f);

        float noise = multiplicativeNoise + multiplicativeNoise * (ridgeNoise);
        return noise;
    }

}
