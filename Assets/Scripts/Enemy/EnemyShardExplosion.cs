using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShardExplosion : MonoBehaviour
{
    public int shardCount = 5;
    public float arcAngle = 90f;
    public GameObject shard;

    public enum Distribution
    {
        Uniform, Random, HalfRandom
    }

    public Distribution shardDistribution = Distribution.HalfRandom;
    [Tooltip("Additional random for HalfRandom distribution")]
    public float randomAngle = 15f;

    void OnEnable()
    {
        for (float angle = -arcAngle / 2f; angle < arcAngle / 2f; angle += arcAngle / shardCount)
        {
            Vector3 hostEulerAngles = transform.rotation.eulerAngles;
            float newZ = 0;// = hostEulerAngles.z + angle + Random.Range(-randomAngle / 2f, randomAngle / 2f);
            switch (shardDistribution)
            {
                case Distribution.Uniform:
                    newZ = hostEulerAngles.z + angle;
                    break;
                case Distribution.Random:
                    newZ = Random.Range(-arcAngle / 2f, arcAngle / 2f);
                    break;
                case Distribution.HalfRandom:
                    newZ = hostEulerAngles.z + angle + Random.Range(-randomAngle / 2f, randomAngle / 2f);
                    break;
                default:
                    break;
            }

            PoolManager.GetPool(shard, transform.position, Quaternion.Euler(hostEulerAngles.x, hostEulerAngles.y, newZ));
        }
    }
}
