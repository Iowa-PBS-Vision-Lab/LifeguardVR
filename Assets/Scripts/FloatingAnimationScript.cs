using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingAnimationScript : MonoBehaviour
{
    private float speed = 0f;
    private float blue;
    private float red;
    private float green;
    // Start is called before the first frame update
    void Start()
    {
        /*red = Random.Range(0, 255)/255;
        green = Random.Range(0, 255)/255;
        blue = Random.Range(0, 255)/255;
        Renderer myRenderer = GetComponent<Renderer>();
        Color newColor = new Color(red, green, blue);
        myRenderer.material.color = newColor;*/
        //Set random speed, just enough to seem random.
        speed = Random.Range(0.95f, 1.05f);
        GetComponent<Animator>().speed = speed;
    }
}
