using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSphere
{

    public static (List<Vector3>, List<int>) CreateIcosahedron(int numPoints)
    {

        Vector3[] positionPolar = new[] {
            new Vector3(1f, 0f, 0f*Mathf.PI/180),
            new Vector3(1f, 0f, 90*Mathf.PI/180),
            new Vector3(1f, 90f*Mathf.PI/180, 90f*Mathf.PI/180),
            new Vector3(1f, 180f*Mathf.PI/180, 90f*Mathf.PI/180),
            new Vector3(1f, 270f*Mathf.PI/180, 90f*Mathf.PI/180),
            new Vector3(1f, 0f*Mathf.PI/180, 180f*Mathf.PI/180)};

        Vector3[] vertices = SphericalGeometry.ConvertPolarToWorld(positionPolar);

        return InterpolateWorldPoints(vertices, numPoints);
    }

    private static (List<Vector3>, List<int>) InterpolateWorldPoints(Vector3[] positionWorld, int numPoints)
    {
        
        List<Vector3> interpPoints = new List<Vector3>();
        List<int> interpTris = new List<int>();

        // First, get all vertex positons
        InterpolateTriangle(positionWorld[0], positionWorld[1], positionWorld[2]);
        InterpolateTriangle(positionWorld[0], positionWorld[2], positionWorld[3]);
        InterpolateTriangle(positionWorld[0], positionWorld[3], positionWorld[4]);
        InterpolateTriangle(positionWorld[0], positionWorld[4], positionWorld[1]);
        InterpolateTriangle(positionWorld[5], positionWorld[2], positionWorld[1]);
        InterpolateTriangle(positionWorld[5], positionWorld[3], positionWorld[2]);
        InterpolateTriangle(positionWorld[5], positionWorld[4], positionWorld[3]);
        InterpolateTriangle(positionWorld[5], positionWorld[1], positionWorld[4]);

        void InterpolateTriangle(Vector3 X1, Vector3 X2, Vector3 X3)
        {
            for (int k = 0; k <= numPoints; k++)
            {

                for (int l = 0; l <= numPoints - k; l++)
                {

                    Vector3 newVertex = CalculateVertexPosition(X1, X2, X3, k, l, numPoints);

                    if (!interpPoints.Contains(newVertex))
                    {
                        interpPoints.Add(newVertex);
                    }

                    // Set upwards-facing triangles
                    if (k != numPoints && l != numPoints - k)
                    {
                        interpTris.Add(FindIndex(ref interpPoints, newVertex));

                        Vector3 vertex2 = CalculateVertexPosition(X1, X2, X3, k + 1, l, numPoints);
                        interpTris.Add(FindIndex(ref interpPoints, vertex2));


                        Vector3 vertex3 = CalculateVertexPosition(X1, X2, X3, k, l + 1, numPoints);
                        interpTris.Add(FindIndex(ref interpPoints, vertex3));

                    }

                    // Sets downwards-facing traingles
                    if (k < numPoints - 1 && l < numPoints - k && l > 0)
                    {
                        interpTris.Add(FindIndex(ref interpPoints, newVertex));

                        Vector3 vertex2 = CalculateVertexPosition(X1, X2, X3, k + 1, l - 1, numPoints);
                        interpTris.Add(FindIndex(ref interpPoints, vertex2));

                        Vector3 vertex3 = CalculateVertexPosition(X1, X2, X3, k + 1, l, numPoints);
                        interpTris.Add(FindIndex(ref interpPoints, vertex3));
                    }

                }

            }
        }

        return (interpPoints, interpTris);
    }


    static int FindIndex(ref List<Vector3> interpPoints, Vector3 vertex)
    {
        int idx = interpPoints.FindIndex(x => x == vertex);
        if (idx < 0)
        {
            interpPoints.Add(vertex);
            idx = interpPoints.FindIndex(x => x == vertex);
        }
        return idx;
    }


    static Vector3 CalculateVertexPosition(Vector3 X1, Vector3 X2, Vector3 X3, int X2_index, int X3_index, int numPoints)
    {
        float frac2 = (float)(X2_index) / (numPoints);
        float frac3 = (float)(X3_index) / (numPoints);
        Vector3 newVertex = X1 * (1 - frac2 - frac3) + X2 * frac2 + X3 * frac3;
        return newVertex;
    }
}