using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField]
    private float timer = 0.5f;
    public bool toPool = true;

    void OnEnable()
    {
        StartCoroutine(NextFrame()); // Cause OnEnable interrupts execution order
    }

    private IEnumerator NextFrame()
    {
        yield return new WaitForSeconds(timer);
        if (gameObject) PoolManager.ReturnToPool(gameObject);
    }
}
