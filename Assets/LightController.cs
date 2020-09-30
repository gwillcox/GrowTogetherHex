using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public float timeScale;
    public float distance;
    public float timePassed; 

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime * 0.05f * GlobalSingletons.Instance.timeScale;
        transform.localPosition = distance * new Vector3(Mathf.Sin(timePassed),  0, Mathf.Cos(timePassed));
        transform.LookAt(new Vector3(0, 0, 0));
    }
}
