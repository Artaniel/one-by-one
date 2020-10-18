using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveDestroyAfterRoomClear : MonoBehaviour
{
    [SerializeField] private float dissolveSpeed = 1;
    [SerializeField] private float timeToDestroy = 5f;
    [SerializeField] private bool stopCollider = true;

    private float dissolveParam = 0;

    private Material dissolveMaterial;
    private Room room;
    private Collider2D coll;
    private bool shouldDissolve = false;

    private void Awake()
    {
        dissolveMaterial = GetComponent<SpriteRenderer>().material;
        room = GetComponentInParent<Room>();
        if (!room) room = Labirint.currentRoom;
        coll = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        if (stopCollider) coll.enabled = true;
        dissolveParam = 0;
        
        if (room.cleared)
            StartDissolve();
        else 
            room.OnThisClear.AddListener(StartDissolve);
    }

    private void Update()
    {
        if (shouldDissolve)
        {
            dissolveParam += Mathf.Clamp01(Time.deltaTime * dissolveSpeed);
            dissolveMaterial.SetFloat("_Dissolve", dissolveParam);
        }
    }

    private void StartDissolve()
    {
        room.OnThisClear.RemoveListener(StartDissolve);
        shouldDissolve = true;
        if (stopCollider) coll.enabled = false;
        PoolManager.ReturnToPool(gameObject, timeToDestroy);
    }
}
