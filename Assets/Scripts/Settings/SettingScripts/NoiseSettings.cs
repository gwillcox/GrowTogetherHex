using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseSettings : ScriptableObject {

    [HideInInspector]
    public bool foldout;

    public float size;

    [Range(0.1f, 2f)]
    public float frequency;

    public Vector3 offset;

    [Range(1, 20)]
    public int octaves;

    [Range(0.1f, 1)]
    public float persistence = 0.5f;
    [Range(0.1f, 1)]
    public float lacunarity = 0.5f;

    [Range(-1, 1)]
    public float minValue;

    public float calcNoise(Vector3 vector)
    {
        float noise = 0;
        float amplitude_sum = 0;
        Noise noiseFilter = new Noise();
        Vector3 shiftedVector = vector + offset;

        for (int i = 0; i < octaves; i++)
        {
            float octave_amplitude = Mathf.Pow(persistence, i);
            float octave_frequency = frequency * Mathf.Pow(1f / lacunarity, i);
            noise += octave_amplitude * noiseFilter.Evaluate(shiftedVector * octave_frequency);
            amplitude_sum += octave_amplitude;
        }
        noise = (noise / amplitude_sum) * size;

        noise = Mathf.Max(noise, minValue);

        return noise;
    }

}
