using System;
using Boo.Lang;
using Game.Events;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private List<KeyCode> _cheat = new List<KeyCode>
    {
        KeyCode.I, KeyCode.D, 
        KeyCode.K, KeyCode.F, 
        KeyCode.A,
    };
    private int _cheatIdx = 0;
    
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

    private void Update()
    {
        if (Input.GetKeyDown(_cheat[_cheatIdx]))
        {
            if (++_cheatIdx == _cheat.Count)
            {
                EventManager.Notify("Cheatcode Activated", 5);
                EventManager.OnMoneyChange.Invoke(100);
                _cheatIdx = 0;
            }
        }
        else if (Input.anyKeyDown) _cheatIdx = 0;
    }
}