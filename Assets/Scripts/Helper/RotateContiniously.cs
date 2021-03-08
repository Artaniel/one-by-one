using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateContiniously : MonoBehaviour
{
    bool isRigidbody = false;
    public float speed = 90f;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isRigidbody = rb != null;
    }
    
    void Update()
    {
        if (isRigidbody)
        {
            rb.rotation = speed;
        }
        else
        {
            transform.Rotate(0, 0, speed * Time.deltaTime);
        }
    }
}
