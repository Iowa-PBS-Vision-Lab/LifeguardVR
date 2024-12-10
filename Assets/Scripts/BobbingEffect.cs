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
    private float pauseTimer = 0f;
    private bool isPaused = false;

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
        if (animator != null)
        {
            if (isPaused)
            {
                pauseTimer += Time.deltaTime;
                if (pauseTimer >= 2f)
                {
                    ResumeAnimation();
                    pauseTimer = 0f;
                    isPaused = false;
                }
            }
            else if (!isBobbingPaused)
            {
                // Calculate vertical bobbing offset using Animator component
                elapsedTime += Time.deltaTime;
                float addToPos = Mathf.Sin(elapsedTime * bobPosSpeed) * bobPosAmount;
                animator.SetFloat("ElapsedTime", elapsedTime);
                transform.position = initialPosition + Vector3.up * addToPos;

                // Start the pause when reaching the bottom position
                if (Mathf.Sin(elapsedTime * bobPosSpeed) <= -0.99f && !isPaused)
                {
                    PauseAnimation();
                    isPaused = true;
                }
            }
        }
    }

    // Method to pause the animation
    public void PauseAnimation()
    {
        if (animator != null)
        {
            animator.speed = 0f; // Pauses the animation
            isBobbingPaused = true;
        }
    }

    // Method to resume the animation
    public void ResumeAnimation()
    {
        if (animator != null)
        {
            animator.speed = 1f; // Resumes the animation at normal speed
            isBobbingPaused = false;
            // Adjust elapsedTime to ensure smooth upward bobbing
            float currentSinValue = Mathf.Sin(elapsedTime * bobPosSpeed);
            if (currentSinValue <= -0.99f)
            {
                elapsedTime += (Mathf.PI / bobPosSpeed); // Ensure the next value starts moving upwards smoothly
            }
        }
    }
}
