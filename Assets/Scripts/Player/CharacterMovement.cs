using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    public float speed;
    [HideInInspector]public bool activeSkill;
    [HideInInspector]public Vector2 direction;
    
    private Animator anim;
    private Animator shadowAnim;
    new private AudioSource audio;
    private float speedMultiplier = 1f;
    new private Rigidbody2D rigidbody;


    private void Start()
    {
        activeSkill = false;
        audio = GetComponent<AudioSource>();       
        var anims = GetComponentsInChildren<Animator>();
        anim = anims[0];
        shadowAnim = anims[1];
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
        if (!activeSkill) direction = Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1f);
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
