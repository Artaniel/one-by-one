using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AIAgent : MonoBehaviour
{
    public float maxSpeed = 3.5f;
    public float maxRotation = 200f;
    public float velocityFallBackPower = 3f;
    public float knockBackStability = 1f;
    [HideInInspector] public float orientation;
    [HideInInspector] public float rotation;
    [HideInInspector] public Vector2 externalVelocity;
    public float moveSpeedMult = 1f;
    protected EnemySteering steering;

    [Header("All Behaviours activation condition")]
    public List<ProximityCheckOption> proximityCheckOption = new List<ProximityCheckOption> { ProximityCheckOption.OnScreen, ProximityCheckOption.GroupAggroable };
    public float timeToLoseAggro = -1;

    public enum ProximityCheckOption
    {
        Distance,
        OnScreen,
        DirectSight,
        Always,
        GroupAggroable,
        ShootingAgroble
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

        Vector2 displacement = (moveSpeedMult * Time.deltaTime) * movement ;
        steering = new EnemySteering();

        Vector2 velocityFallBack =
            externalVelocity * (velocityFallBackPower * Time.deltaTime);

        externalVelocity -= velocityFallBack;

        rigidbody.velocity = (externalVelocity + displacement) * 50 * Time.fixedDeltaTime;

        OOBCheck();

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

    public void StopMovement(float time)
    {
        allowMovement = false;
        StartCoroutine(EnableMovement(time));
    }

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

    private IEnumerator EnableMovement(float wait)
    {
        yield return new WaitForSeconds(wait);
        allowMovement = true;
    }

    public void PauseKnockback()
    {
        var rigidbody = GetComponent<Rigidbody2D>();
        savedVelocity = rigidbody.velocity;
        rigidbody.isKinematic = true;
        rigidbody.Sleep();
    }

    private void ResumeKnockback()
    {
        orientation = -transform.rotation.eulerAngles.z;
        var rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.WakeUp();
        rigidbody.isKinematic = false;
        externalVelocity = savedVelocity;
    }

    private void OOBCheck() {
        if (Labirint.instance && Labirint.currentRoom)
        {
            if (!Labirint.currentRoom.PositionIsInbounds(transform.position))
            {
                if (!(GetComponent<BorderLoopMovement>() || GetComponent<GhostPhase>())) // Harpy Queen & Ghost can go OOB
                {
                    transform.position = Labirint.currentRoom.GetNearInboundsPosition(transform.position);
                    Debug.Log($"{gameObject.name} is OOB in room {Labirint.GetCurrentRoom().name}");
                }
            }
        }
    }

    Vector3 savedVelocity = new Vector3();
    private Align[] rotateBehaviors;
    private MoveBehaviour[] moveBehaviours;
    private EnemyBehavior[] behaviours;
    private bool wasPausedLastFrame = false;
    private bool allowMovement = true;

    new private Rigidbody2D rigidbody;

    // private Dictionary<int, List<EnemySteering>> groups;
}
