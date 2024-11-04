using UnityEngine;

public class BobbingEffect : MonoBehaviour
{
    public float bobPosSpeed = 1.0f;   // Speed of vertical bobbing
    public float bobPosAmount = 0.5f;  // Amplitude of vertical bobbing
    public float bobRotSpeed = 1.0f;   // Speed of rotational bobbing
    public float bobRotAmount = 15.0f; // Amplitude of rotational bobbing

    private float xRot;
    private float zRot;

    void Update()
    {
        // Calculate vertical bobbing offset
        float addToPos = Mathf.Sin(Time.time * bobPosSpeed) * bobPosAmount;
        
        //wait for to add timing for adding target state
        transform.position += Vector3.up * addToPos * Time.deltaTime;

        // Calculate rotational bobbing
        xRot = Mathf.Sin(Time.time * bobRotSpeed) * bobRotAmount;
        zRot = Mathf.Sin((Time.time - 1.0f) * bobRotSpeed) * bobRotAmount;

        // Apply rotation and retain the current y rotation
        transform.eulerAngles = new Vector3(xRot, transform.eulerAngles.y, zRot);
    }
}
