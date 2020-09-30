using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSingletons : MonoBehaviour
{
    public static GlobalSingletons Instance { get; private set; }

    public float timeScale;
    public float spaceScale;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
