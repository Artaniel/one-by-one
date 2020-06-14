﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    public float speed = 12f;
    
    private Animator anim;
    private Animator shadowAnim;
    new private AudioSource audio;
    private float speedMultiplier = 1f;
    private Camera mainCamera = null;
    private Vector3 previousPosition = new Vector3();
    new private Rigidbody2D rigidbody;


    private void Start()
    {
        audio = GetComponent<AudioSource>();       
        var anims = GetComponentsInChildren<Animator>();
        anim = anims[0];
        shadowAnim = anims[1];
        mainCamera = Camera.main;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Pause.Paused) return;

        Movement();
        OOBCheck();
    }
    
    private void Movement()
    {       
        Vector2 direction;
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        rigidbody.velocity = direction * speed * speedMultiplier * Time.fixedDeltaTime * 50f;
        if (anim != null)
        {
            if (CharacterLife.isDeath) return;

            if (rigidbody.velocity.sqrMagnitude == 0f ) 
            {
                AudioManager.PauseSource("Walk", audio);
                anim.Play("HeroIdle");
                shadowAnim.Play("ShadowIdle");
            }
            else if (AudioManager.isPlaying("Walk", audio) == false)
            {
                AudioManager.Play("Walk", audio);
                anim.Play("HeroWalking");
                shadowAnim.Play("HeroShadow");
            }        
        }
        previousPosition = transform.position;
    }

    public void AddToSpeedMultiplier(float addValue)
    {
        speedMultiplier += addValue;
    }

    private void OOBCheck() 
    {
        if (Labirint.instance != null) 
        {
            if (!Labirint.GetCurrentRoom().GetComponent<Room>().PositionIsInbounds(transform.position))
            {
                Debug.Log("Player OOB alert");
                transform.position = Labirint.GetCurrentRoom().GetComponent<Room>().GetNearInboundsPosition(transform.position);
            }
        }
    }
}
