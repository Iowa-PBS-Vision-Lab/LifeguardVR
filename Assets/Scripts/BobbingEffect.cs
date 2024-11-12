using UnityEngine;
using System.Collections;

public class BobbingEffect : MonoBehaviour
{
    [Range(0.1f, 10.0f)] public float bobPosSpeed = 1.0f;
    [Range(0.0f, 10.0f)] public float bobPosAmount = 0.5f;

    private Vector3 initialPosition;
    private static bool isBobbingPaused = false;
    private float elapsedTime = 0f;
    private Animator animator;

    void Start()
    {
        // Store the initial position of the model
        initialPosition = transform.position;
        
        // Get the Animator component
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject.");
            return;
        }

        // Set up Animator parameters
        animator.SetFloat("BobPosSpeed", bobPosSpeed);
        animator.SetFloat("BobPosAmount", bobPosAmount);
    }

    void Update()
    {
        if (animator != null && !isBobbingPaused)
        {
            // Calculate vertical bobbing offset using Animator component
            elapsedTime += Time.deltaTime;
            float addToPos = Mathf.Sin(elapsedTime * bobPosSpeed) * bobPosAmount;
            animator.SetFloat("ElapsedTime", elapsedTime);
            transform.position = initialPosition + Vector3.up * addToPos;
        }
    }
}
