using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private float timeToDetonate = 1f;
    [SerializeField] private float distance = 1f;
    private float timer = 0f;
    private bool countdownStarted = false;
    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (!Pause.Paused) {
            if (countdownStarted)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)                
                    Blast();
            }
            else {
                if (Vector3.Distance(transform.position, player.transform.position) <= distance) {
                    CountdownStarted();
                }
            }
        }
    }

    private void CountdownStarted()
    {
        countdownStarted = true;
        timer = timeToDetonate;
        //animation?
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void Blast()
    {
        //animation?
        if (Vector3.Distance(transform.position, player.transform.position) <= distance) {
            player.GetComponent<CharacterLife>().Damage();
        }
        Destroy(gameObject);
    }

}
