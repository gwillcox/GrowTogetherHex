using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public bool flatWorld;
    public GameObject world;

    public float movementSpeed;
    public float turnSpeed;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        float wobble = (float)Math.Abs(Math.Sin(Time.time * 5))/500;
        float forwardMovement = Input.GetAxis("Vertical");
        float rotationAmount = Input.GetAxis("Horizontal");

        if (flatWorld)
        {
            transform.position = transform.position - transform.rotation * new Vector3(forwardMovement / 100, 0, 0) + new Vector3(0, wobble, 0);
            transform.Rotate(new Vector3(0f, 1f, 0f), rotationAmount);
        }
        else
        {

            world.transform.InverseTransformVector(new Vector3(1, 0, 0));
            world.transform.Rotate(world.transform.InverseTransformVector(new Vector3(0, 0, -1)), forwardMovement / 25 * movementSpeed);
            world.transform.Rotate(world.transform.InverseTransformVector(new Vector3(0, -1, 0)), rotationAmount * turnSpeed);
            transform.position = transform.position + new Vector3(0, wobble, 0);
        }

    }
}
