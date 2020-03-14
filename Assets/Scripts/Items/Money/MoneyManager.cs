using System;
using Game.Events;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static int MoneyAmount
    {
        get => PlayerPrefs.GetInt("MoneyAmount");
        private set => PlayerPrefs.SetInt("MoneyAmount", value);
    }

    private void Awake()
    {
        EventManager.OnMoneyChange.AddListener(OnMoneyChange);
        if (!PlayerPrefs.HasKey("MoneyAmount"))
            MoneyAmount = 0;
    }

    private void OnMoneyChange(int delta)
    {
        MoneyAmount += delta;
    }
}