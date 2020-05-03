using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneScript : MonoBehaviour
{
    private bool used = false;
    private SpriteRenderer sprite;

    private void Start()
    {
        if (!used) Debug.LogWarning("Is Zone attached to anything? Ping it or add it in inspector.");
        sprite = GetComponent<SpriteRenderer>();
        Color color1 = sprite.color;
        color1.a = 0f;
        sprite.color = color1;
    }

    public Vector2 RandomZonePosition()
    {
        Vector2 vector = new Vector2(Random.Range(-gameObject.transform.localScale.x/2, 
            gameObject.transform.localScale.x/2) + gameObject.transform.position.x,
            Random.Range(-gameObject.transform.localScale.y/2, 
            gameObject.transform.localScale.y/2) + gameObject.transform.position.y);
        //Debug.Log(vector);
        return vector;
    }

    public void UseZone()
    {
        used = true;
    }
}
