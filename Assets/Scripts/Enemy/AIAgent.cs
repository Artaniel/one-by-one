﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AIAgent : MonoBehaviour
{
    public float maxSpeed = 300f;
    public float maxRotation = 200f;
    public float velocityFallBackPower = 3f;
    public float knockBackStability = 1f;
    [HideInInspector] public float orientation;
    [HideInInspector] public float rotation;
    [HideInInspector] public Vector2 externalVelocity;
    public float moveSpeedMult = 1f;
    public bool needsOOBCheck = true;
    protected EnemySteering steering;

    [Header("Behaviours default activation condition")]
    public List<ProximityCheckOption> proximityCheckOption = new List<ProximityCheckOption> { ProximityCheckOption.OnScreen, ProximityCheckOption.GroupAggroable };
    public float timeToLoseAggro = -1;

    public enum ProximityCheckOption
    {
        Distance,
        OnScreen,
        DirectSight,
        Always,
        GroupAggroable,
        DamageAggroable,
        External
    }

    private void Awake()
    {
        maxSpeed += Random.Range(-maxSpeed / 7f, 0);
    }

    private void Start()
    {
        moveBehaviours = GetComponents<MoveBehaviour>();
        rotateBehaviors = GetComponents<Align>();
        behaviours = GetComponents<EnemyBehavior>();

        rigidbody = GetComponent<Rigidbody2D>();
        externalVelocity = Vector2.zero;
        steering = new EnemySteering();
        
        orientation = -transform.rotation.eulerAngles.z;
        rotation = 0;
    }

    protected void FixedUpdate()
    {
        if (Pause.Paused) return;
        if (!allowMovement) return;

        foreach (var i in behaviours)
        {
            i.CalledUpdate();
        }

        if (rigidbody.bodyType == RigidbodyType2D.Static) return;

        foreach (var i in rotateBehaviors)
        {
            rotation += i.GetRotation();
        }
        
        rotation = Mathf.Sign(rotation) * Mathf.Min(Mathf.Abs(rotation), maxRotation);
        orientation += rotation * Time.deltaTime;

        orientation %= 360.0f;
        if (orientation < 0.0f)
        {
            orientation += 360.0f;
        }
        transform.rotation = Quaternion.Euler(0, 0, -orientation);

        Vector2 movement = Vector2.zero;
        foreach (var i in moveBehaviours)
        {
            movement += i.Move();
        }
        Vector2 displacement = moveSpeedMult * movement;
        steering = new EnemySteering();

        Vector2 velocityFallBack =
            externalVelocity * (velocityFallBackPower * Time.deltaTime);
        externalVelocity -= velocityFallBack;
        
        rigidbody.velocity = (externalVelocity + displacement);

        if (needsOOBCheck) OOBCheck();

        rotation = 0;
    }

    protected virtual void Update()
    {
        ProceedPauseUnpause();
    }

    public void KnockBack(Vector2 knockVector)
    {
        externalVelocity += knockVector / knockBackStability;
    }

    public void SetDamaged() => damaged = true;

    public bool IsDamaged() => damaged;

    // TODO: Заменить на Event + Listener?
    private void ProceedPauseUnpause()
    {
        if (Pause.Paused && !wasPausedLastFrame)
        {
            wasPausedLastFrame = true;
            PauseKnockback();
        }

        if (Pause.UnPaused && wasPausedLastFrame)
        {
            wasPausedLastFrame = false;
            ResumeKnockback();
        }
    }

    public void PauseKnockback()
    {
        var rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.isKinematic = true;
        rigidbody.Sleep();
    }

    private void ResumeKnockback()
    {
        orientation = -transform.rotation.eulerAngles.z;
        var rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.WakeUp();
        rigidbody.isKinematic = false;
    }

    private void OOBCheck() {
        if (Labirint.instance && Labirint.currentRoom)
        {
            if (!Labirint.currentRoom.PositionIsInbounds(transform.position))
            {
                transform.position = Labirint.currentRoom.GetNearInboundsPosition(transform.position);
                Debug.Log($"{gameObject.name} is OOB in room {Labirint.GetCurrentRoom().name}");
            }
        }
    }
    
    private Align[] rotateBehaviors;
    private MoveBehaviour[] moveBehaviours;
    private EnemyBehavior[] behaviours;
    private bool wasPausedLastFrame = false;
    private bool allowMovement = true;
    private bool damaged = false;

    new private Rigidbody2D rigidbody;

    // private Dictionary<int, List<EnemySteering>> groups;
}
