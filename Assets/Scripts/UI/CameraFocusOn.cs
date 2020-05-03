using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFocusOn : MonoBehaviour
{
    void Start()
    {
        mainCam = Camera.main;
        cineCamera = mainCam.GetComponentInChildren<CinemachineVirtualCamera>();
    }
    
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (focusing && timeLeft > 0)
        {
            mainCam.transform.position =
                Vector3.Lerp(focusEndPosition, focusStartPosition, timeLeft / length);
            cineCamera.m_Lens.OrthographicSize = Mathf.Lerp(lensOrthoEnd, lensOrthoStart, timeLeft / length);
        }
        else if (focusEndPosition != Vector3.zero && timeLeft > 0)
        {
            mainCam.transform.position =
                Vector3.Lerp(focusStartPosition, focusEndPosition, timeLeft / length);
            cineCamera.m_Lens.OrthographicSize = Mathf.Lerp(lensOrthoStart, lensOrthoEnd, timeLeft / length);
        }
    }

    private void DisableOtherScripts()
    {
        var camLabyrinth = mainCam.GetComponent<CameraForLabirint>();
        if (camLabyrinth) camLabyrinth.enabled = false;
        var camFollow = mainCam.GetComponent<CameraFollowScript>();
        if (camLabyrinth) camLabyrinth.enabled = false;
    }

    private void EnableOtherScripts()
    {
        var camLabyrinth = mainCam.GetComponent<CameraForLabirint>();
        if (camLabyrinth) camLabyrinth.enabled = true;
        var camFollow = mainCam.GetComponent<CameraFollowScript>();
        if (camLabyrinth) camLabyrinth.enabled = true;
    }

    public void FocusOn(Vector3 focusOn, float focusDuration, float zoomMultiplier)
    {
        DisableOtherScripts();
        focusing = true;
        length = focusDuration;
        timeLeft = focusDuration;

        focusStartPosition = transform.position;
        focusEndPosition = new Vector3(focusOn.x, focusOn.y, transform.position.z);
        lensOrthoStart = cineCamera.m_Lens.OrthographicSize;
        lensOrthoEnd = cineCamera.m_Lens.OrthographicSize / zoomMultiplier;
    }

    public void UnFocus(float unfocusDuration)
    {
        EnableOtherScripts();
        length = unfocusDuration;
        timeLeft = unfocusDuration;
        focusing = false;
    }

    private Camera mainCam;
    private CinemachineVirtualCamera cineCamera;

    private bool focusing = false;
    private Vector3 focusStartPosition = Vector3.zero;
    private Vector3 focusEndPosition = Vector3.zero;
    private float lensOrthoStart = 0;
    private float lensOrthoEnd = 0;

    private float length = 0;
    private float timeLeft = 0;
}
