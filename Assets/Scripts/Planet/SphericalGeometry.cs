using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalGeometry 
{
    public static Vector3[] ConvertPolarToWorld(Vector3[] positionPolar)
    {
        Vector3[] positionWorld = new Vector3[positionPolar.Length];

        for (int i = 0; i < positionPolar.Length; i++)
        {
            positionWorld[i] = (PolarToWorld(positionPolar[i]));
        }

        return positionWorld;
    }

    public static Vector3 PolarToWorld(Vector3 positionPolar)
    {
        float radius = positionPolar.x;
        float phi = positionPolar.y;
        float theta = positionPolar.z;

        return new Vector3(
            radius * Mathf.Sin(theta) * Mathf.Cos(phi),
            positionPolar.x * Mathf.Sin(theta) * Mathf.Sin(phi),
            positionPolar.x * Mathf.Cos(theta)
            );
    }

    public static Vector3 WorldToPolar(Vector3 worldCoordinates)
    {

        float radius = worldCoordinates.magnitude;
        float phi = Mathf.Atan2(worldCoordinates.y, worldCoordinates.x);
        float theta = Mathf.Acos(worldCoordinates.z / radius);

        return new Vector3(radius, phi, theta);
    }
}
