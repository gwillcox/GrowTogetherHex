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

    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponentInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.AddForce(new Vector3(0, -1, 0));
        float forwardMovement = Input.GetAxis("Vertical") * Time.deltaTime / Time.timeScale;
        float rotationAmount = Input.GetAxis("Horizontal") * Time.deltaTime / Time.timeScale;

        if (flatWorld)
        {
            transform.position = transform.position - transform.rotation * new Vector3(forwardMovement / 100, 0, 0);
            transform.Rotate(new Vector3(0f, 1f, 0f), rotationAmount);
        }
        else
        {

            world.transform.InverseTransformVector(new Vector3(1, 0, 0));
            world.transform.Rotate(world.transform.InverseTransformVector(new Vector3(0, 0, -1)), forwardMovement / 25 * movementSpeed);
            world.transform.Rotate(world.transform.InverseTransformVector(new Vector3(0, -1, 0)), rotationAmount * turnSpeed);
        }

    }
}
