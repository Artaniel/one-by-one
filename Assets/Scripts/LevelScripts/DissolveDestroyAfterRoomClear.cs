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
    private bool appliedListener = false;

    private void Awake()
    {
        dissolveMaterial = GetComponent<SpriteRenderer>().material;
        coll = GetComponent<Collider2D>();
        room = GetComponentInParent<Room>(); // For pre-spawned 
    }

    private void OnEnable()
    {
        shouldDissolve = false;
        if (stopCollider) coll.enabled = true;
        dissolveParam = 0;
        dissolveMaterial.SetFloat("_Dissolve", dissolveParam);

        if (!room) room = Labirint.currentRoom; // Cause room changes on each spawn from PoolManager
        if (room.cleared)
            StartDissolve();
        else  if (!appliedListener)
        {
            appliedListener = true;
            room.OnThisClear.AddListener(StartDissolve);
        }
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
        appliedListener = false;
        room = null;
        shouldDissolve = true;
        if (stopCollider) coll.enabled = false;
        PoolManager.ReturnToPool(gameObject, timeToDestroy);
    }
}
