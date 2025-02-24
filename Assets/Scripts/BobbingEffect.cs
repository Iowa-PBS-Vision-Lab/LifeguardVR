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

    }

    void Update()
    {
        if (animator != null)
        {
            if(transform.parent.name.Contains("Drown")){
                animator.Play("Drown0");
            }
            else{
                animator.Play("Swim0");
            }
        }
    }
}
