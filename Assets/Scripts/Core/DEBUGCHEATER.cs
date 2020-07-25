using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUGCHEATER : MonoBehaviour
{
    [SerializeField] private AudioClip killEveryone = null;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        characterLife = GetComponent<CharacterLife>();
        collider2D = GetComponent<CircleCollider2D>();
        canvas.GetComponentInChildren<FPSMeter>().cheating = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            canvas.SetActive(!canvas.activeSelf);
            CurrentEnemyUI.GetCanvasInstance().SetActive(!CurrentEnemyUI.GetCanvasInstance().activeSelf);
            CharacterShooting.GetCursor().SetActive(!CharacterShooting.GetCursor().activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                collider2D.enabled = !collider2D.enabled;
            }
            else
            {
                characterLife.enabled = !characterLife.enabled;
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            characterLife.transform.position = CharacterShooting.GetCursor().transform.position;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                enemy.GetComponent<MonsterLife>().Damage(null, 99999, true);
            }
            var source = AudioManager.instance.GetComponent<AudioSource>();
            source.clip = killEveryone;
            AudioManager.Play("bdush", source);
        }
    }

    private GameObject canvas;
    private CharacterLife characterLife;
    new private CircleCollider2D collider2D;
}
