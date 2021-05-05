using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TimerSpawnBulletMod", menuName = "ScriptableObject/BulletModifier/Timer Spawn Object", order = 1)]
public class TimerSpawnMod : BulletModifier
{
    public GameObject objectToSpawn;
    public float timer = 0.5f;
    private float timerLeft;

    public bool repeat = false;

    public override void StartModifier(BulletLife bullet)
    {
        timerLeft = timer;
    }

    public override void ModifierUpdate(BulletLife bullet)
    {
        if (timerLeft >= 0)
        {
            timerLeft -= Time.deltaTime;
            if (timerLeft < 0)
            {
                PoolManager.GetPool(objectToSpawn, bullet.transform.position, Quaternion.identity);
                if (repeat)
                    timerLeft = timer;
            }
        }
    }
}
