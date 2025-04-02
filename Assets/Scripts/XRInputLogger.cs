using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class XRInputLogger : MonoBehaviour
{
    // Buffer for log lines before flushing to CSV.
    private List<string> eventBuffer = new List<string>();

    // Subject details—set these via Inspector or via SubjectSubmit.
    public string subjectID = "DefaultSubject";
    public string age = "N/A";
    public string gender = "N/A";
    public string handedness = "N/A";

    // Event fields that only populate on the frame the event occurs.
    private string epochSchedulerOutput = "";
    private string controllerInput = "";
    private string userSwimmerSelection = "";

    // Reference to the head (camera) transform. If not assigned, defaults to this.transform.
    public Transform headTransform;

    private string folderPath;
    private string filePath;

    // Maximum number of swimmers to log (determined externally via EpochScheduler.EPOCH_SETS).
    private int maxSwimmers = 0;

    private void Awake()
    {
        // If headTransform is not set, default to this.transform.
        if (headTransform == null)
        {
            headTransform = this.transform;
            Debug.Log("[XRInputLogger] headTransform not set in Inspector. Defaulting to this.transform: " + headTransform.name);
        }

        // Load subject details from SubjectSubmit if available.
        SubjectSubmit subjectSubmit = FindObjectOfType<SubjectSubmit>();
        subjectID = subjectSubmit?.idInfo?.text ?? "";
        age = subjectSubmit?.ageInfo?.text ?? "";
        gender = subjectSubmit?.genderInfo?.text ?? "";
        handedness = subjectSubmit?.handInfo?.text ?? "";

        // Define logging path.
        folderPath = Path.Combine(Application.dataPath, "loggingData");
        string runTimestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        filePath = Path.Combine(folderPath, "InputLog_" + runTimestamp + ".csv");

        // Determine maxSwimmers from an external EpochScheduler.
        var epochScheduler = FindObjectOfType<EpochScheduler>();
        if (epochScheduler != null && epochScheduler.EPOCH_SETS != null)
        {
            foreach (int val in epochScheduler.EPOCH_SETS)
            {
                if (val > maxSwimmers)
                    maxSwimmers = val;
            }
        }
        else
        {
            // If no EpochScheduler is found, default to the current number of swimmers.
            GameObject[] swimmersFallback = GameObject.FindGameObjectsWithTag("Swimmer");
            maxSwimmers = swimmersFallback.Length;
            Debug.LogWarning("[XRInputLogger] EpochScheduler not found. Setting maxSwimmers to current swimmers count: " + maxSwimmers);
        }

        Debug.Log("[XRInputLogger] Awake - New log file created: " + filePath);
    }

    // Use Update to capture positions and rotations.
    private void Update()
    {
        if (headTransform == null)
            return;

        // Capture head pose.
        Vector3 pos = headTransform.position;
        Vector3 rot = headTransform.eulerAngles;
        float headRoll  = rot.z;
        float headPitch = rot.x;
        float headYaw   = rot.y;

        // Build the base CSV line:
        // Timestamp,SubjectID,Age,Gender,Handedness,EpochSchedulerOutput,ControllerInput,UserSwimmerSelection,
        // HeadX,HeadY,HeadZ,HeadRoll,HeadPitch,HeadYaw
        string line = string.Format("{0:O},{1},{2},{3},{4},{5},{6},{7},{8:F4},{9:F4},{10:F4},{11:F4},{12:F4},{13:F4}",
            DateTime.UtcNow,
            subjectID,
            age,
            gender,
            handedness,
            epochSchedulerOutput,
            controllerInput,
            userSwimmerSelection,
            pos.x, pos.y, pos.z,
            headRoll, headPitch, headYaw
        );

        // Retrieve swimmers from the EpochScheduler's swimmerSet, if available.
        EpochScheduler epochScheduler = FindObjectOfType<EpochScheduler>();
        List<GameObject> swimmers = (epochScheduler != null) ? epochScheduler.swimmerSet : new List<GameObject>();

        // Append swimmer positions and rotations for up to maxSwimmers.
        // For each swimmer, we use the parent's x and z, but for the y and rotation we get data from the "male" child.
        string swimmerData = "";
        for (int i = 0; i < maxSwimmers; i++)
        {
            if (i < swimmers.Count && swimmers[i] != null)
            {
                // Get x and z from the parent.
                Vector3 swPos = swimmers[i].transform.position;

                // Search for the "male" child (recursively) to get the animated y value and its rotation.
                Transform maleTransform = null;
                foreach (Transform child in swimmers[i].GetComponentsInChildren<Transform>())
                {
                    if (child.name.Equals("male"))
                    {
                        maleTransform = child;
                        break;
                    }
                }

                if (maleTransform != null)
                {
                    swPos.y = maleTransform.position.y;
                    // Get rotation from the "male" child.
                    Vector3 maleRot = maleTransform.rotation.eulerAngles;
                    swimmerData += string.Format(",{0:F4},{1:F4},{2:F4},{3:F4},{4:F4},{5:F4}", 
                        swPos.x, swPos.y, swPos.z, maleRot.x, maleRot.y, maleRot.z);
                }
                else
                {
                    // If "male" child isn't found, log parent's position and rotation.
                    Vector3 swRot = swimmers[i].transform.rotation.eulerAngles;
                    swimmerData += string.Format(",{0:F4},{1:F4},{2:F4},{3:F4},{4:F4},{5:F4}", 
                        swPos.x, swPos.y, swPos.z, swRot.x, swRot.y, swRot.z);
                }
            }
            else
            {
                swimmerData += ",,,,,,";
            }
        }

        line += swimmerData;
        eventBuffer.Add(line);

        // Reset event fields so they only appear on the frame they occur.
        epochSchedulerOutput = "";
        controllerInput = "";
        userSwimmerSelection = "";
    }

    // Static logging methods.
    public static void LogEpochSchedulerOutput(string output)
    {
        XRInputLogger logger = FindObjectOfType<XRInputLogger>();
        if (logger == null)
        {
            Debug.LogWarning("[XRInputLogger] No active instance to record epoch scheduler output!");
            return;
        }
        logger.epochSchedulerOutput = output;
    }

    public static void LogControllerInput(string input)
    {
        XRInputLogger logger = FindObjectOfType<XRInputLogger>();
        if (logger == null)
        {
            Debug.LogWarning("[XRInputLogger] No active instance to record controller input!");
            return;
        }
        logger.controllerInput = input;
    }

    public static void LogUserSwimmerSelection(string selection)
    {
        XRInputLogger logger = FindObjectOfType<XRInputLogger>();
        if (logger == null)
        {
            Debug.LogWarning("[XRInputLogger] No active instance to record user swimmer selection!");
            return;
        }
        logger.userSwimmerSelection = selection;
    }

    public static void LogCustomEvent(string message)
    {
        XRInputLogger logger = FindObjectOfType<XRInputLogger>();
        if (logger == null)
        {
            Debug.LogWarning("[XRInputLogger] No active instance to record event!");
            return;
        }
        logger.BufferCustomEvent(message);
    }

    private void BufferCustomEvent(string message)
    {
        string line = string.Format("{0:O},{1},{2},{3},{4},,,,," +
            ",,,,", DateTime.UtcNow, subjectID, age, gender, handedness) + message;
        eventBuffer.Add(line);
        Debug.Log($"[XRInputLogger] Buffered custom event: {line}");
    }

    private void OnDestroy()
    {
        FlushLogsToCSV();
    }

    private void FlushLogsToCSV()
    {
        if (eventBuffer.Count == 0)
        {
            Debug.Log("[XRInputLogger] No events to flush. Skipping write.");
            return;
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"[XRInputLogger] Created folder: {folderPath}");
        }

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            // Build header.
            string header = "Timestamp,SubjectID,Age,Gender,Handedness,EpochSchedulerOutput,ControllerInput,UserSwimmerSelection,HeadX,HeadY,HeadZ,HeadRoll,HeadPitch,HeadYaw";
            // For each swimmer, add 6 columns: x, y, z, rotX, rotY, rotZ.
            for (int i = 1; i <= maxSwimmers; i++)
            {
                header += string.Format(",swimmer{0}x,swimmer{0}y,swimmer{0}z,swimmer{0}rotX,swimmer{0}rotY,swimmer{0}rotZ", i);
            }
            writer.WriteLine(header);

            foreach (string line in eventBuffer)
            {
                writer.WriteLine(line);
            }
        }

        Debug.Log($"[XRInputLogger] Flushed {eventBuffer.Count} events to CSV: {filePath}");
        eventBuffer.Clear();
    }
}
