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

    public Action<Vector3> PlayerMoved; 

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponentInChildren<Rigidbody>();
    }

    public void Move(float forward, float rotation)
    {

        rigidBody.AddForce(new Vector3(0, -1, 0));
        float forwardMovement = forward * Time.deltaTime * GlobalSingletons.Instance.spaceScale;
        float rotationAmount = rotation * Time.deltaTime;

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

        PlayerMoved?.Invoke(transform.position - world.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
