using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] public float speed;
    [HideInInspector]public bool allowDirectionSwitch = true;
    [HideInInspector]public Vector2 direction;
    
    private Animator anim;
    private Animator shadowAnim;
    new private AudioSource audio;
    private float speedMultiplier = 1f;
    new private Rigidbody2D rigidbody;
    private SkillManager skillManager;
    
    private float dummySpeed = 0;
    private Vector3 dummyDestination;
    private bool dummyReturnNormal;

    [HideInInspector] public bool shouldDoOOBCheck = true;

    private float gravityX;
    private float gravityY;

    private float inputAcceleration = 7f;
    private float inputDeceleration = 9f;

    private void Awake()
    {
        allowDirectionSwitch = true;
        audio = GetComponent<AudioSource>();       
        var anims = GetComponentsInChildren<Animator>();
        anim = anims[0];
        shadowAnim = anims[1];
        rigidbody = GetComponent<Rigidbody2D>();
        skillManager = GetComponent<SkillManager>();
        characterLife = GetComponent<CharacterLife>();

        inputActions = new PlayerControls();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

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
        Vector2 axis = inputActions.gameplay.move.ReadValue<Vector2>();
        ApplyGravity(ref axis);
        if (allowDirectionSwitch) direction = Vector2.ClampMagnitude(new Vector2(axis.x, axis.y), 1f);
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
            dummySpeed = 0;
            if (dummyReturnNormal)
            {
                shouldDoOOBCheck = true;
                GetComponent<UnityEngine.Collider2D>().enabled = true;
            }
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

    public void DummyMovement(Vector3 dummyDestination, float timeToDestination = 0.5f, bool dummyReturnNormal = true)
    {
        this.dummyReturnNormal = dummyReturnNormal;
        this.dummyDestination = dummyDestination;
        GetComponent<UnityEngine.Collider2D>().enabled = false;
        dummySpeed = (dummyDestination - transform.position).magnitude / timeToDestination;
        shouldDoOOBCheck = false;
    }

    private IEnumerator RoomTransitionDummyInner(Vector3 dummyDoor1, Vector3 dummyDoor2, Vector3 destination, float timeToDestination)
    {
        if (timeToDestination < 0.15f + 0.25f) timeToDestination = 0.15f + 0.25f;
        DummyMovement(dummyDoor1, 0.15f, dummyReturnNormal: false);
        yield return new WaitForSeconds(0.15f);
        characterLife.HidePlayer();
        yield return new WaitForSeconds(0.25f);
        characterLife.RevealPlayer();
        transform.position = dummyDoor2;
        DummyMovement(destination, timeToDestination - 0.15f - 0.25f, dummyReturnNormal: true);
    }

    public void DummyRoomTransition(Vector3 dummyDoor1, Vector3 dummyDoor2, Vector3 destination, float timeToDestination = 0.7f)
    {
        StartCoroutine(RoomTransitionDummyInner(dummyDoor1, dummyDoor2, destination, timeToDestination));
    }


    private void ApplyGravity(ref Vector2 moveVector)
    {
        if (moveVector.x == 0)
        {
            gravityX = Mathf.MoveTowards(gravityX, 0f, Time.deltaTime * inputDeceleration);
        }
        else
            gravityX = Mathf.MoveTowards(gravityX, moveVector.x, Time.deltaTime * inputAcceleration);

        if (moveVector.y == 0)
            gravityY = Mathf.MoveTowards(gravityY, 0f, Time.deltaTime * inputDeceleration);
        else
            gravityY = Mathf.MoveTowards(gravityY, moveVector.y, Time.deltaTime * inputAcceleration);

        gravityX = Mathf.Clamp(gravityX, -1, 1);
        gravityY = Mathf.Clamp(gravityY, -1, 1);
        moveVector.x = gravityX;
        moveVector.y = gravityY;
    }

    private WeaponSkill.WeaponType weaponType = WeaponSkill.WeaponType.Empty;
    private CharacterLife characterLife;
    private PlayerControls inputActions;
}
