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
    [SerializeField]
    public GameObject swimmer;
    private int drownId = 0;
    private int swimmerId = 0;
    public List<GameObject> swimmerSet = new List<GameObject>();
    private float timeRemaining;
    private int epochsRemaining;
    private int epochSetIndex;
    private int epochSetRemaining;
    private int epochSize;
    [SerializeField]
    private TMP_Text outputText;
    private float fps;

    public int calculateWave(){
        //Simple calculation of the change in size of our swimmingpool set.
        return epochSize - swimmerSet.Count;
    }
    private void spawnSwimmers(int count){
        //Add "count" number of swimmer objects in randomized fashion.
        //We should make better "randomization" to avoid overlapping.
        for (int i = 0; i < count; i++) {
            //This is just an eyeballed range.
            var temp_z = UnityEngine.Random.Range(-40.0f, -190.0f);
            var temp_x = UnityEngine.Random.Range(31.0f, -40.0f);
            //Grab prefab "swimmer" and instantiate copies.
            var temp = Instantiate(swimmer);
            temp.name  = "Swimmer" + swimmerId.ToString();
            //Name the collider for raycasting, rember to patch if we add extra game objects to the human male prefab.
            temp.transform.GetChild(0).gameObject.name = "Collider" + swimmerId.ToString();
            swimmerId++;
            temp.GetComponent<Transform>().position = new Vector3(temp_x,0,temp_z);
            //Add swimmer to our list, this isn't for rendering or the engine, but to track who is on screen for data output as well as culling/addition.
            swimmerSet.Add(temp);
        }
    }
    private void cullSwimmers(int count){
        //Remove "count" number of swimmer objects.
        for (int i = 0; i < count; i++) {
            Destroy(swimmerSet[0]);
            swimmerSet.Remove(swimmerSet[0]);
        }
    }

    private void writeToDebug(){
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
                //Calculate the index in the set of epochs.
                epochSetIndex = EPOCH_SETS.Length - epochSetRemaining;
                epochSize = EPOCH_SETS[epochSetIndex];
                epochsRemaining = EPOCH_AMOUNT;
                //Calculate if we must add or remove swimmers.
                //*Note that this feature is purely here for testing the epoch structure, and will be changed/improved completely.
                var temp = calculateWave();
                if(temp > 0){
                    spawnSwimmers(temp);
                }
                else if(temp < 0){
                    cullSwimmers(temp*-1);
                }
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
        //Spawn in the people.
        spawnSwimmers(EPOCH_SETS[0]);
        //Repeatedly update the debug screen.
        InvokeRepeating("writeToDebug", 1f, 0.1f);
    }

    void Update(){
        //Run the epoch scheduler.
        runEpoch();
    }
}