using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class EyeTrackingLogger : MonoBehaviour
{
    // List of detected eye‐tracking devices
    private List<InputDevice> eyeDevices = new List<InputDevice>();

    // Paths for folder & file
    private string folderPath;
    private string filePath;

    // StreamWriter for logging
    private StreamWriter writer;

    void Awake()
    {
        // 1) Prepare output folder in Assets/eye_tracking_logs
        folderPath = Path.Combine(Application.dataPath, "eye_tracking_logs");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // 2) Create a new CSV file with timestamp
        string runTimestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        filePath = Path.Combine(folderPath, "EyeLog_" + runTimestamp + ".csv");

        // 3) Open writer and write header
        writer = new StreamWriter(filePath, false);
        writer.WriteLine(
            "Timestamp," +
            "ValidGazeOrigin,GazeOrigin.x,GazeOrigin.y,GazeOrigin.z," +
            "ValidGazeRotation,GazeDir.x,GazeDir.y,GazeDir.z," +
            "ValidLeftEyePos,LeftEyePos.x,LeftEyePos.y,LeftEyePos.z," +
            "ValidLeftEyeRot,LeftEyeRot.x,LeftEyeRot.y,LeftEyeRot.z,LeftEyeRot.w," +
            "ValidRightEyePos,RightEyePos.x,RightEyePos.y,RightEyePos.z," +
            "ValidRightEyeRot,RightEyeRot.x,RightEyeRot.y,RightEyeRot.z,RightEyeRot.w," +
            "ValidFixationPoint,FixationPoint.x,FixationPoint.y,FixationPoint.z"
        );
        writer.Flush();

        // 4) Query for eye-tracking devices
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking, eyeDevices);
        if (eyeDevices.Count == 0)
            Debug.LogWarning("[EyeTrackingLogger] No eye-tracking devices found.");
    }

    void Update()
    {
        // If devices list went stale, refresh it
        if (eyeDevices.Count == 0)
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking, eyeDevices);

        var timestamp = DateTime.UtcNow;

        // Default values
        bool validGazeOrigin = false, validGazeRot = false;
        Vector3 gazeOrigin = Vector3.zero, gazeDir = Vector3.zero;
        Quaternion gazeRot = Quaternion.identity;

        bool validLeftEyePos = false, validLeftEyeRot = false;
        bool validRightEyePos = false, validRightEyeRot = false;
        bool validFixPt = false;
        Vector3 leftEyePos = Vector3.zero, rightEyePos = Vector3.zero, fixPt = Vector3.zero;
        Quaternion leftEyeRot = Quaternion.identity, rightEyeRot = Quaternion.identity;

        // Grab the first eye-tracking device if available
        if (eyeDevices.Count > 0)
        {
            var device = eyeDevices[0];
            if (device.isValid)
            {
                // Gaze origin & rotation → direction
                device.TryGetFeatureValue(EyeTrackingUsages.gazePosition, out gazeOrigin);
                device.TryGetFeatureValue(EyeTrackingUsages.gazeRotation, out gazeRot);
                if (validGazeRot)
                    gazeDir = gazeRot * Vector3.forward;

                // Detailed Eyes data (left/right eye poses, fixation)
                if (device.TryGetFeatureValue(CommonUsages.eyesData, out Eyes eyes))
                {
                    validLeftEyePos  = eyes.TryGetLeftEyePosition(out leftEyePos);
                    validLeftEyeRot  = eyes.TryGetLeftEyeRotation(out leftEyeRot);
                    validRightEyePos = eyes.TryGetRightEyePosition(out rightEyePos);
                    validRightEyeRot = eyes.TryGetRightEyeRotation(out rightEyeRot);
                    validFixPt       = eyes.TryGetFixationPoint(out fixPt);
                }
            }
        }

        // Format one CSV line
        string line = string.Format(
            "{0:O}," +                             // ISO timestamp
            "{1},{2:F4},{3:F4},{4:F4}," +         // gaze origin
            "{5},{6:F4},{7:F4},{8:F4}," +         // gaze rotation → direction
            "{9},{10:F4},{11:F4},{12:F4}," +      // left eye position
            "{13},{14:F4},{15:F4},{16:F4},{17:F4}," + // left eye rotation
            "{18},{19:F4},{20:F4},{21:F4}," +     // right eye position
            "{22},{23:F4},{24:F4},{25:F4},{26:F4}," + // right eye rotation
            "{27},{28:F4},{29:F4},{30:F4}",        // fixation point
            timestamp,
            validGazeOrigin?1:0, gazeOrigin.x, gazeOrigin.y, gazeOrigin.z,
            validGazeRot?1:0,    gazeDir.x,     gazeDir.y,     gazeDir.z,
            validLeftEyePos?1:0, leftEyePos.x,  leftEyePos.y,  leftEyePos.z,
            validLeftEyeRot?1:0, leftEyeRot.x,  leftEyeRot.y,  leftEyeRot.z, leftEyeRot.w,
            validRightEyePos?1:0,rightEyePos.x, rightEyePos.y, rightEyePos.z,
            validRightEyeRot?1:0,rightEyeRot.x, rightEyeRot.y, rightEyeRot.z, rightEyeRot.w,
            validFixPt?1:0,      fixPt.x,       fixPt.y,       fixPt.z
        );

        writer.WriteLine(line);
    }

    void OnDestroy()
    {
        if (writer != null)
        {
            writer.Flush();
            writer.Close();
            Debug.Log($"[EyeTrackingLogger] Log closed: {filePath}");
        }
    }
}
