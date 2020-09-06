using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCamera : MonoBehaviour
{
    Quaternion cameraRotation = Quaternion.identity;
    public float lookTowardsX = 0f;
    public float lookTowardsY = 0f;
    public float cameraZoom = 1f;
    public GameObject eggBoyBody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float cameraRotationHorizontal=0;
        float cameraRotationVertical=0;

        if (Input.GetAxis("Fire1")>0)
        {
            cameraRotationHorizontal = Input.GetAxis("Mouse X");
            cameraRotationVertical = Input.GetAxis("Mouse Y");
            lookTowardsX -= cameraRotationHorizontal / 5;
            lookTowardsY += cameraRotationVertical / 5;
        }
        
        cameraZoom = cameraZoom - cameraZoom * Input.GetAxis("Mouse ScrollWheel")*0.5f;
        cameraZoom = Mathf.Min(9f, Mathf.Max(cameraZoom, 0.8f));
        
        float cameraDistance = 4f;
        transform.localPosition = new Vector3( 
            cameraZoom * cameraDistance*(float)Math.Cos(lookTowardsX),
            cameraZoom * 2 + eggBoyBody.transform.localPosition.y, 
            cameraZoom * cameraDistance*(float)Math.Sin(lookTowardsX)); 

        transform.LookAt(eggBoyBody.transform.position+new Vector3(0, lookTowardsY, 0));
    }
}
