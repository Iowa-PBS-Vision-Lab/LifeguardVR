using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class XRInputLogger : MonoBehaviour
{
    private InputDevice leftController;
    private InputDevice rightController;
    private InputDevice leftHand;
    private InputDevice rightHand;

    private Dictionary<string, bool> buttonStates = new Dictionary<string, bool>();

    void Start()
    {
        InitializeDevices();
    }

    void Update()
    {
        // Reinitialize devices if lost
        if (!leftController.isValid || !rightController.isValid || !leftHand.isValid || !rightHand.isValid)
        {
            InitializeDevices();
        }

        // Log controller inputs and hand tracking
        bool inputDetected = false;
        inputDetected |= LogControllerInput(leftController, "Left Controller");
        inputDetected |= LogControllerInput(rightController, "Right Controller");
        inputDetected |= LogHandTracking(leftHand, "Left Hand");
        inputDetected |= LogHandTracking(rightHand, "Right Hand");

        // Log only if an input event occurred
        if (inputDetected)
        {
            Debug.Log($"--- XR Input Event ---");
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

        // Check buttons
        inputDetected |= CheckButton(device, CommonUsages.primaryButton, $"{name}: Primary Button");
        inputDetected |= CheckButton(device, CommonUsages.secondaryButton, $"{name}: Secondary Button");
        inputDetected |= CheckButton(device, CommonUsages.menuButton, $"{name}: Menu Button");

        // Check trigger and grip
        inputDetected |= CheckAxis(device, CommonUsages.trigger, $"{name}: Trigger");
        inputDetected |= CheckAxis(device, CommonUsages.grip, $"{name}: Grip");

        // Check thumbstick movement
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbstick) && thumbstick.magnitude > 0.1f)
        {
            Debug.Log($"{name}: Thumbstick Moved {thumbstick}");
            inputDetected = true;
        }

        return inputDetected;
    }

    bool LogHandTracking(InputDevice device, string name)
    {
        if (!device.isValid) return false;
        bool inputDetected = false;

        // Log hand position
        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
        {
            Debug.Log($"{name} Position: {position}");
            inputDetected = true;
        }

        // Log hand rotation
        if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
        {
            Debug.Log($"{name} Rotation: {rotation.eulerAngles}");
            inputDetected = true;
        }

        // Log grip (fist)
        inputDetected |= CheckAxis(device, CommonUsages.grip, $"{name} Grip");

        // Log index finger pinch
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
            Debug.Log($"{label}: {value}");
            return true;
        }
        return false;
    }
}