using System.Collections.Generic;
using UnityEngine;

public class BiomeManager
{
    private List<Biome> _biomes;

    public void Tick()
    {
        foreach (var biome in _biomes)
        {
            biome.Tick();
        }
    }
}