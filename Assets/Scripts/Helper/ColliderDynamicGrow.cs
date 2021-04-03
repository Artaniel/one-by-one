using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDynamicGrow : MonoBehaviour
{
    public bool shouldGrow = true;

    enum ColliderType { Circle, Box };
    ColliderType colliderType;

    [SerializeField] private Vector2 min = Vector2.zero;
    [SerializeField] private Vector2 max = Vector2.one;
    [SerializeField] private float maxTime = 0.5f;

    void Awake()
    {
        coll = GetComponent<UnityEngine.Collider2D>();
        if (coll is BoxCollider2D)
        {
            colliderType = ColliderType.Box;
            boxColl = coll as BoxCollider2D;
        }
        else
        {
            colliderType = ColliderType.Circle;
            circleColl = coll as CircleCollider2D;
        }
    }

    void OnEnable()
    {
        timer = 0;
        if (colliderType == ColliderType.Box)
        {
            BoxColliderInit(boxColl);
        }
        else if (colliderType == ColliderType.Circle)
        {
            CircleColliderInit(circleColl);
        }
    }

    void Update()
    {
        if (Pause.Paused || !shouldGrow) return;
        timer += Time.deltaTime;

        if (colliderType == ColliderType.Box)
        {
            BoxColliderIncrease(boxColl);
        }
        else if (colliderType == ColliderType.Circle)
        {
            CircleColliderIncrease(circleColl);
        }
    }

    void BoxColliderInit(BoxCollider2D coll)
    {
        coll.size = min;
    }

    void BoxColliderIncrease(BoxCollider2D coll)
    {
        coll.size = Vector2.Lerp(min, max, timer / maxTime);
    }

    void CircleColliderInit(CircleCollider2D coll)
    {
        coll.radius = min.x;
    }

    void CircleColliderIncrease(CircleCollider2D coll)
    {
        coll.radius = Mathf.Lerp(min.x, max.x, timer / maxTime);
    }

    private UnityEngine.Collider2D coll;
    private BoxCollider2D boxColl;
    private CircleCollider2D circleColl;
    private float timer = 0;
}
