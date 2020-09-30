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
    public AnimationCurve ageAffinity;

    public float deathThreshold;
    public float sickThreshold = 0.3f;
    public float reproductionThreshold;

    public float calcAffinity(float temperature, float rainfall, float illumination, float age)
    {
        return temperatureAffinity.Evaluate(temperature)
            * rainfallAffinity.Evaluate(rainfall)
            * illuminationAffinity.Evaluate(illumination)
            * ageAffinity.Evaluate(age);
    }

}
