using UnityEngine;
public class BobbingEffect : MonoBehaviour
{
    [Range(0.1f, 10.0f)] public float bobPosSpeed = 1.0f;
    [Range(0.0f, 2.0f)] public float bobPosAmount = 0.5f;
    [Range(0.1f, 10.0f)] public float bobRotSpeed = 1.0f;
    [Range(0.0f, 30.0f)] public float bobRotAmount = 15.0f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        // Store the initial position and rotation of the model
        initialPosition = transform.position;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Calculate vertical bobbing offset
        float addToPos = Mathf.Sin(Time.time * bobPosSpeed) * bobPosAmount;
        transform.position = initialPosition + Vector3.up * addToPos; 

        // Calculate rotational bobbing
        // float xRot = Mathf.Sin(Time.time * bobRotSpeed) * bobRotAmount;
        // float zRot = Mathf.Sin((Time.time - 1.0f) * bobRotSpeed) * bobRotAmount;

        // // Apply bobbing rotation on top of the initial rotation
        // transform.localRotation = initialRotation * Quaternion.Euler(xRot, 0, zRot);
    }
}