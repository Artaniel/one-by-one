using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] public float speed;
    [HideInInspector]public bool allowDirectionSwitch;
    [HideInInspector]public Vector2 direction;
    
    private Animator anim;
    private Animator shadowAnim;
    new private AudioSource audio;
    private float speedMultiplier = 1f;
    new private Rigidbody2D rigidbody;
    private SkillManager skillManager;
    
    private float dummySpeed = 0;
    private Vector3 dummyDestination;

    [HideInInspector] public bool shouldDoOOBCheck = true;

    private void Awake()
    {
        allowDirectionSwitch = false;
        audio = GetComponent<AudioSource>();       
        var anims = GetComponentsInChildren<Animator>();
        anim = anims[0];
        shadowAnim = anims[1];
        rigidbody = GetComponent<Rigidbody2D>();
        skillManager = GetComponent<SkillManager>();
        characterLife = GetComponent<CharacterLife>();
    }

    private void FixedUpdate()
    {
        if (Pause.Paused) return;
        Movement();
    }
    
    private void Movement()
    {
        if (CharacterLife.isDeath) return;

        if (dummySpeed > 0) DummyMovementUpdate();
        else NormalMovementUpdate();

        UpdateMoveAnimation();
        if (shouldDoOOBCheck) OOBCheck();
    }

    private void NormalMovementUpdate()
    {
        if (!allowDirectionSwitch) direction = Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1f);
        if (rigidbody.velocity.magnitude > speed * Mathf.Max(0, speedMultiplier))
            rigidbody.AddForce(direction * speed * Mathf.Max(0, speedMultiplier) * 10f); // множитель подобран на глаз, возможно надо покалибровать вместе с трением
        else
            rigidbody.velocity = direction * speed * Mathf.Max(0, speedMultiplier);
    }

    private void DummyMovementUpdate()
    {
        if (Vector3.Distance(transform.position, dummyDestination) > 0.5f)
        {
            rigidbody.velocity = Vector2.zero;
            transform.Translate((dummyDestination - transform.position).normalized * dummySpeed * Time.fixedDeltaTime, Space.World);
        }
        else
        {
            shouldDoOOBCheck = true;
            dummySpeed = 0;
            GetComponent<Collider2D>().enabled = true;
        }
    }

    public void AddToSpeedMultiplier(float addValue)
    {
        speedMultiplier += addValue;
    }

    private void OOBCheck() 
    {
        if (Labirint.instance && Labirint.currentRoom) 
        {
            if (!Labirint.currentRoom.PositionIsInbounds(transform.position))
            {
                Debug.Log("Player OOB alert");
                transform.position = Labirint.currentRoom.GetNearInboundsPosition(transform.position);
            }
        }
    }

    public void UpdateWeaponType(WeaponSkill.WeaponType newWeaponType)
    {
        if (weaponType != WeaponSkill.WeaponType.Empty)
            anim.Play($"HeroWalk/{weaponType}/UnSwitch");
        else
            anim.Play("AfterSwitch");
        weaponType = newWeaponType;
        foreach (var weaponType in WeaponSkill.weaponTypes)
        {
            anim.ResetTrigger($"Take {weaponType}");
        }
        anim.SetTrigger($"Take {weaponType}");
    }

    private void UpdateMoveAnimation()
    {
        if (rigidbody.velocity.sqrMagnitude == 0f)
        {
            AudioManager.PauseSource("Walk", audio);
            anim.SetBool("Moves", false);
        }
        else
        {
            if (anim.GetBool("Moves") == false)
            {
                AudioManager.Play("Walk", audio);
            }
            anim.SetBool("Moves", true);
        }
    }

    public void DummyMovement(Vector3 dummyDestination, float timeToDestination = 0.5f, bool hidePlayer = false)
    {
        this.dummyDestination = dummyDestination;
        GetComponent<Collider2D>().enabled = false;
        dummySpeed = (dummyDestination - transform.position).magnitude / timeToDestination;
        shouldDoOOBCheck = false;
        if (hidePlayer)
        {
            StartCoroutine(HideRevealDummyPlayer(timeToDestination));
        }
    }

    private IEnumerator HideRevealDummyPlayer(float timeSequence)
    {
        yield return new WaitForSeconds(0.1f);
        characterLife.HidePlayer();
        yield return new WaitForSeconds(timeSequence - 0.2f);
        characterLife.RevealPlayer();
    }

    private WeaponSkill.WeaponType weaponType = WeaponSkill.WeaponType.Empty;
    private CharacterLife characterLife;
}
