using System;
using Game.Events;
using UnityEngine;

public class CoinPickupManager : MonoBehaviour
{
    private void Awake()
    {
        EventManager.OnCoinPickup.AddListener(PickupCoin);
    }

    void PickupCoin(Vector3 place, int amount)
    {
        Debug.Log($"Added {amount} coins");
    }
}