using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant: MonoBehaviour
{
    private PlantSettings _plantSettings;
    Vector3 _position;
    Biome _biome;
    Planet _planet;

    void Start()
    {
    }

    public void Init(Planet planet, Biome biome, Vector3 position, PlantSettings plantSettings)
    {
        _plantSettings = plantSettings;
        _position = position;
        _biome = biome;
        _planet = planet;
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
        float plantAffinity = _plantSettings.calcAffinity(conditions.Temperature, conditions.Rainfall);

        float r_num = Random.Range(0, 1f);

        if (plantAffinity < _plantSettings.deathThreshold)
        {
            this.KillPlant();
        }
        else
        {
            this.Grow();
        }
        
        if (plantAffinity > _plantSettings.reproductionThreshold && r_num > 1f)
        {
            this.Reproduce();
        }
        return null;
    }

    private void Grow()
    {
        this.transform.localScale = Vector3.one * Mathf.Min(this.transform.localScale[0], 5f) * (1 + .01f*Time.deltaTime);

        // not done yet
    }

    private void Reproduce()
    {
        // Todo: make this more intelligent.
        Vector3 randomDisplacement = Random.onUnitSphere * 20;
        Vector3 polarPosition = SphericalGeometry.WorldToPolar(randomDisplacement + _position);
        PlacePlant.placeNew(polarPosition, _planet, _plantSettings);
    }
    public void KillPlant()
    {
        _biome.KillPlant(this);
        GameObject.DestroyImmediate(this.gameObject);
    }
}
