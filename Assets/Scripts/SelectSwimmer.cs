using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                }
            }
        }
        //Double check that we are interacting with a collider of some sort.
        
    }
}