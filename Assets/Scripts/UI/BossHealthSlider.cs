using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthSlider : MonoBehaviour
{
    [SerializeField] AlphaManager sliderAlphaManager = null;

    // Start is called before the first frame update
    void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        sliderAlphaManager.HideImmediate();
    }

    public void UpdateSlider(float value)
    {
        slider.value = value;
    }

    void Update()
    {
        sliderAlphaManager.Update(Time.deltaTime);
    }

    public void Show() => sliderAlphaManager.Show();

    private Slider slider;
}
