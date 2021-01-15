using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSpawnOnStart : MonoBehaviour
{
    public GameObject objectToSpawn = null;
    public int count = 8;
    public float offset = 0;
    public float radius = 0.5f;

    public Vector3 direction = Vector2.up;

    void OnEnable()
    {
        for (int i = 0; i < count; i ++)
        {
            float angle = 360f / count * i;
            var obj = PoolManager.GetPool(objectToSpawn, transform.position, Quaternion.Euler(0, 0, angle));
            obj.transform.Translate(direction * radius);
        }            
    }
}
