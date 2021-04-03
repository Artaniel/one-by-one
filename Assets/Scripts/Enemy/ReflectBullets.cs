using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectBullets : MonoBehaviour
{
    [SerializeField] private GameObject bulletReflectAnim = null;
    [SerializeField] private EnemyReflectBulletMod reflectBulletMod = null;

    private void Start()
    {
        aiAgent = GetComponentInParent<AIAgent>();
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D coll)
    {
        var bulletLife = coll.gameObject.GetComponent<BulletLife>();
        if (bulletLife)
        {
            RaycastHit2D hit = Physics2D.Raycast(coll.transform.position, coll.transform.right,
                float.PositiveInfinity, LayerMask.GetMask("Default"));
            if (hit)
            {
                Vector2 reflectDir = Vector2.Reflect(coll.transform.right, hit.normal);
                float rot = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
                coll.transform.eulerAngles = new Vector3(0, 0, rot);
                bulletLife.KnockBack(aiAgent);
                TurnBulletIntoEnemy(bulletLife);
                var reflection = PoolManager.GetPool(bulletReflectAnim, coll.transform.position, Quaternion.Euler(0, 0, rot - 90));
                var reflectionAnim = reflection.GetComponentInChildren<Animation>();
                reflectionAnim.wrapMode = WrapMode.Once;
                PoolManager.ReturnToPool(reflection, 2f);
            }
        }
    }

    private void TurnBulletIntoEnemy(BulletLife bullet)
    {
        var instance = bullet.AddMod(reflectBulletMod);
        instance.ApplyModifier(bullet);
    }

    private AIAgent aiAgent = null;
}
