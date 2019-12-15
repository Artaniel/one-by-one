﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentEnemy : MonoBehaviour
{
    [SerializeField]
    GameObject CanvasPrefab = null;

    // Start is called before the first frame update
    void Awake()
    {
        var canvasEnemyName = Instantiate(CanvasPrefab);
        var canvasScript = canvasEnemyName.GetComponent<Canvas>();
        canvasScript.worldCamera = Camera.main;
        canvasScript.sortingLayerName = "OnEffect";
        EnemyName = canvasEnemyName.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public static void SetCurrentEnemy(string enemyName, GameObject enemy)
    {
        EnemyName.text = enemyName;
    }

    public static void SetCurrentEnemyName(string enemyName)
    {
        EnemyName.text = enemyName;
    }

    private GameObject gameController;
    public static TMPro.TextMeshProUGUI EnemyName;
}
