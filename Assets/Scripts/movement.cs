using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float wobble = (float)Math.Sin(Time.time * 5)/3000;
        float forwardMovement = Input.GetAxis("Vertical");
        float rotationAmount = Input.GetAxis("Horizontal");
        transform.position = transform.position - transform.rotation * new Vector3(forwardMovement / 100, 0, 0) + new Vector3(0, wobble, 0);
/*
        // Apply gravity
        transform.position = transform.position - new Vector3(0, 0.01f, 0);*/

        transform.Rotate(new Vector3(0, 1, 0), rotationAmount);

    }
}
