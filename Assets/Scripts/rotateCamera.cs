using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class rotateCamera : MonoBehaviour
{
    public bool firstPersonView { get; private set; }
    private Camera camera;
    Quaternion cameraRotation = Quaternion.identity;
    public float cameraSensitivity = 3f;
    public float lookTowardsX = 0f;
    public float lookTowardsY = 0f;
    public float cameraZoom = 1f;
    float cameraDistance = 4f;
    public GameObject eggBoyBody;

    private GameObject firstPersonCanvas;
    private Text interactionText;

    public RaycastHit newHit;
    public RaycastHit hit;
    public bool hitBool = false;
    public bool newHitBool = false;
    public LayerMask RayIgnoreLayer;

    public Plant selectedPlant; 


    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        firstPersonCanvas = GetComponentInChildren<Canvas>().gameObject;
        interactionText = GetComponentInChildren<Text>();
        firstPersonView = true;

        RayIgnoreLayer = LayerMask.GetMask("EggBoy");
    }

    public void RotateView(float rotationX, float rotationY)
    {
        lookTowardsX = (lookTowardsX - rotationX * cameraSensitivity) % 360f;
        lookTowardsY = Mathf.Clamp(lookTowardsY + rotationY * cameraSensitivity, -89f, 89f);
    }

    public void SetFirstPersonView(bool firstPerson)
    {
        firstPersonView = firstPerson;

        if (firstPersonView)
        {
            cameraSensitivity = 3f;
            firstPersonCanvas.SetActive(true);
        }
        else
        {
            cameraSensitivity = 0.6f;
            firstPersonCanvas.SetActive(false);
        }

        ResetViewDirs();
    }

    private void ResetViewDirs()
    {
        lookTowardsY = 0f;
        lookTowardsX = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateView();

        CheckForInteractables();

        if (Input.GetKeyDown(KeyCode.Q) && hitBool && hit.collider.gameObject.CompareTag("Plant"))
        {
            selectedPlant = hit.collider.gameObject.GetComponentInParent<Plant>();
            Debug.Log($"Selected Plant: {selectedPlant}");
            interactionText.text = hit.collider.gameObject.name +
                ":\n    Age: " + Math.Round(selectedPlant.age, 2).ToString() + " minutes old" +
                ":\n    Health: " + Math.Round(selectedPlant.health * 100f, 2).ToString() + "%" +
                ":\n    Height: " + Math.Round(selectedPlant.scale, 2).ToString() + " meters tall" +
                ":\n    Light Amount: " + Math.Round(selectedPlant.illumination * 100f, 2).ToString() + "%";
        }
    }

    void CheckForInteractables()
    {
        newHitBool = Physics.Raycast(transform.position, transform.forward, out newHit, 100f, ~RayIgnoreLayer);
        if (newHitBool & (newHit.collider != hit.collider))
        {
            Debug.Log("HIT");
            hit = newHit;
            hitBool = newHitBool;
            // if raycast hits, it checks if it hit an object with the tag Player
            if (hit.collider.gameObject.CompareTag("Plant"))
            {
                interactionText.text = hit.collider.gameObject.name + ":\nPress Q to analyze";/*
                interactionText.transform.position = Camera.main.WorldToScreenPoint(hit.transform.position);
                Debug.Log(hit.transform.position);*/
            }
            else { interactionText.text = ""; }
        }/*
        else if (hitBool)
        {
            Debug.Log($"{hitBool} {newHitBool}, {hit}");
            // keep the text in the same world space
            interactionText.transform.position = Camera.main.WorldToScreenPoint(hit.transform.position);
        }*/

    }

    void UpdateView()
    {
        if (firstPersonView)
        {
            transform.localPosition = eggBoyBody.transform.localPosition + new Vector3(0, 0.5f, 0);

            transform.LookAt(transform.position + new Vector3(
                -1f,
                Mathf.Sin(lookTowardsY * Mathf.PI / 180f),
                0
                ));
        }
        else
        {
            cameraZoom = cameraZoom - cameraZoom * Input.GetAxis("Mouse ScrollWheel") * 0.5f;
            cameraZoom = Mathf.Min(9f, Mathf.Max(cameraZoom, 0.8f));

            transform.localPosition = new Vector3(
                cameraZoom * cameraDistance * (float)Math.Cos(lookTowardsX),
                cameraZoom * 2 + eggBoyBody.transform.localPosition.y,
                cameraZoom * cameraDistance * (float)Math.Sin(lookTowardsX));

            transform.LookAt(eggBoyBody.transform.position + new Vector3(0, 100 * Mathf.Sin(lookTowardsY * Mathf.PI / 180f), 0));
        }
    }

    void OnDrawGizmosSelected()
    {
        RaycastHit tempHit;
        var temphitBool = Physics.Raycast(transform.position, transform.forward, out tempHit, 100f, ~RayIgnoreLayer);
        if (temphitBool) { 
            Gizmos.color = (tempHit.collider.gameObject.tag == "Plant") ? Color.red : Color.yellow;

            var p = Camera.main.WorldToScreenPoint(tempHit.collider.gameObject.transform.position);
            Handles.Label(tempHit.transform.position, $"Position: {tempHit.transform.position}");
        }
        else { Gizmos.color = Color.white; }
        Gizmos.DrawLine(transform.position, transform.forward * 100 + transform.position);
    }
}
