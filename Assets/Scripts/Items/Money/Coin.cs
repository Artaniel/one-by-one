using UnityEngine;
using Game.Events;

public class Coin : PickupableItem
{
    public int Amount = 1;
    
    protected override void PickUp(Collider2D player)
    {
        Debug.Log("Picked up");
        EventManager.OnCoinPickup.Invoke(transform.position, Amount);
        Destroy(transform.gameObject);
    }
}
