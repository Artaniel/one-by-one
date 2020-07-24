using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthSlider : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        slider = GetComponentInChildren<Slider>();
    }

    public void UpdateSlider(float value)
    {
        slider.value = value;
    }

    private Slider slider;
}
