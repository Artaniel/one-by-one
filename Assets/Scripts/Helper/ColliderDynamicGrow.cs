using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDynamicGrow : MonoBehaviour
{
    [SerializeField] private Vector2 min = Vector2.zero;
    [SerializeField] private Vector2 max = Vector2.one;
    [SerializeField] private float maxTime = 0.5f;

    void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    void OnEnable()
    {
        coll.size = min;
    }

    void Update()
    {
        if (Pause.Paused) return;

        coll.size = Vector2.Lerp(min, max, timer / maxTime);
        timer += Time.deltaTime;
    }

    private BoxCollider2D coll;
    private float timer = 0;
}
