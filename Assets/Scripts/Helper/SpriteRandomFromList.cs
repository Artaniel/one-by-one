using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRandomFromList : MonoBehaviour
{
    public Sprite[] sprites;

    void OnEnable()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
