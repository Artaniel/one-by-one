using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantFireOnEnter : MonoBehaviour
{
    public GameObject firePrefab;
    private bool ignited = false;
    private Room currentRoom;

    void Start()
    {
        currentRoom = GetComponentInParent<Room>();
    }

    void Update()
    {
        currentRoom.OnThisEnter.AddListener(Ignite);    
    }

    void Ignite()
    {
        currentRoom.OnThisEnter.RemoveListener(Ignite);
        FireOnTilemap.StartFire(transform.position, firePrefab);
    }
}
