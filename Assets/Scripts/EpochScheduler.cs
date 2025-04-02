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
        // Simple calculation of the change in size of our swimming pool set.
        return epochSize - swimmerSet.Count;
    }

    private void pickDrowner(){
        // Clear any existing (Drowner) from all swimmers.
        for (int i = 0; i < swimmerSet.Count; i++){
            if(swimmerSet[i].name.Contains("(Drowner)")){
                swimmerSet[i].name = swimmerSet[i].name.Replace("(Drowner)", "");
            }
        }
        // Randomly pick one.
        drownId = UnityEngine.Random.Range(0, swimmerSet.Count);
        swimmerSet[drownId].name += "(Drowner)";
        // Log the event.
        XRInputLogger.LogCustomEvent($"EpochScheduler picked drowner: {swimmerSet[drownId].name}");
    }

    private void spawnSwimmers(int count){
        // Add "count" number of swimmer objects in a randomized fashion.
        for (int i = 0; i < count; i++) {
            // This is just an eyeballed range.
            var temp_x = spawnx[UnityEngine.Random.Range(0, spawnx.Count)];
            spawnz.Remove(temp_x);
            var temp_z = spawnz[UnityEngine.Random.Range(0, spawnz.Count)];
            spawnx.Remove(temp_z);

            var temp = Instantiate(swimmer);
            temp.name  = "Swimmer" + swimmerId.ToString();
            // Name the collider for raycasting.
            temp.transform.GetChild(0).gameObject.name = "Collider" + swimmerId.ToString();
            swimmerId++;
            temp.GetComponent<Transform>().position = new Vector3(temp_x, 0, temp_z);
            swimmerSet.Add(temp);
            // Log the spawn event.
            XRInputLogger.LogCustomEvent($"EpochScheduler spawned: {temp.name}");
        }
    }

    private void cullSwimmers(int count){
        // Remove "count" number of swimmer objects.
        // Also re-append their spawnpoint to our open coordinates.
        for (int i = 0; i < count; i++) {
            // Log the culling event.
            XRInputLogger.LogCustomEvent($"EpochScheduler culled: {swimmerSet[0].name}");
            spawnx.Add((int)Math.Round(swimmerSet[0].GetComponent<Transform>().position.x));
            spawnz.Add((int)Math.Round(swimmerSet[0].GetComponent<Transform>().position.z));
            Destroy(swimmerSet[0]);
            swimmerSet.RemoveAt(0);
        }
    }

    private void writeToDebug(){
        // Calculate fps.
        fps = (int)(1 / Time.deltaTime);
        // Set values to screen.
        outputText.text = string.Format("Epochs Left: {0}\nTime Left: {1}\nFPS: {2}\nSet Index: {3}",
            epochsRemaining, (int)timeRemaining, fps, epochSetIndex);
    }

    private void runEpoch(){
        // Count down the remaining time.
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0) {
            if (epochsRemaining <= 0) {
                // Decrement the epoch index set.
                epochSetRemaining--;
                // Calculate the index in the set of epochs.
                epochSetIndex = EPOCH_SETS.Length - epochSetRemaining;
                if (epochSetIndex >= EPOCH_SETS.Length) {
                    // Stop the game when we run out of epochs.
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
                else {
                    // Reset swimmer ID counter at start of new epoch set
                    swimmerId = 0;

                    epochSize = EPOCH_SETS[epochSetIndex];
                    epochsRemaining = EPOCH_AMOUNT;
                    // Calculate if we must add or remove swimmers.
                    int temp = calculateWave();
                    if (temp > 0) {
                        spawnSwimmers(temp);
                    }
                    else if (temp < 0) {
                        cullSwimmers(-temp);
                    }
                    // Pick a drowner from our new set.
                    pickDrowner();
                }
            }
            // Decrement the epoch.
            timeRemaining = EPOCH_TIME;
            epochsRemaining--;
        }
    }

    private void formatCoordinates(){
        // Align spawnz.
        for (int i = 0; i < spawnz.Count; i++){
            spawnz[i] = (spawnz[i] * 2) - 215;
        }
        // Align spawnx.
        for (int i = 0; i < spawnx.Count; i++){
            spawnx[i] = (spawnx[i] * 2) - 30;
        }
    }

    void Start(){
        // Get coordinates for the swimmers.
        spawnz = Enumerable.Range(0, 90).ToList();
        spawnx = Enumerable.Range(0, 35).ToList();
        formatCoordinates();

        // Write to the debug screen.
        writeToDebug();
        // Initialize values.
        epochSetRemaining = EPOCH_SETS.Length;
        epochSetIndex = 0;
        epochSize = EPOCH_SETS[epochSetIndex];
        timeRemaining = EPOCH_TIME;
        epochsRemaining = EPOCH_AMOUNT;
        // Spawn the swimmers.
        spawnSwimmers(EPOCH_SETS[0]);
        // Pick a drowner.
        pickDrowner();
        // Repeatedly update the debug screen.
        InvokeRepeating("writeToDebug", 1f, 0.1f);
    }

    void Update(){
        // Run the epoch scheduler.
        runEpoch();
    }
}