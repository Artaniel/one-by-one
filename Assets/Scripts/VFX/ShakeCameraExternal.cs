using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCameraExternal : MonoBehaviour
{
    public bool shakeOnStart = true;
    public float intensity = 1f;
    public float shakeLength = 0.5f;

    void Awake()
    {
        cameraShaker = Camera.main.GetComponent<CameraShaker>();
        if (shakeOnStart) ShakeCamera();
    }

    public void ShakeCamera()
    {
        cameraShaker.ShakeCamera(intensity, shakeLength);
    }

    public void ShakeCamera(float intensity, float shakeLength)
    {
        cameraShaker.ShakeCamera(intensity, shakeLength);
    }

    private CameraShaker cameraShaker = null;
}
