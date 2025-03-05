using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
public class SelectSwimmer : MonoBehaviour
{
    //The raycast of the hand we want to use...
    public GameObject LeftInteractor;
    public GameObject RightInteractor;
    private bool triggerValue;
    // Update is called once per frame
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
            if (LeftInteractor.GetComponent<XRRayInteractor>().TryGetCurrent3DRaycastHit(out RaycastHit hitL) && PlayerPrefs.GetString("Hand") == "L") {
                //Print the name of the interactor to the console.
                if (hitL.collider.transform.parent.gameObject.name.Contains("(Drowner)") && device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue){
                    Debug.Log("The player has found the drowner!");
                }
            }

            if(RightInteractor.GetComponent<XRRayInteractor>().TryGetCurrent3DRaycastHit(out RaycastHit hitR) && PlayerPrefs.GetString("Hand") == "R"){
                if (hitR.collider.transform.parent.gameObject.name.Contains("(Drowner)") && device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue){
                        Debug.Log("The player has found the drowner!");
                }
            }

        }
        //Double check that we are interacting with a collider of some sort.
        
    }
}