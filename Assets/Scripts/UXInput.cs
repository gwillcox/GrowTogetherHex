using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UXInput : MonoBehaviour
{
    public GameObject world;
    private TimeController timeController;
    public GameObject eggBoy;
    public movement eggBoyMovementController;
    private rotateCamera eggBoyCameraController; 

    public GameObject gameMenu;
    public Text timeText;
    public Slider timeSlider;
    public Text scaleText;
    public Slider scaleSlider;


    // Start is called before the first frame update
    void Start()
    {
        timeController = world.GetComponent<TimeController>();
        eggBoyCameraController = eggBoy.GetComponentInChildren<rotateCamera>();
        eggBoyMovementController = eggBoy.GetComponentInChildren<movement>();
        UpdateTimeScale();
        UpdateSpatialScale();
        gameMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            gameMenu.SetActive(!gameMenu.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.V)) { 
            eggBoyCameraController.SetFirstPersonView(!eggBoyCameraController.firstPersonView);
        }

        if (!gameMenu.activeSelf)
        {
            eggBoyMovementController.Move(Input.GetAxis("Vertical"), Input.GetAxis("Mouse X") + Input.GetAxis("Horizontal"));
            eggBoyCameraController.RotateView(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

    }

    public void UpdateTimeScale()
    {
        float newTimeScale = Mathf.Exp(timeSlider.value) - Mathf.Exp(timeSlider.minValue);
        GlobalSingletons.Instance.timeScale = newTimeScale;
        timeText.text = newTimeScale.ToString();
        Debug.Log($"Updated Time Scale: {GlobalSingletons.Instance.timeScale}");
    }

    public void UpdateSpatialScale()
    {
        float newSpatialScale = Mathf.Exp(scaleSlider.value) - Mathf.Exp(scaleSlider.minValue);
        GlobalSingletons.Instance.spaceScale = newSpatialScale;
        scaleText.text = newSpatialScale.ToString();
        Debug.Log($"Updated Space Scale: {GlobalSingletons.Instance.spaceScale}");
    }
}
