using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerIntroSequence : MonoBehaviour
{
    public float[] timings;
    public GameObject[] rooms;
    public GameObject[] monsters;

    private GameObject currentRoom;
    private GameObject currentMonster;

    private int i = 0;
    private float timer = 0;

    void Start()
    {
        timer -= Time.deltaTime;
        Camera.main.GetComponent<CameraFocusOn>().FocusOn(Vector3.zero, 0, 4);
        Camera.main.GetComponent<CameraFocusOn>().FocusOn(Vector3.zero, 17f, 0.33f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timings[i])
        {
            CreateScene();
        }
        
    }

    private void CreateScene()
    {
        if (currentRoom)
        {
            currentRoom.SetActive(false);
            currentMonster.SetActive(false);
        }
        currentRoom = Instantiate(rooms[i], transform.position, Quaternion.identity);
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            enemy.SetActive(false);
        }

        currentMonster = Instantiate(monsters[i], transform.position, Quaternion.identity);
        i++;
        currentMonster.GetComponentInChildren<TMPro.TextMeshPro>().enabled = false;
        currentMonster.GetComponentInChildren<Collider2D>().enabled = false;
        currentMonster.GetComponent<MonsterLife>().fadeInTime = 0.15f;
        currentMonster.GetComponent<AIAgent>().enabled = false;
        
    }
}
