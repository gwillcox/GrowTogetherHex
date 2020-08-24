using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCamera : MonoBehaviour
{
    Quaternion cameraRotation = Quaternion.identity;
    public float lookTowardsX = 0f;
    public float lookTowardsY = 0f;
    public GameObject eggBoyBody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float cameraRotationHorizontal = Input.GetAxis("Mouse X");
        float cameraRotationVertical = Input.GetAxis("Mouse Y");
        lookTowardsX -= cameraRotationHorizontal/5;
        lookTowardsY += cameraRotationVertical/10;
        float cameraVerticalPosition = 1.2f + eggBoyBody.transform.localPosition.y;
        float cameraDistance = 4f;

        transform.localPosition = new Vector3( 
            cameraDistance*(float)Math.Cos(lookTowardsX), 
            cameraVerticalPosition, 
            cameraDistance*(float)Math.Sin(lookTowardsX));
        Debug.Log($"Look to X: {Math.Cos(lookTowardsX)}");

        transform.LookAt(eggBoyBody.transform.position+new Vector3(0, lookTowardsY, 0));
    }
}
