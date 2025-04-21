using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class SelectSwimmer : MonoBehaviour
{
    // The GameObjects representing the left and right interactors.
    public GameObject LeftInteractor;
    public GameObject RightInteractor;
    private bool triggerValue;

    void Start(){
        if(PlayerPrefs.GetString("Hand") == "L"){
            LeftInteractor.GetComponent<XRInteractorLineVisual>().enabled = true;
        }
        else{
            RightInteractor.GetComponent<XRInteractorLineVisual>().enabled = true;
        }
    }

    void Update()
    {
        var devices = new List<UnityEngine.XR.InputDevice>();

        if(PlayerPrefs.GetString("Hand") == "L"){
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, devices);
        }
        else{
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, devices);
        }

        if(devices.Count > 0){
            var device = devices[0];

            // For Left Hand:
            if(PlayerPrefs.GetString("Hand") == "L" && 
                LeftInteractor.GetComponent<XRRayInteractor>().TryGetCurrent3DRaycastHit(out RaycastHit hitL))
            {
                // If the hit object's parent's name contains "(Drowner)" and the trigger is pressed.
                if (hitL.collider.transform.parent.gameObject.name.Contains("(Drowner)") && 
                    device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
                {
                    // Debug.Log("The player has found the drowner!");
                    // // Log the event using the logging system.
                    // XRInputLogger.LogCustomEvent($"User selected {hitL.collider.transform.parent.gameObject.name}");
                }
            }

            // For Right Hand:
            if(PlayerPrefs.GetString("Hand") == "R" && 
                RightInteractor.GetComponent<XRRayInteractor>().TryGetCurrent3DRaycastHit(out RaycastHit hitR))
            {
                if (hitR.collider.transform.parent.gameObject.name.Contains("(Drowner)") && 
                    device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
                {
                    // Debug.Log("The player has found the drowner!");
                    // XRInputLogger.LogCustomEvent($"User selected {hitR.collider.transform.parent.gameObject.name}");
                }
            }
        }
        // Double check that we are interacting with a collider of some sort.
    }
}
