using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] public float speed;
    [HideInInspector]public bool dashActiveSkill;
    [HideInInspector]public Vector2 direction;
    
    private Animator anim;
    private Animator shadowAnim;
    new private AudioSource audio;
    private float speedMultiplier = 1f;
    new private Rigidbody2D rigidbody;
    private SkillManager skillManager;

    [HideInInspector] public bool shouldDoOOBCheck = true;

    private void Awake()
    {
        dashActiveSkill = false;
        audio = GetComponent<AudioSource>();       
        var anims = GetComponentsInChildren<Animator>();
        anim = anims[0];
        shadowAnim = anims[1];
        rigidbody = GetComponent<Rigidbody2D>();
        skillManager = GetComponent<SkillManager>();
    }

    private void FixedUpdate()
    {
        if (Pause.Paused) return;

        Movement();
        if (shouldDoOOBCheck) OOBCheck();
    }
    
    private void Movement()
    {
        if (!dashActiveSkill) direction = Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1f);
        if (rigidbody.velocity.magnitude > speed * Mathf.Max(0, speedMultiplier))
            rigidbody.AddForce(direction * speed * Mathf.Max(0, speedMultiplier) * 10f); // множитель подобран на глаз, возможно надо покалибровать вместе с трением
        else
            rigidbody.velocity = direction * speed * Mathf.Max(0, speedMultiplier);

        if (CharacterLife.isDeath) return;

        UpdateMoveAnimation();
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
            //shadowAnim.Play("ShadowIdle");
        }
        else
        {
            if (AudioManager.isPlaying("Walk", audio) == false)
            {
                AudioManager.Play("Walk", audio);
            }
            anim.SetBool("Moves", true);
            //shadowAnim.Play("HeroShadow"); k
        }
    }

    private WeaponSkill.WeaponType weaponType = WeaponSkill.WeaponType.Empty;
}
