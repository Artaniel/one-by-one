﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveDashSkill", menuName = "ScriptableObject/ActiveSkill/ActiveDashSkill", order = 1)]
public class ActiveDashSkill : ActiveSkill
{
    private ParticleSystem dashEffect;
    private TrailRenderer trail;
    private GameObject player;
    private CharacterMovement characterMove;
    private CharacterLife characterLife;
    public float speedDash;
    [SerializeField] private GameObject VFXDash = null;

    public bool noTakeDamage; // test flag

    public override void InitializeSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        characterMove = player.GetComponent<CharacterMovement>();
        characterLife = player.GetComponent<CharacterLife>();
    }

    protected override void ActivateSkill()
    {
        characterMove.allowDirectionSwitch = false;
        characterLife.dashActiveSkill = noTakeDamage;
        characterMove.direction = characterMove.transform.up*speedDash;
        if (!dashEffect)
        {
            dashEffect = Instantiate(VFXDash, player.transform.position, Quaternion.identity, player.transform).GetComponent<ParticleSystem>();
            trail = dashEffect.GetComponentInChildren<TrailRenderer>();
        }
        else
        {
            trail.emitting = true;
            dashEffect.Play();
        }
    }

    protected override void EndOfSkill()
    {
        trail.Clear();
        trail.emitting = false;
        dashEffect.Stop();
        characterMove.allowDirectionSwitch = true;
        characterLife.dashActiveSkill = false;
    }
}