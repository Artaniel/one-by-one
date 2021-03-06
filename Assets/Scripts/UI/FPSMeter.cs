﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSMeter : MonoBehaviour
{
    private Text text;
    private float fpsSum = 0;
    private uint calculations = 0;
    private float fpsTimer;
    private string seed = "";
    
    void Start()
    {
        text = GetComponent<Text>();
        seed = " " + SaveLoading.seed;
    }

    void Update()
    {
        fpsTimer += Time.deltaTime;
        if (fpsTimer >= 1)
        {
            fpsTimer = 0;
            calculations++;
            var currentFPS = 1 / Time.deltaTime;
            fpsSum += currentFPS;
            text.text = $"FPS: {(currentFPS).ToString("0.00")}. Average: { (fpsSum / calculations).ToString("0.00") }";
            if (cheating) text.text += "  <b>CHEATING</b>!";
            text.text += seed;
        }
    }

    [HideInInspector] public bool cheating;
}
