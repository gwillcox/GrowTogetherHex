using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Plant: MonoBehaviour
{
    private PlantSettings _plantSettings;
    Vector3 _position;
    Biome _biome;
    Planet _planet;
    private Mesh plantMesh;

    private float scale = 1f;
    [SerializeField]
    private float widestHeight = 0.1f;
    [SerializeField]
    private float widestRadius = 1f;

    public Vector3 rayDir;
    public Vector3 rayStart;

    public int meshInt;

    public float shade = 0f;

    public Vector3 shadePos;

    void Start()
    {
    }

    public void Init(Planet planet, Biome biome, Vector3 position, PlantSettings plantSettings)
    {
        _plantSettings = plantSettings;
        _position = position;
        _biome = biome;
        _planet = planet;
        scale = 0.05f;
        gameObject.transform.localScale = Vector3.one * scale; // make the plant small to start. 

        plantMesh = gameObject.GetComponentInChildren<MeshFilter>().mesh;
    }

    void Update()
    {
        if (_biome != null)
        {
            ProcessBiomeConditions(_biome._conditions);
        }
    }

    public BiomeConditions ProcessBiomeConditions(BiomeConditions conditions)
    {
        shade = CalcRayShade();

        float plantAffinity = _plantSettings.calcAffinity(conditions.Temperature, conditions.Rainfall, shade);

        float r_num = Random.Range(0, 1f);

        if (plantAffinity < _plantSettings.deathThreshold)
        {
            Debug.Log($"Killing: {plantAffinity}");
            this.KillPlant();
        }
        else
        {
            this.Grow();
        }
        
        if (plantAffinity > _plantSettings.reproductionThreshold && r_num > 0.999f)
        {
            this.Reproduce();
        }
        return null;
    }

    // TODO: update this to assume the sun is coming from the equator
    public float CalcSentShade(Vector3 location)
    {
        float castShade = 0f;
        float distanceToRim = 0f;
        float theta = 0f;

        // Location is the top of the light-recieving circle of the plant in question. 
        float heightDifference = scale * widestHeight - location[2];
        if (heightDifference > 0)
        {
            distanceToRim = new Vector2(_position.x - location.x, _position.y - location.y).magnitude - widestRadius * scale;
            theta = Mathf.Atan2(heightDifference, distanceToRim);

            castShade = Mathf.InverseLerp(Mathf.PI / 2 - 1f, Mathf.PI / 2 + 1f, theta);
        }

        castShade = (1 - shade) * castShade;

        return castShade;
    }

    public float CalcRayShade()
    { 
        int count = 0;
        int numRays = 1;
        // Calculates ambient light by determining whether a given random ray can see the sky or not. 
        Vector3 selectedVertex = transform.position + Quaternion.Euler(0, 0, 0) * plantMesh.vertices[Random.Range(0, plantMesh.vertices.Length)];
        for (int i = 0; i < numRays; i++)
        {
            selectedVertex = transform.position + Quaternion.Euler(0, 0, 0) * plantMesh.vertices[Random.Range(0, plantMesh.vertices.Length)];
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            bool rayHitsSky = !Physics.Raycast(selectedVertex, randomDirection);
            count += rayHitsSky?1:0;
        }

        Vector3 sunDirection = _planet.sun.transform.position - transform.position;
        bool rayHitsSun = !Physics.Raycast(selectedVertex, sunDirection);
        float sunlight = rayHitsSun ? 1f : 0f;

        if (rayHitsSun) { Debug.Log("HitSun"); }

        shade = shade * .95f + .01f * count / numRays + 0.04f * sunlight;
        return shade;
    }

    public Vector3 GetShadeCircleCenter()
    {
        return transform.position + Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(0, 0, scale * widestHeight);
    }

    public float GetShadeRadius()
    {
        return scale * widestRadius;
    }

    private void Grow()
    { 
        scale = Mathf.Min(scale, 2f) * (1 + .01f * Time.deltaTime);
        this.transform.localScale = Vector3.one * scale;

        // Iterate over plants. If any are too close, destroy them! 
        foreach (var pl in _biome._plants)
        {
            if (pl != this)
            {
                float dist = Vector3.Distance(pl.transform.position, transform.position);
/*                if (dist < scale) // TODO: Handle deletion of plants better.
                {
                    pl.KillPlant();
                }*/
            }
        }
    }

    private void Reproduce()
    {
        // Todo: make this more intelligent.
        Vector3 randomDisplacement = Random.onUnitSphere * 5;
        Vector3 polarPosition = SphericalGeometry.WorldToPolar(randomDisplacement + _position);
        PlacePlant.placeNew(polarPosition, _planet, _plantSettings);
    }

    public void KillPlant()
    {
        _biome.KillPlant(this);
        GameObject.Destroy(this.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, scale);
        Handles.Label(transform.position + new Vector3(scale, 0, 0), $"This plant's shade: {shade}");

        // Draw and label the shade circle
        Gizmos.DrawWireSphere(GetShadeCircleCenter(), GetShadeRadius());
        Handles.Label(GetShadeCircleCenter() + Quaternion.Euler(transform.localRotation.eulerAngles) * new Vector3(0, 0, GetShadeRadius()), $"Shade Circle");

        Gizmos.DrawWireSphere(transform.position + shadePos, 0.25f);
        Handles.Label(transform.position + shadePos * 1.1f, $"Shade: {shade}");

        // Calculates ambient light by determining whether a given random ray can see the sky or not. 
        Vector3 selectedVertex = transform.position + Quaternion.Euler(0, 0, 0) * plantMesh.vertices[meshInt];
        bool rayHitsSky = !Physics.Raycast(selectedVertex, rayDir);

        Gizmos.color = rayHitsSky ? Color.white : Color.red;
        Gizmos.DrawLine(selectedVertex, selectedVertex + (rayDir*100));
    }
}
