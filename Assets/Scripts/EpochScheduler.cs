using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class EpochScheduler : MonoBehaviour
{
    public int[] EPOCH_SETS = {4, 20, 4, 20, 8, 12, 16, 20};
    [SerializeField]
    public float EPOCH_TIME = 10f;
    [SerializeField]
    public int EPOCH_AMOUNT = 10;
    [SerializeField]
    public GameObject swimmer;
    public int drownId = 0;
    private int swimmerId = 0;
    public List<GameObject> swimmerSet = new List<GameObject>();
    private float timeRemaining;
    private int epochsRemaining;
    private int epochSetIndex;
    private int epochSetRemaining;
    private List<int> spawnx = new List<int>();
    private List<int> spawnz = new List<int>();
    private int epochSize;
    [SerializeField]
    private TMP_Text outputText;
    private float fps;

    public int calculateWave(){
        //Simple calculation of the change in size of our swimmingpool set.
        return epochSize - swimmerSet.Count;
    }
private void pickDrowner()
{
    // Clear any existing (Drowner) from all swimmers
    for (int i = 0; i < swimmerSet.Count; i++)
    {
        if (swimmerSet[i].name.Contains("(Drowner)"))
        {
            // Log the removal using the epoch scheduler log method.
            Debug.Log($"EpochScheduler removed drowner: {swimmerSet[i].name}");
            XRInputLogger.LogEpochSchedulerOutput($"EpochScheduler removed drowner: {swimmerSet[i].name}");
            swimmerSet[i].name = swimmerSet[i].name.Replace("(Drowner)", "");
        }
    }
    
    // Randomly pick one
    drownId = UnityEngine.Random.Range(0, swimmerSet.Count);
    var drowner = swimmerSet[drownId];
    drowner.name += "(Drowner)";

    // Log the event using the epoch scheduler log method.
    XRInputLogger.LogEpochSchedulerOutput($"EpochScheduler picked drowner: {drowner.name}");
}
    private void spawnSwimmers(int count){
        //Add "count" number of swimmer objects in randomized fashion.
        //We should make better "randomization" to avoid overlapping.

        for (int i = 0; i < count; i++) {
            //This is just an eyeballed range.
            //Grab prefab "swimmer" and instantiate copies.
            var temp_x = spawnx[UnityEngine.Random.Range(0, spawnx.Count)];
            spawnz.Remove(temp_x);
            var temp_z = spawnz[UnityEngine.Random.Range(0, spawnz.Count)];
            spawnx.Remove(temp_z);

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
        //We also want to re-append their spawnpoint to our open coordinates.
        for (int i = 0; i < count; i++) {
            spawnx.Add((int)Math.Round(swimmerSet[0].GetComponent<Transform>().position.x));
            spawnz.Add((int)Math.Round(swimmerSet[0].GetComponent<Transform>().position.z));
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
            if(epochsRemaining <= 0) {
                //Decrement the epoch index set.
                epochSetRemaining--;
                //Calculate the index in the set of epochs.
                epochSetIndex = EPOCH_SETS.Length - epochSetRemaining;
                if(epochSetIndex >= EPOCH_SETS.Length){
                    //Stops the game when we run out of epochs.
                    //For in editor. Do note that it *will* execute all other code in this function when enabled.
                    UnityEditor.EditorApplication.isPlaying = false;
                    //For compiled code.
                    Application.Quit();
                }
                else{
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
                    //Pick a drowner from our new set.
                    pickDrowner();
                }
            }
            //Decrement the epoch.
            timeRemaining = EPOCH_TIME;
            epochsRemaining--;

        }
    }

    //This merely aligns our swimmers with the pool.
    //I'm not a massive fan of this solution but it works for the time being.
    private void formatCoordinates(){
        //Sort through both lists and align.
        for (int i = 0; i < spawnz.Count; i++){
            spawnz[i]=(spawnz[i]*2)-215;
        }

        for (int i = 0; i < spawnx.Count; i++){
            spawnx[i]=(spawnx[i]*2)-30;
        }
    }

    void Start(){
        //Get our coordinates for the swimmers
        spawnz = Enumerable.Range(0, 90).ToList();
        spawnx = Enumerable.Range(0, 35).ToList();
        formatCoordinates();

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
        //Pick a drowner.
        pickDrowner();
        //Repeatedly update the debug screen.
        InvokeRepeating("writeToDebug", 1f, 0.1f);
    }

    void Update(){
        //Run the epoch scheduler.
        runEpoch();
    }
}