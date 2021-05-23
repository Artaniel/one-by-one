using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneScript : MonoBehaviour
{
    private bool used = false;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        Color color1 = sprite.color;
        color1.a = 0f;
        sprite.color = color1;
        StartCoroutine(WarningIfUnused());
    }

    private IEnumerator WarningIfUnused()
    {
        yield return new WaitForSeconds(5f);
        if (!used) Debug.LogWarning($"Is Zone {gameObject.name} attached to anything? Call UseZone() or add in inspector where necessary.");
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

    public Vector3 RandomZonePosition3()
    {
        Vector2 randomZonePosition = RandomZonePosition();
        return new Vector3(randomZonePosition.x, randomZonePosition.y, 0);
    }

    public void UseZone()
    {
        used = true;
    }
}
