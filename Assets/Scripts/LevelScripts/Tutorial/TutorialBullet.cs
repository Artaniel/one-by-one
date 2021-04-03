using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBullet : MonoBehaviour
{
    public float TTDLeft = 2f;
    public float speed = 1f;

    void FixedUpdate()
    {
        if (Pause.Paused) return;
        TTDLeft -= Time.fixedDeltaTime;
        Move();

        if (TTDLeft < 0)
        {
            DestroyBullet();
        }
    }

    void OnTriggerEnter2D(UnityEngine.Collider2D coll)
    {
        if (coll.GetComponent<Door>() != null)
        {
            coll.GetComponent<Door>().Unlock(forceAnimation: true);
            coll.GetComponent<BoxCollider2D>().enabled = false; // remove blocker collider
            Destroy(gameObject, 0.1f);
        }
    }

    protected virtual void Move()
    {
        transform.Translate(Vector2.right * speed * Time.fixedDeltaTime);
    }

    public virtual void DestroyBullet()
    {
        this.enabled = false;
        GetComponent<UnityEngine.Collider2D>().enabled = false;
        GetComponent<DynamicLightInOut>()?.FadeOut();
        Destroy(gameObject, 1);
    }
}