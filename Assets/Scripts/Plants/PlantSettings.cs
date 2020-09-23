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
    public AnimationCurve illuminationAffinity;

    public float deathThreshold;
    public float reproductionThreshold;

    public float calcAffinity(float temperature, float rainfall, float illumination)
    {
        return temperatureAffinity.Evaluate(temperature) * rainfallAffinity.Evaluate(rainfall) * illuminationAffinity.Evaluate(illumination);
    }

}
