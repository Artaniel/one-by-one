using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveDashSkill", menuName = "ScriptableObject/ActiveSkill/ActiveDashSkill", order = 1)]
public class ActiveDashSkill : ActiveSkill
{
    private ParticleSystem dashEffect;
    private GameObject player;
    private CharacterMovement characterMove;
    private CharacterLife characterLife;
    public float speedDash;

    public bool noTakeDamage; // test flag

    public override void InitializeSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        dashEffect = player.transform.GetChild(0).GetComponent<ParticleSystem>();
        characterMove = player.GetComponent<CharacterMovement>();
        characterLife = player.GetComponent<CharacterLife>();
    }

    public override void ActivateSkill()
    {
        dashEffect.Play();
        characterMove.activeSkill = true;
        characterLife.activeSkill = noTakeDamage;
        characterMove.direction = characterMove.transform.up*speedDash;
    }

    public override void EndOfSkill()
    {
        dashEffect.Stop();
        characterMove.activeSkill = false;
        characterLife.activeSkill = false;
    }
}