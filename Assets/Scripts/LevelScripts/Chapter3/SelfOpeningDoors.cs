using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfOpeningDoors : MonoBehaviour
{
    public float timeToSwitch = 3f;
    public bool selfControlled = true;
    public bool openedFromStart = false;

    private bool closed;
    private float timeToSwitchLeft;

    Animator[] animators;

    void Start()
    {
        animators = GetComponentsInChildren<Animator>();
        timeToSwitchLeft = timeToSwitch;

        closed = openedFromStart; // inverted because next function inverts
        ChangeState();

        if (!selfControlled) enabled = false;
    }

    void Update()
    {
        if (timeToSwitchLeft < 0)
        {
            timeToSwitchLeft = timeToSwitch;
            ChangeState();
        }
        timeToSwitchLeft -= Time.deltaTime;
    }

    void ChangeState()
    {
        if (closed) Open();
        else Close();
    }

    void Close()
    {
        closed = true;
        foreach (var animator in animators)
        {
            animator.Play("Close");
        }
    }

    void Open()
    {
        closed = false;
        foreach (var animator in animators)
        {
            animator.Play("Open");
        }
    }
}
