﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCameraExternal : MonoBehaviour
{
    public bool shakeOnStart = true;
    public float intensity = 1f;
    public float shakeLength = 0.5f;

    void Start()
    {
        cameraShaker = Camera.main.GetComponent<CameraShaker>();
        cameraShaker.ShakeCamera(intensity, shakeLength);
    }

    private CameraShaker cameraShaker = null;
}