using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FloatingAnimationScript : MonoBehaviour
{
    public double startTime = 0f;
    public float elapsedTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        System.Random random = new System.Random();
        startTime = random.NextDouble();
        GetComponent<Animator>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (GetComponent<Animator>().enabled == false && elapsedTime > startTime){
            GetComponent<Animator>().enabled = true;
        }
        
    }
}
