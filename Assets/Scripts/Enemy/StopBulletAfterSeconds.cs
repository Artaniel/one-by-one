using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBulletAfterSeconds : MonoBehaviour
{
    [SerializeField] private Vector2 stopTimeRange = new Vector2(0.5f, 1);
    [SerializeField] private float minSpeed = 0;

    private void Start()
    {
        bulletLife = GetComponent<EnemyBulletLife>();
        startSpeed = bulletLife.BulletSpeed;
    }

    private void Update()
    {
        bulletLife.BulletSpeed = startSpeed * Mathf.Max(minSpeed, (1 - Mathf.Pow(Mathf.InverseLerp(stopTimeRange.x, stopTimeRange.y, time), 3)));
        time += Time.deltaTime;
    }

    private float startSpeed = 0;
    private float time = 0;
    private EnemyBulletLife bulletLife;
}
