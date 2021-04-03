using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRandomlyOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        transform.Rotate(0, 0, Random.Range(0, 360f));
    }
}
