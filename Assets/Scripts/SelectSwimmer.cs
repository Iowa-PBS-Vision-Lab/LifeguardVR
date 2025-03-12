using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class SelectSwimmer : MonoBehaviour
{
    // The raycast interactor objects for the left and right hands.
    public GameObject LeftInteractor;
    public GameObject RightInteractor;
    private bool triggerValue;

    void Start()
    {
        if (PlayerPrefs.GetString("Hand") == "L")
        {
            LeftInteractor.GetComponent<XRInteractorLineVisual>().enabled = true;
        }
        else
        {
            RightInteractor.GetComponent<XRInteractorLineVisual>().enabled = true;
        }
    }

    void Update()
    {
        var devices = new List<UnityEngine.XR.InputDevice>();

        if (PlayerPrefs.GetString("Hand") == "L")
        {
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, devices);
        }
        else
        {
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, devices);
        }

        if (devices.Count > 0)
        {
            var device = devices[0];

            // For left hand
            if (PlayerPrefs.GetString("Hand") == "L" && 
                LeftInteractor.GetComponent<XRRayInteractor>().TryGetCurrent3DRaycastHit(out RaycastHit hitL))
            {
                // Check if the hit object is a drowner and if the trigger is pressed.
                if (hitL.collider.transform.parent.gameObject.name.Contains("(Drowner)") &&
                    device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
                {
                    // Log the user swimmer selection event.
                    XRInputLogger.LogUserSwimmerSelection("User found drowner: " + hitL.collider.transform.parent.gameObject.name);
                }
            }

            // For right hand
            if (PlayerPrefs.GetString("Hand") == "R" && 
                RightInteractor.GetComponent<XRRayInteractor>().TryGetCurrent3DRaycastHit(out RaycastHit hitR))
            {
                if (hitR.collider.transform.parent.gameObject.name.Contains("(Drowner)") &&
                    device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
                {
                    XRInputLogger.LogUserSwimmerSelection("User found drowner: " + hitR.collider.transform.parent.gameObject.name);
                }
            }
        }
    }
}
