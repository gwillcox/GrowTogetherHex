using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[CreateAssetMenu()]
public class PlantSettings : ScriptableObject {

    [HideInInspector]
    public bool foldout;

    public GameObject plantObject;

    public AnimationCurve temperatureAffinity;
    public AnimationCurve rainfallAffinity;

    public float deathThreshold;
    public float reproductionThreshold;

    public float calcAffinity(float temperature, float rainfall)
    {
        Debug.Log($"{temperatureAffinity.Evaluate(1f)}, {temperature}, {rainfall}");
        return temperatureAffinity.Evaluate(temperature) * rainfallAffinity.Evaluate(rainfall);
    }

}
