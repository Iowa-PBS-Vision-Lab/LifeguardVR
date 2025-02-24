using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System;
public class SelectSwimmer : MonoBehaviour
{
    //The raycast of the hand we want to use...
    public XRRayInteractor rayInteractor;
    private bool triggerValue;
    // Update is called once per frame
    void Start(){
    }
    void Update()
    {
        var devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, devices);
        if(devices.Count > 0){
            var device = devices[0];
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit)) {
                //Print the name of the interactor to the console.
                if (hit.collider.transform.parent.gameObject.name.Contains("(Drowner)") && device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue){
                    Debug.Log("The player has found the drowner!");
                    XRInputLogger.LogCustomEvent($"User selected {hit.collider.transform.parent.gameObject.name}");
                }
            }
        }
        //Double check that we are interacting with a collider of some sort.
        
    }
}

public class XRControllerInputLogger : MonoBehaviour
{
    // 1) Autocreate this script before any scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void CreateControllerLogger()
    {
        // Avoid duplicates if already present
        if (!FindObjectOfType<XRControllerInputLogger>())
        {
            GameObject loggerObj = new GameObject("XRControllerInputLogger");
            loggerObj.AddComponent<XRControllerInputLogger>();
            Debug.Log("[XRControllerInputLogger] Automatically created.");
        }
    }

    void Update()
    {
        // 2) Enumerate all XR devices
        var devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);

        foreach (var device in devices)
        {
            // Example: Log trigger value (if > 0.1)
            if (device.TryGetFeatureValue(CommonUsages.trigger, out float triggerVal) && triggerVal > 0.1f)
            {
                XRInputLogger.LogCustomEvent($"Trigger pressed on {device.name}, value={triggerVal:F2}");
            }

            // Example: Log grip value (if > 0.1)
            if (device.TryGetFeatureValue(CommonUsages.grip, out float gripVal) && gripVal > 0.1f)
            {
                XRInputLogger.LogCustomEvent($"Grip pressed on {device.name}, value={gripVal:F2}");
            }

            // Example: Log primary button press
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryPressed) && primaryPressed)
            {
                XRInputLogger.LogCustomEvent($"Primary button pressed on {device.name}");
            }

            // Example: Log secondary button press
            if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryPressed) && secondaryPressed)
            {
                XRInputLogger.LogCustomEvent($"Secondary button pressed on {device.name}");
            }

            // Example: Log menu button press
            if (device.TryGetFeatureValue(CommonUsages.menuButton, out bool menuPressed) && menuPressed)
            {
                XRInputLogger.LogCustomEvent($"Menu button pressed on {device.name}");
            }

            // Example: Log thumbstick movement
            if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axisVal) && axisVal.magnitude > 0.1f)
            {
                XRInputLogger.LogCustomEvent($"Thumbstick moved on {device.name}, axis=({axisVal.x:F2}, {axisVal.y:F2})");
            }
        }
    }
}