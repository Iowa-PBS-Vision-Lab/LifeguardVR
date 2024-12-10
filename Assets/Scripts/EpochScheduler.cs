using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EpochScheduler : MonoBehaviour
{
    [SerializeField]
    public int[] EPOCH_SETS = {8, 12, 16, 20, 8, 12, 16, 20};
    [SerializeField]
    public float EPOCH_TIME = 10f;
    [SerializeField]
    public int EPOCH_AMOUNT = 10;
    private float timeRemaining;
    private int epochsRemaining;
    private int epochSetIndex;
    private int epochSetRemaining;
    private int epochSize;
    [SerializeField]
    private TMP_Text outputText;
    private float fps;

    public void writeToDebug(){
        //Calculate fps.
        fps = (int)(1/Time.deltaTime);
        //Set values to screen.
        outputText.text  = string.Format("Epochs Left: {0}\nTime Left: {1}\nFPS: {2}\nSet Index: {3}", epochsRemaining, (int)timeRemaining, fps, epochSetIndex);
    }
    private void runEpoch(){
        //Counts down the remaining time left.
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0) { 
            if(epochSetIndex >= EPOCH_SETS.Length){
                //stops the game when we run out of epochs.
                UnityEditor.EditorApplication.isPlaying = false;
            }
            if(epochsRemaining <= 0) {
                //Decrement the epoch index set.
                epochSetRemaining--;
                epochSetIndex = EPOCH_SETS.Length - epochSetRemaining;
                epochSize = EPOCH_SETS[epochSetIndex];
                epochsRemaining = EPOCH_AMOUNT;
            }
            //Decrement the epoch.
            timeRemaining = EPOCH_TIME;
            epochsRemaining--;
        }
    }

    void Start(){
        //Write to the debug screne.
        writeToDebug();
        //Initialize our values.
        epochSetRemaining = EPOCH_SETS.Length;
        epochSetIndex = 0;
        epochSize = EPOCH_SETS[epochSetIndex];
        timeRemaining = EPOCH_TIME;
        epochsRemaining = EPOCH_AMOUNT;
        //Repeatedly update the debug screen.
        InvokeRepeating("writeToDebug", 1f, 0.1f);
    }

    void Update(){
        //Run the epoch scheduler.
        runEpoch();
    }
}