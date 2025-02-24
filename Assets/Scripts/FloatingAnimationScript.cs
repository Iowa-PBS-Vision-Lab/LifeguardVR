using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingAnimationScript : MonoBehaviour
{
    private float speed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        //Set random speed, just enough to seem random.
        speed = Random.Range(0.95f, 1.05f);;
        GetComponent<Animator>().speed = speed;
    }
}
