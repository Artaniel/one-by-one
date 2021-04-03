using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupableItem : MonoBehaviour
{
    public float destanceToPickup = 1f;
    public float inactiveTime = 0.5f;
    public AudioClip pickupSound = null;
    private bool active = false;

    protected virtual void Update()
    {
        if (Application.IsPlaying(gameObject)) {
            if (!active) {
                inactiveTime -= Time.deltaTime;
                if (inactiveTime <= 0) active = true;
            }
        } 
    }

    private void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        if (active && collision.CompareTag("Player"))
           _PickUp(collision);
    }

    private void _PickUp(UnityEngine.Collider2D player)
    {
        if (pickupSound) AudioManager.Play(pickupSound);
        PickUp(player);
    }

    protected abstract void PickUp(UnityEngine.Collider2D player);
}
