using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnStart : MonoBehaviour
{
    public float delay = 0;
    public GameObject objectToSpawn;

    void OnEnable()
    {
        if (delay > 0)
        {
            StartCoroutine(DelayedSpawnObject());
        }
        else
        {
            SpawnObject();
        }
    }

    private IEnumerator DelayedSpawnObject()
    {
        yield return new WaitForSeconds(delay);
        SpawnObject();
    }

    private void SpawnObject()
    {
        PoolManager.GetPool(objectToSpawn, transform.position, Quaternion.identity);
    }
}
