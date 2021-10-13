using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    static InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
    static InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;

    private InputDevice targetDevice;
    public GameObject ControllerPrefab;

    private GameObject SpawnedController;

    private MeshRenderer SpawnedControllerRender;

    public UnityEngine.XR.Interaction.Toolkit.XRDirectInteractor interactor;

    public UnityEvent PrimaryButtonEvents;
    public UnityEvent SencondaryButtonEvents;
    public UnityEvent TriggerEvent;

    public bool IsRight = true;

    public SphereCollider HandCollider;


    //----------Control Helpers---------------------//

    bool triggerPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();

        if (IsRight)
        {
            InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
        }
        else
        {
            InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);
        }

        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }

        SpawnedController = Instantiate(ControllerPrefab,transform);

        SpawnedControllerRender = SpawnedController.GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float GripValue) && GripValue >= 0.8f)
        {
            //Try grabbed item
            HandCollider.radius = 0f;
        }
        else
        {
            HandCollider.radius = 0.1f;
        }
        
        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
        {
            //Debug.Log("pressing Primary Button");
            PrimaryButtonEvents.Invoke();
        }
        
        if (targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue) && secondaryButtonValue)
        {
            //Debug.Log("pressing secondary Button");
            SencondaryButtonEvents.Invoke();
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.6f)
        {
            //Debug.Log("Trigger Pressed " + triggerValue);
            if (!triggerPressed)
            {
                triggerPressed = true;
                triggerInitPress();
            }
        }
        else
        {
            if (triggerPressed)
            {
                triggerPressed = false;
                triggerReleasedPress();
            }
        }
        
        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue) && primary2DAxisValue != Vector2.zero)
        {
            //Debug.Log("Primary Touchpad " + primary2DAxisValue);
        }
    }

    void triggerInitPress()
    {
        TriggerEvent.Invoke();
    }
    void triggerReleasedPress()
    {
        //does nothing for now
    }

    public void TurnInvisable()
    {
        SpawnedControllerRender.enabled = false;
    }
    public void TurnVisable()
    {
        SpawnedControllerRender.enabled = true;
    }
}
