using System.Collections;
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

    public override void ActivateSkill()
    {
        characterMove.dashActiveSkill = true;
        characterLife.dashActiveSkill = noTakeDamage;
        characterMove.direction = characterMove.transform.up*speedDash;
        if (!dashEffect)
        {
            dashEffect = Instantiate(VFXDash, player.transform.position, Quaternion.identity, player.transform).GetComponent<ParticleSystem>();
            trail = player.transform.GetComponentInChildren<TrailRenderer>();
        }
        else
        {
            trail.emitting = true;
            dashEffect.Play();
        }
    }

    public override void EndOfSkill()
    {
        trail.Clear();
        trail.emitting = false;
        dashEffect.Stop();
        characterMove.dashActiveSkill = false;
        characterLife.dashActiveSkill = false;
    }
}