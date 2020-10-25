﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubEnterLabyrinth : MonoBehaviour
{
    public float delayedTime = 0;
    public string sceneToLoad = "Hub";
    private Scene scene;
    private int sceneToLoadInt;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (loading || !coll.CompareTag("Player")) return;

        loading = true;

        if (delayedTime > 0)
            StartCoroutine(DelayedLoad());
        else
            LoadSceneFromHub();
    }

    private void LoadSceneFromHub()
    {
        RelodScene.OnSceneChange?.Invoke();
        SceneLoading.LoadScene(sceneToLoad);
    }

    private IEnumerator DelayedLoad()
    {
        PlayAnimation();
        yield return new WaitForSeconds(delayedTime);
        LoadSceneFromHub();
    }

    protected virtual void PlayAnimation()
    {
        gameObject?.GetComponentInChildren<Animator>().SetBool("Load", true);
        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<CharacterMovement>().speed = 0;
        player.GetComponent<Rigidbody2D>().Sleep();
        StartCoroutine(EatPlayer(player));
    }

    private IEnumerator EatPlayer(GameObject player)
    {
        yield return new WaitForSeconds(1.53f);
        var playerSprites = player.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in playerSprites)
        {
            sprite.gameObject.SetActive(false);
        }
        
    }

    private bool loading = false;
}
