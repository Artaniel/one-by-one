using UnityEngine;
using Game.Events;

public class CoinsManager : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    
    private void Awake()
    {
        EventManager.OnMonsterDead.AddListener(DropCoins);
        EventManager.OnCoinPickup.AddListener(PickupCoin);
    }

    void DropCoins(Vector3 place)
    {
        Instantiate(coinPrefab, place, Quaternion.identity);
    }
    
    void PickupCoin(Vector3 place, int amount)
    {
        EventManager.OnMoneyChange.Invoke(amount);
    }
}