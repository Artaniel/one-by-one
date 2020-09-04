using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.LWRP;

public class Door : MonoBehaviour
{
    [HideInInspector] public Room room;
    public Door connectedDoor;
    public bool locked = false; 
    private GameObject player;

    [HideInInspector] public bool unlockOnTimer = true;
    public bool dontUnlockAuto = false;
    private float timer = 0.5f;
    
    [SerializeField] public Direction.Side direction = Direction.Side.UNSET;
    public string sceneName=""; // name of scene to change on enter this door
    public bool isSpawned = false;
    [SerializeField] private Transform doorVisual = null;

    [SerializeField] private GameObject arrowSprite = null;
    new private Camera camera;
    
    [SerializeField] Sprite visitedSprite = null;

    void Awake()
    {
        //doorVisual = transform.GetChild(0); // moved to set from inspector
        //spriteRenderer = doorVisual.GetComponent<SpriteRenderer>();
        camera = Camera.main;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        switch (direction)
        {
            case Direction.Side.LEFT:
                doorVisual.localPosition += Vector3.left * 3;
                break;
            case Direction.Side.UP:
                doorVisual.Rotate(0, 0, -90);
                doorVisual.localPosition += Vector3.up * 3;
                break;
            case Direction.Side.DOWN:
                doorVisual.Rotate(0, 0, 90);
                doorVisual.localPosition += Vector3.down * 3;
                break;
            case Direction.Side.RIGHT:
                doorVisual.Rotate(0, 0, 180);
                doorVisual.localPosition += Vector3.right * 3;
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (!dontUnlockAuto && isSpawned && unlockOnTimer && locked) {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Unlock();
            }
        }
        ArrowCheck();
        AdjustLightOnVisit();
    }

    private void OnTriggerStay2D(Collider2D collision) // "Stay" needed to make door work if player was on trigger in moment of unlock
    {
        if (isSpawned && !locked && collision.gameObject == player) {
            locked = true;
            timer = timerMax;
            if (sceneName == "") {
                connectedDoor.room.MoveToRoom(connectedDoor);
            } else {
                Metrics.OnWin();
                RelodScene.OnSceneChange?.Invoke();
                SceneLoading.LoadScene(sceneName);
            }
        }
    }

    public void Unlock(bool forceAnimation = false) {
        doorLight = GetComponentInChildren<Light2D>();
        if (doorLight) doorLightColor = doorLight.color;

        if (locked && isSpawned)
        {
            locked = false;
            if (forceAnimation || !Labirint.instance || !Labirint.instance.blueprints[room.roomID].visited)
            {
                foreach (var animation in GetComponentsInChildren<Animation>())
                {
                    animation.Play();
                }
                foreach (var animation in GetComponentsInChildren<Animator>())
                {
                    animation.Play("Open");
                }
            }
            timer = 0;
        }
    }

    public void Lock()
    {
        doorLight = GetComponentInChildren<Light2D>();
        if (doorLight) doorLightColor = doorLight.color;

        if (isSpawned)
        {
            locked = true;
            //animation?
            timer = timerMax;
        }
        else
        {
            foreach (var animation in GetComponentsInChildren<Animator>())
            {
                animation.Play("Close");
            }
        }
    }

    public void SpawnDoor() {
        if (!isSpawned)
        {
            isSpawned = true;
            doorVisual.gameObject.SetActive(true);
        }
    }

    public bool directionAutoset() {
        if (sceneName == "")
        {
            if (transform.localPosition.x / (Mathf.Abs(transform.localPosition.y) + 0.001f) > 2)    // эврестическая оценка направления от центра на дверь, оверрайдится из инспектора
            {                                                                                       // если дверь на правой стене x/|y|>2 в локальных координатах то дверь правая, и т.п.
                direction = Direction.Side.RIGHT;
            }
            else if (transform.localPosition.x / (Mathf.Abs(transform.localPosition.y) + 0.001f) < -2) // + 0.001f - защита от деления на ноль
            {
                direction = Direction.Side.LEFT;
            }
            else if (transform.localPosition.y / (Mathf.Abs(transform.localPosition.x) + 0.001f) > 2)
            {
                direction = Direction.Side.UP;
            }
            else if (transform.localPosition.y / (Mathf.Abs(transform.localPosition.x) + 0.001f) < -2)
            {
                direction = Direction.Side.DOWN;
            }
            else
            {
                Debug.LogWarning("Cant get door dirrection automaticaly. Need to set it in inspector manually.");
                return false;
            }
            return true;
        }
        return false;
    }
    
    private void OnDrawGizmos()
    {
        if (isSpawned)
        {
            Gizmos.color = Color.green; // green box for trigger
            Gizmos.DrawWireCube(transform.position + (Vector3)GetComponent<BoxCollider2D>().offset, GetComponent<BoxCollider2D>().size);

            BoxCollider2D blocker = null; // red box for blocker
            foreach (BoxCollider2D coll in GetComponentsInChildren<BoxCollider2D>())
            {
                if (coll.gameObject != gameObject) blocker = coll;
            }
            if (blocker != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(blocker.transform.position + (Vector3)blocker.offset, blocker.size);
            }

            if (connectedDoor)
            { // blue line for connection between doors
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, connectedDoor.transform.position);
            }

            if (locked)
            {
                Gizmos.color = Color.red; // red circle for locked door
                Gizmos.DrawWireSphere(transform.position, 1);
            }
            else
            {
                Gizmos.color = Color.green; // green circle for unlocked door, and spawn position
                Gizmos.DrawWireSphere(transform.position, 1);
            }
        }
    }


    private void ArrowCheck() {
        if (arrowSprite && room.roomID == Labirint.instance.currentRoomID && isSpawned && !locked && timer <= 0)
        {
            bool arrowNeeded = false;
            Vector3 viewportPosition = camera.WorldToViewportPoint(transform.position);
            float shiftFromCenter = 5f;
            if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1)
            {
                arrowNeeded = true;
            }

            if (arrowSprite.activeSelf != arrowNeeded)
                arrowSprite.SetActive(arrowNeeded);
            if (arrowNeeded)
            {
                arrowSprite.transform.rotation = Quaternion.LookRotation(Vector3.back, transform.position - player.transform.position);
                arrowSprite.transform.position = player.transform.position + arrowSprite.transform.up * shiftFromCenter;
                if (connectedDoor)
                { // exception for exit to another scene
                    if (Labirint.instance.blueprints[connectedDoor.room.roomID].visited)
                    {
                        arrowSprite.GetComponentInChildren<SpriteRenderer>().sprite = visitedSprite;
                    }
                }
            }
        }
        else if (arrowSprite && arrowSprite.activeSelf) arrowSprite.SetActive(false);
    }

    private void AdjustLightOnVisit()
    {
        if (doorLight && connectedDoor && Labirint.instance.blueprints[connectedDoor.room.roomID].visited)
        {
            doorLight.color = Color.Lerp(Color.white, doorLightColor, timer / timerMax);
        }
    }

    private Light2D doorLight;
    private Color doorLightColor;
    private float timerMax = 0.5f;
}
