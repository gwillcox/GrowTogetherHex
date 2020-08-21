using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexElement : MonoBehaviour
{
	Vector3[] vertices;
	int[] triangles;
	HexElement hexElement;
	Canvas gridCanvas;
	Mesh mesh;

	public const float outerRadius = 10f;
	public const float innerRadius = outerRadius * 0.86602f;
	public Vector3 center;

	public HexElement(Vector3 centerVector)
    {
		center = centerVector;
    }

	private void Initialize(Vector3 center)
	{
		hexElement = GetComponentInChildren<HexElement>();
		gridCanvas = GetComponentInChildren<Canvas>();
		hexElement.name = "HexElement";
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		transform.position = center;

		mesh.Clear();

		vertices = new Vector3[] {
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, outerRadius),
			new Vector3(innerRadius, 0f, 0.5f * outerRadius),
			new Vector3(innerRadius, 0f, -0.5f * outerRadius),
			new Vector3(0f, 0f, -outerRadius),
			new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
			new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
			new Vector3(0f, 0f, outerRadius)
		};
		mesh.vertices = vertices;

		for (int i = 1; i < 7; i++)
		{
			addTriangle(0, i, i + 1);
		}
		mesh.triangles = triangles;

		void addTriangle(int v1, int v2, int v3)
		{
			triangles[triangles.Length] = v1;
			triangles[triangles.Length] = v2;
			triangles[triangles.Length] = v3;
		}
	}
}
