using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSpeedSkill", menuName = "ScriptableObject/ActiveSkill/ActiveSpeedSkill", order = 1)]
public class ActiveSpeedSkill : ActiveSkill
{
    private CharacterMovement character;
    [SerializeField] private GameObject VFXTrail = null;

    protected ActiveSpeedSkill()
    {
        cooldownDuration = 5f;
        activeDuration = 3f;
    }

    public override void InitializeSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        character = player.GetComponent<CharacterMovement>();
    }

    protected override void ActivateSkill()
    {
        character.speed *= 1.65f;
        if (!trail)
        {
            trail = Instantiate(VFXTrail, player.transform.position - (player.transform.up * 0.4f), Quaternion.identity, player.transform).GetComponent<TrailRenderer>();
        }
        else
        {
            trail.emitting = true;
        }
    }

    protected override void EndOfSkill()
    {
        character.speed /= 1.65f;
        trail.emitting = false;
    }

    private TrailRenderer trail;
    private GameObject player;
}
