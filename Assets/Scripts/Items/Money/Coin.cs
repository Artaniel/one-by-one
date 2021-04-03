using UnityEngine;
using Game.Events;

public class Coin : PickupableItem
{
    public int Amount = 1;
    
    protected override void PickUp(UnityEngine.Collider2D player)
    {
        EventManager.OnCoinPickup.Invoke(transform.position, Amount);
        
        AudioManager.Play("CoinPickup", GetComponent<AudioSource>());
        Destroy(transform.gameObject);
    }
}
