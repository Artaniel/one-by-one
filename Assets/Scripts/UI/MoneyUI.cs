using System;
using Game.Events;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI сounterUI = null;

    [SerializeField]
    private AlphaManager alphaManager;

    private void Start()
    {
        сounterUI.text = MoneyManager.MoneyAmount.ToString();
        EventManager.OnMoneyChange.AddListener(UpdateMoneyCounter);
        alphaManager.HideImmediate();
    }

    private void Update()
    {
        alphaManager.Update(Time.deltaTime);
    }

    void UpdateMoneyCounter(int delta)
    {
        alphaManager.Show();
        сounterUI.text = MoneyManager.MoneyAmount.ToString();
    }
}