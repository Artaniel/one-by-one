using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadHint : CurrentEnemyHint
{
    public float speed = 22f;

    private Transform player;
    private TrailRenderer[] trailRenderers;

    public float rotationSpeedFlying = 1080f;
    public float rotationSpeedAttached = 120f;

    protected override void Start()
    {
        trailRenderers = GetComponentsInChildren<TrailRenderer>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetupHintVisual(null);
    }

    protected override void SetupHintVisual(Transform parent)
    {
        this.parent = parent;
        if (parent == null)
        {
            ClearTrails();
            transform.position = player.position;
            transform.SetParent(player);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            transform.SetParent(null);
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    protected override void UpdateVisual(float timeFraction)
    {
        if (parent != null)
        {
            Vector3 from = transform.position; from.z = 0;
            Vector3 to = parent.position; to.z = 0;
            transform.position = Vector3.MoveTowards(from, to, speed * Time.deltaTime);
            if (Vector3.Distance(from, to) < 1)
            {
                transform.Rotate(0, 0, rotationSpeedAttached * Time.deltaTime);
            }
            else
            {
                transform.Rotate(0, 0, rotationSpeedFlying * Time.deltaTime);
            }
        }
    }

    private void ClearTrails()
    {
        foreach (var rend in trailRenderers)
        {
            rend.Clear();
        }
    }
}
