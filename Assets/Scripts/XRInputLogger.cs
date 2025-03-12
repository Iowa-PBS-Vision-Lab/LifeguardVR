using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class XRInputLogger : MonoBehaviour
{
    // Remove auto-creation; you now attach this script manually to your main camera.

    // Buffer for log lines before flushing to CSV.
    private List<string> eventBuffer = new List<string>();

    // Subject details—set these via inspector or via PlayerPrefs.
    public string subjectID = "DefaultSubject";
    public string age = "N/A";
    public string gender = "N/A";
    public string handedness = "N/A";

    // Event fields that only populate on the frame the event occurs.
    private string epochSchedulerOutput = "";
    private string controllerInput = "";
    private string userSwimmerSelection = "";

    // Reference to the head (camera) transform. If not assigned, will default to this.transform.
    public Transform headTransform;

    private string folderPath;
    private string filePath;

    private void Awake()
    {
        // Since you're attaching this manually to the main camera, use its transform if headTransform is not set.
        if (headTransform == null)
        {
            headTransform = this.transform;
            Debug.Log("[XRInputLogger] headTransform not set in Inspector. Defaulting to this.transform: " + headTransform.name);
        }

        // Load subject details from PlayerPrefs if they were set.
        subjectID = PlayerPrefs.GetString("subjectID", subjectID);
        age = PlayerPrefs.GetString("age", age);
        gender = PlayerPrefs.GetString("gender", gender);
        handedness = PlayerPrefs.GetString("handedness", handedness);

        // Define logging path.
        folderPath = Path.Combine(Application.dataPath, "loggingData");

        // Create a new log file name for each run.
        string runTimestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        filePath = Path.Combine(folderPath, "InputLog_" + runTimestamp + ".csv");

        Debug.Log("[XRInputLogger] Awake - New log file created: " + filePath);
    }

    private void Update()
    {
        if (headTransform == null)
        {
            // Should not happen because we defaulted in Awake, but check anyway.
            return;
        }

        // Capture head pose.
        Vector3 pos = headTransform.position;
        Vector3 rot = headTransform.eulerAngles;
        // Unity's convention:
        //   rot.x = Pitch, rot.y = Yaw, rot.z = Roll.
        float headRoll  = rot.z;
        float headPitch = rot.x;
        float headYaw   = rot.y;

        // CSV format:
        // time, subjectID, age, gender, handedness, epoch scheduler output,
        // controller input, user swimmer selection, headX, headY, headZ, headRoll, headPitch, headYaw
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

        eventBuffer.Add(line);
        // Debug.Log($"[XRInputLogger] Buffered frame log: {line}");

        // Reset event fields so they only appear on the frame they occur.
        epochSchedulerOutput = "";
        controllerInput = "";
        userSwimmerSelection = "";
    }

    // Static methods for logging events when they occur.
    public static void LogEpochSchedulerOutput(string output)
    {
        if (FindObjectOfType<XRInputLogger>() == null)
        {
            Debug.LogWarning("[XRInputLogger] No active instance to record epoch scheduler output!");
            return;
        }
        FindObjectOfType<XRInputLogger>().epochSchedulerOutput = output;
    }

    public static void LogControllerInput(string input)
    {
        if (FindObjectOfType<XRInputLogger>() == null)
        {
            Debug.LogWarning("[XRInputLogger] No active instance to record controller input!");
            return;
        }
        FindObjectOfType<XRInputLogger>().controllerInput = input;
    }

    public static void LogUserSwimmerSelection(string selection)
    {
        if (FindObjectOfType<XRInputLogger>() == null)
        {
            Debug.LogWarning("[XRInputLogger] No active instance to record user swimmer selection!");
            return;
        }
        FindObjectOfType<XRInputLogger>().userSwimmerSelection = selection;
    }

    // Optional: still available for other custom events.
    public static void LogCustomEvent(string message)
    {
        if (FindObjectOfType<XRInputLogger>() == null)
        {
            Debug.LogWarning("[XRInputLogger] No active instance to record event!");
            return;
        }
        FindObjectOfType<XRInputLogger>().BufferCustomEvent(message);
    }

    private void BufferCustomEvent(string message)
    {
        // This logs a custom event. For consistency, event-specific columns remain empty.
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
            // Write header if the file doesn't exist.
            if (!File.Exists(filePath))
            {
                writer.WriteLine("Timestamp,SubjectID,Age,Gender,Handedness,EpochSchedulerOutput,ControllerInput,UserSwimmerSelection,HeadX,HeadY,HeadZ,HeadRoll,HeadPitch,HeadYaw");
            }
            foreach (string line in eventBuffer)
            {
                writer.WriteLine(line);
            }
        }

        Debug.Log($"[XRInputLogger] Flushed {eventBuffer.Count} events to CSV: {filePath}");
        eventBuffer.Clear();
    }
}