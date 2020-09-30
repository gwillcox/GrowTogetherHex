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
    private MeshRenderer plantMeshRenderer;
    private Vector3[] plantVertexArray;
    private Transform plantMeshTransform;

    public Material plantHealthyMaterial;
    public Material plantSickMaterial;
    public Material plantDyingMaterial;

    public float scale = 1f;
    public int meshInt;
    public float illumination = 0f;
    public float affinity;
    public float health = 1f;
    public float age = 0f;

    private float directLight = 0f;
    private float indirectLight = 0f;
    private Vector3 testMeshVertex;

    public bool showMesh = false;

    void Start()
    {
    }
    void Update()
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
        plantMeshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        plantMeshTransform = transform.GetChild(0).GetComponent<Transform>(); // Assumes that every plant has exactly one child transform.

        plantVertexArray = plantMesh.vertices;
        UpdateRandomVertex();
    }

    private void IncrementAge()
    {
        age += Time.deltaTime * GlobalSingletons.Instance.timeScale / 60f;
    }

    public BiomeConditions ProcessBiomeConditions(BiomeConditions conditions)
    {
        IncrementAge();
        illumination = CalcRayShade();

        affinity = _plantSettings.calcAffinity(conditions.Temperature, conditions.Rainfall, illumination, age);
        health = RecalculateHealth();

        if (health < _plantSettings.deathThreshold)
        {
            this.KillPlant();
        }
        else if (health < _plantSettings.sickThreshold)
        {
            if (plantMeshRenderer.sharedMaterial != plantSickMaterial)
            {
                this.DisplaySickness();
            }
        }
        else
        {
            if (plantMeshRenderer.sharedMaterial != plantHealthyMaterial)
            {
                this.DisplayHealth();
            }
            this.Grow();
        }
        
        if (affinity > _plantSettings.reproductionThreshold && Random.Range(0,1f)> 0.999f)
        {
            this.Reproduce();
        }
        return null;
    }

    private void DisplayHealth()
    {
        plantMeshRenderer.sharedMaterial = plantHealthyMaterial;
    }

    private void DisplaySickness()
    {
        plantMeshRenderer.sharedMaterial = plantSickMaterial;
    }

    private float RecalculateHealth()
    { 
        // Calculate health using a 99% - 1% scaling. 
        return health * .95f + affinity * 0.05f;
    }

    public float CalcRayShade()
    {
        // Update the vertex we want to use once every 100 frames. This appears compute-intensive. 
        if (Random.Range(0f, 1f) < 0.01)
        {
            UpdateRandomVertex();
        }

        // Calculates ambient light by determining whether a given random ray can see the sky or not. 
        indirectLight = Physics.Raycast(
            testMeshVertex,
            new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized
            ) ? 0 : 1;

        directLight = RayHitsSun() ? 1f : 0f;

        // Todo: allow plants to process this differently. 
        illumination = illumination * .95f + .01f * indirectLight + 0.04f * directLight;

        return illumination;
    }

    public bool RayHitsSun()
    {
        return !Physics.Raycast(
            testMeshVertex,
            _planet.sun.transform.position - transform.position,
            500f);
    }

    public void UpdateRandomVertex()
    {
        testMeshVertex = plantMeshTransform.localToWorldMatrix.MultiplyPoint(
                        plantVertexArray[Random.Range(0, plantMesh.vertices.Length)]
                        );
    }

    private void Grow()
    { 
        scale = Mathf.Min(scale, 2f) * (1 + affinity * .03f * Time.deltaTime * GlobalSingletons.Instance.timeScale);
        this.transform.localScale = Vector3.one * scale;
/*
        // Iterate over plants. If any are too close, destroy them! 
        foreach (var pl in _biome._plants)
        {
            if (pl != this)
            {
                float dist = Vector3.Distance(pl.transform.position, transform.position);
*//*                if (dist < scale) // TODO: Handle deletion of plants better.
                {
                    pl.KillPlant();
                }*//*
            }
        }*/
    }

    private void Reproduce()
    {
        // Todo: make this more intelligent.
        PlacePlant.placeNew(
            SphericalGeometry.WorldToPolar(Random.onUnitSphere * 5 + _position),
            _planet, 
            _plantSettings
            );
    }

    public void KillPlant()
    {
        _biome.KillPlant(this);

    }

    void OnDrawGizmosSelected()
    {
        if (Selection.activeGameObject != transform.gameObject)
        {
            return;
        }

        if (showMesh)
        {
            foreach (var vertex in plantMesh.vertices)
            {
                Gizmos.color = Color.grey;
                Gizmos.DrawWireSphere(plantMeshTransform.localToWorldMatrix.MultiplyPoint(
                        vertex
                        ),
                    0.05f
                    );
            }
        }
        Gizmos.DrawWireSphere(transform.position, scale);
        Handles.Label(transform.position - scale * Vector3.one, $"Illumination: {illumination} \n Health: {health}");

        // Calculates and plots whether the plant can see the sun. 
        Vector3 selectedVertex = testMeshVertex;
        bool hit = !Physics.Raycast(selectedVertex, _planet.sun.transform.position - transform.position, 500f);
        Gizmos.color = hit ? Color.white : Color.red;
        Gizmos.DrawLine(selectedVertex, _planet.sun.transform.position);

    }
}
