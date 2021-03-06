﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomButterflyAppearance : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> butterflyAppearances = null;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = butterflyAppearances[Random.Range(0, butterflyAppearances.Count)];
    }
}
