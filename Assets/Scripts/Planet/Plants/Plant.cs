using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant
{
    public PlantSettings _plantSettings;
    Vector3 _position;
    Biome _biome;
    Planet _planet;
    public GameObject _plantObject;

    public Plant(Planet planet, Biome biome, Vector3 position, PlantSettings plantSettings)
    {
        _plantSettings = plantSettings;
        _position = position;
        _biome = biome;
        _planet = planet;

        _plantObject = GameObject.Instantiate(
            _plantSettings.plantObject,
            _planet.transform.localToWorldMatrix.MultiplyPoint(_position),
            Quaternion.LookRotation(_position),
            _planet.transform);
    }

    public BiomeConditions ProcessBiomeConditions(BiomeConditions conditions)
    {
        float plantAffinity = _plantSettings.calcAffinity(conditions.Temperature, conditions.Rainfall);
        Debug.Log($"Affinity: {plantAffinity}");

        if (plantAffinity < _plantSettings.deathThreshold)
        {
            Debug.Log("Killed");
            this.KillPlant();
        }
        else
        {
            Debug.Log("Grew");
            this.Grow();
        }
        
        if (plantAffinity > _plantSettings.reproductionThreshold)
        {
            Debug.Log("Reproduced");
            this.Reproduce();
        }
        return null;
    }

    private void Grow()
    {
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
        GameObject.DestroyImmediate(_plantObject);
        _biome.KillPlant(this);
    }
}
