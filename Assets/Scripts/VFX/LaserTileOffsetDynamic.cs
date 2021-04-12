using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTileOffsetDynamic : MonoBehaviour
{
    LineRenderer lineRenderer;
    public float speed = 1f;
    private float tiling;

    Vector2 offset = Vector2.zero;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        tiling = lineRenderer.material.GetTextureScale("_BaseMap").x;
        speed = speed / tiling;
    }


    void Update()
    {
        offset += Vector2.right * speed * Time.deltaTime;
        lineRenderer.material.SetTextureOffset("_BaseMap", offset);    
    }
}
