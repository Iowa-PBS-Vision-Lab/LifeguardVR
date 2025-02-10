using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
public class SelectSwimmer : MonoBehaviour
{
    //The raycast of the hand we want to use...
    public XRRayInteractor rayInteractor;
    // Update is called once per frame
    public void HoverOverSwimmer()
    {
        //Double check that we are interacting with a collider of some sort.
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            //Print the name of the interactor to the console.
            Debug.Log("The player has pointed at " + hit.collider.gameObject.name + "!");
        }
    }
}