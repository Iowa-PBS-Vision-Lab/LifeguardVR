using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EpochScheduler : MonoBehaviour
{
    [SerializeField]
    public float EPOCH_TIME = 10f;
    [SerializeField]
    public int EPOCH_AMOUNT = 10;
    private float timeRemaining;
    private int epochsRemaining;
    [SerializeField]
    private TMP_Text outputText;
    private float fps;
    public void writeToDebug(){
        fps = (int)(1/Time.deltaTime);
        outputText.text  = string.Format("Epochs Left: {0}\nTime Left: {1}\nFPS: {2}", epochsRemaining, (int)timeRemaining, fps);
    }
    private void runEpoch(){
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0) { 
            if(epochsRemaining <= 0) {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            timeRemaining = EPOCH_TIME;
            epochsRemaining--;
        }
    }
    void Start(){
        writeToDebug();
        timeRemaining = EPOCH_TIME;
        epochsRemaining = EPOCH_AMOUNT;
        InvokeRepeating("writeToDebug", 1f, 0.1f);
    }

    // Update is called once per frame
    void Update(){
        runEpoch();
    }
}
