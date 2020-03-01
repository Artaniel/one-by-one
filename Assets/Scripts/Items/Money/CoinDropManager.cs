using UnityEngine;
using Game.Events;

public class CoinDropManager : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    
    private void Awake()
    {
        EventManager.OnMonsterDead.AddListener(DropCoins);
    }

    void DropCoins(Vector3 place)
    {
        Debug.Log("Dropped coins");
        Instantiate(coinPrefab, place, Quaternion.identity);
    }
}