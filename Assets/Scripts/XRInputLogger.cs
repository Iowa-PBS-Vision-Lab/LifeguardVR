using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System.IO;
using System;

public class XRInputLogger : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void CreateLogger()
    {
        GameObject loggerObject = new GameObject("XRInputLogger");
        XRInputLogger logger = loggerObject.AddComponent<XRInputLogger>();
        DontDestroyOnLoad(loggerObject);
        Debug.Log("XRInputLogger Created");
        logger.Start();
    }
    private InputDevice leftController;
    private InputDevice rightController;
    private InputDevice leftHand;
    private InputDevice rightHand;

    private Dictionary<string, bool> buttonStates = new Dictionary<string, bool>();
    private string folderPath;
    private string filePath;

    void Start()
    {
        Debug.Log("XRInputLogger Started");
        InitializeDevices();

        // Define the folder and file path inside persistent data path
        folderPath = Path.Combine(Application.dataPath, "loggingData");
        filePath = Path.Combine(folderPath, "XRInputLog.csv");
        Debug.Log($"Logging to: {filePath}");

        // Create the folder if it does not exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"Created folder: {folderPath}");
        }

        // If the file does not exist, write headers
        if (!File.Exists(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("Timestamp,Device,Input,Value");
            }
            Debug.Log($"Created log file: {filePath}");
        }
    }

    void Update()
    {
        if (!leftController.isValid || !rightController.isValid || !leftHand.isValid || !rightHand.isValid)
        {
            InitializeDevices();
        }

        bool inputDetected = false;
        inputDetected |= LogControllerInput(leftController, "Left Controller");
        inputDetected |= LogControllerInput(rightController, "Right Controller");
        inputDetected |= LogHandTracking(leftHand, "Left Hand");
        inputDetected |= LogHandTracking(rightHand, "Right Hand");

        if (inputDetected)
        {
            Debug.Log(Application.dataPath);
        }
    }

    void InitializeDevices()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);

        foreach (var device in devices)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                if (device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
                    leftController = device;
                else if (device.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
                    leftHand = device;
            }
            else if (device.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                if (device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
                    rightController = device;
                else if (device.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
                    rightHand = device;
            }
        }
    }

    bool LogControllerInput(InputDevice device, string name)
    {
        if (!device.isValid) return false;
        bool inputDetected = false;

        inputDetected |= CheckButton(device, CommonUsages.primaryButton, $"{name}: Primary Button");
        inputDetected |= CheckButton(device, CommonUsages.secondaryButton, $"{name}: Secondary Button");
        inputDetected |= CheckButton(device, CommonUsages.menuButton, $"{name}: Menu Button");

        inputDetected |= CheckAxis(device, CommonUsages.trigger, $"{name}: Trigger");
        inputDetected |= CheckAxis(device, CommonUsages.grip, $"{name}: Grip");

        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbstick) && thumbstick.magnitude > 0.1f)
        {
            LogToCSV(name, "Thumbstick", thumbstick.ToString());
            Debug.Log($"{name}: Thumbstick Moved {thumbstick}");
            inputDetected = true;
        }

        return inputDetected;
    }

    bool LogHandTracking(InputDevice device, string name)
    {
        if (!device.isValid) return false;
        bool inputDetected = false;

        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
        {
            LogToCSV(name, "Position", position.ToString());
            Debug.Log($"{name} Position: {position}");
            inputDetected = true;
        }

        if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
        {
            LogToCSV(name, "Rotation", rotation.eulerAngles.ToString());
            Debug.Log($"{name} Rotation: {rotation.eulerAngles}");
            inputDetected = true;
        }

        inputDetected |= CheckAxis(device, CommonUsages.grip, $"{name} Grip");
        inputDetected |= CheckAxis(device, CommonUsages.trigger, $"{name} Index Finger Pinch");

        return inputDetected;
    }

    bool CheckButton(InputDevice device, InputFeatureUsage<bool> button, string label)
    {
        if (device.TryGetFeatureValue(button, out bool pressed))
        {
            if (buttonStates.ContainsKey(label) && buttonStates[label] == pressed) return false;

            buttonStates[label] = pressed;
            if (pressed)
            {
                LogToCSV(label, "Pressed", "1");
                Debug.Log($"{label}: Pressed");
                return true;
            }
        }
        return false;
    }

    bool CheckAxis(InputDevice device, InputFeatureUsage<float> axis, string label)
    {
        if (device.TryGetFeatureValue(axis, out float value) && value > 0.1f)
        {
            LogToCSV(label, "Value", value.ToString("F2"));
            Debug.Log($"{label}: {value}");
            return true;
        }
        return false;
    }

    void LogToCSV(string device, string input, string value)
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine($"{DateTime.UtcNow},{device},{input},{value}");
        }
    }
}
