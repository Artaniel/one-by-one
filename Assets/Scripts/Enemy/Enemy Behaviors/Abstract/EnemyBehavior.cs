﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class EnemyBehavior : MonoBehaviour
{
    // public float priority = 1.0f;
    public float weight = 1.0f;
    public GameObject target;
    protected bool isActive = false;
    protected AIAgent agent;

    [Header("Override behaviour activation condition")]
    public List<AIAgent.ProximityCheckOption> proximityCheckOption = new List<AIAgent.ProximityCheckOption>();
    public float proximityCheckDistance = 19f;
    public bool invertProximityCheck = false;

    [Tooltip("Don't let it be lower than 2 * proximityCheckPeriod. Exception is <0 which means: NEVER")]
    public float timeToLoseAggro = -1;
    private float timeSinceProximityFail = 0;

    public float moveInviteRadius = 2f;
    public int maxInviteLevel = 1;
    public float timeBeforeGroupeAggroOff = 3f;

    public float agroBlockTime = 2f;

    protected virtual void Awake()
    {
        agent = gameObject.GetComponent<AIAgent>();
        if (!target) target = GameObject.FindGameObjectWithTag("Player");
    }

    protected virtual void Start()
    {
        if (proximityCheckOption.Count == 0)
        {
            proximityCheckOption = agent.proximityCheckOption;
        }
        if (timeToLoseAggro == -1)
        {
            timeToLoseAggro = agent.timeToLoseAggro;
        }
        isGroupAggroed = false;
        currentTimeBeforeGroupeAgroOff = timeBeforeGroupeAggroOff;
    }

    public virtual void CalledUpdate()
    {
        if (!isActive && ProximityCheck())
        {
            if (currentAgroBlockTime < 0)
            {
                isActive = true;
                timeSinceProximityFail = 0;
                if (!isGroupAggroed)
                    SetAggroedInvite();
            }
            else
            {
                currentAgroBlockTime -= Time.deltaTime;
            }
        }
        else if (isActive)
        {
            if (timeToLoseAggro > 0)
            {
                timeSinceProximityFail = ProximityCheck() ? 0 : timeSinceProximityFail + Time.deltaTime;
                isActive = timeSinceProximityFail < timeToLoseAggro;
            }
            currentTimeBeforeGroupeAgroOff = Mathf.Max(0, currentTimeBeforeGroupeAgroOff - Time.deltaTime);
            isGroupAggroed = currentTimeBeforeGroupeAgroOff > 0;
        }
        else
        {
            currentTimeBeforeGroupeAgroOff = timeBeforeGroupeAggroOff;
        }

        if(!isActive && proximityCheckOption.Contains(AIAgent.ProximityCheckOption.DamageAggroable))
        {
            ShootingWeapon.shootingEvents.AddListener(Activate);
            if (!isGroupAggroed)
                SetAggroedInvite();
        }
        else if(isActive)
        {
            isGroupAggroed = currentTimeBeforeGroupeAgroOff > 0;
        }
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void Activate()
    {
        isActive = true;
    }

    public void SetAggroedInvite()
    {
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, moveInviteRadius);
            var enemys = (from t in collider2Ds
                          where t.transform.gameObject.tag == "Enemy"
                          where t.gameObject != this.gameObject
                          select t).ToArray();
            foreach(var enemy in enemys)
            {
                var behaviors = enemy.GetComponents<EnemyBehavior>();
                foreach (var behavior in behaviors)
                {
                    if(!behavior.isGroupAggroed)
                        behavior.GetAggroedInvite();
                }
            }
    }
    public void GetAggroedInvite()
    {
        isGroupAggroed = true;
    }

    private Camera currentCamera = null;
    protected virtual bool TargetOnScreen(GameObject target)
    {
        if (target == null) return false;
        if (currentCamera == null)
        {
            currentCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        var enemyInScreenSpace = currentCamera.WorldToViewportPoint(target.transform.position);
        return enemyInScreenSpace.x >= 0 && enemyInScreenSpace.x <= 1 && enemyInScreenSpace.y >= 0 && enemyInScreenSpace.y <= 1;
    }

    protected bool ProximityCheck()
    {
        timeToProximityCheck = Mathf.Max(0, timeToProximityCheck - Time.deltaTime);
        if (timeToProximityCheck > 0) return false;
        timeToProximityCheck = Random.Range(proximityCheckPeriod - 0.05f, proximityCheckPeriod + 0.05f);
        foreach (var proximityCheckOpt in proximityCheckOption)
        {
            var proximityResult = ProximityCheckBody(proximityCheckOpt);
            if (proximityResult == true  && !invertProximityCheck) return true;
            if (proximityResult == false &&  invertProximityCheck) return true;
        }
        return false;
    }

    private bool ProximityCheckBody(AIAgent.ProximityCheckOption proximityCheckOpt)
    {
        switch (proximityCheckOpt)
        {
            case AIAgent.ProximityCheckOption.Distance:
                return Vector3.Distance(target.transform.position, transform.position) <= proximityCheckDistance;
            case AIAgent.ProximityCheckOption.DirectSight:
                // Check if raycast towards player hits player first and not environment
                var hits = (from t in Physics2D.RaycastAll(transform.position, target.transform.position - transform.position, proximityCheckDistance)
                            where t.transform.gameObject.tag == "Environment" || t.transform.gameObject.tag == "Player"
                            select t).ToArray();
                if (hits.Length == 0) return false;
                return (hits[0].transform.CompareTag("Player"));
            case AIAgent.ProximityCheckOption.Always:
                return true;
            case AIAgent.ProximityCheckOption.OnScreen:
                return TargetOnScreen(gameObject);
            case AIAgent.ProximityCheckOption.GroupAggroable:
                return isGroupAggroed;
            case AIAgent.ProximityCheckOption.DamageAggroable:
                return agent.IsDamaged();
            default:
                Debug.LogError("Proximity check undefined condition");
                return false;
        }
    }

    public void AgroBlock(float blockTime)
    {
        isActive = false;
        currentAgroBlockTime = blockTime;
    }

    public void AgroBlock()
    {
        AgroBlock(agroBlockTime);
    }

    protected void RotateInstantlyTowardsTarget()
    {
        Vector2 direction = target.transform.position - transform.position;
        if (direction.magnitude > 0.0f)
        {
            float targetOrientation = Mathf.Atan2(direction.x, direction.y);
            targetOrientation *= Mathf.Rad2Deg;
            targetOrientation += transform.localEulerAngles.z;
            transform.rotation = Quaternion.Euler(0, 0, -MapToRange(targetOrientation));
        }
    }

    protected void RotateRandomlyAtStart()
    {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
    }

    protected float MapToRange(float rotation)
    {
        rotation %= 360.0f;
        if (Mathf.Abs(rotation) > 180.0f)
        {
            if (rotation < 0.0f)
                rotation += 360.0f;
            else
                rotation -= 360.0f;
        }
        return rotation;
    }

    private float proximityCheckPeriod = 0.5f;
    private float timeToProximityCheck = 0.01f;
    [System.NonSerialized]
    public bool isGroupAggroed;
    private float currentTimeBeforeGroupeAgroOff;
    private float currentAgroBlockTime  = 0;
}