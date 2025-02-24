using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class XRInputLogger : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateLogger()
    {
        if (!FindObjectOfType<XRInputLogger>())
        {
            var loggerObj = new GameObject("XRInputLogger");
            loggerObj.AddComponent<XRInputLogger>();
            Debug.Log("[XRInputLogger] Automatically created at runtime.");
        }
    }

    private static XRInputLogger _instance;
    public static XRInputLogger Instance => _instance;

    private List<string> eventBuffer = new List<string>();

    private string folderPath;
    private string filePath;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        folderPath = Path.Combine(Application.dataPath, "loggingData");
        filePath = Path.Combine(folderPath, "XRInputLog.csv");
        Debug.Log("[XRInputLogger] Awake - Will buffer events until OnDestroy.");
    }

    public static void LogCustomEvent(string message)
    {
        if (Instance == null)
        {
            Debug.LogWarning("[XRInputLogger] No active XRInputLogger instance to record event!");
            return;
        }
        Instance.BufferEvent(message);
    }

    private void BufferEvent(string message)
    {
        // Example format: "Timestamp,EventType,Data"
        string line = $"{DateTime.UtcNow:O},CustomEvent,{message}";
        eventBuffer.Add(line);
        Debug.Log($"[XRInputLogger] Buffered event: {line}");
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            FlushLogsToCSV();
        }
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

        bool fileExists = File.Exists(filePath);

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            // Write headers if the file doesn't exist yet
            if (!fileExists)
            {
                writer.WriteLine("Timestamp,EventType,Message");
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
