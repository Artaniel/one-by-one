using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaoticTremble : MonoBehaviour
{
    public float speed = 5f;
    public float frequency = 0.1f;
    public float lastTimechanged = -1;
    public Vector2 direction;

    void Update()
    {
       if (Time.time - lastTimechanged > frequency)
       {
            direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            lastTimechanged = Time.time;
       }
       transform.Translate(direction * (speed * Time.deltaTime));
    }
}
