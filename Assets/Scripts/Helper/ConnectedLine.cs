using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedLine : MonoBehaviour
{
    public Transform fromTransform;
    private bool hasFromTransform;
    private SpriteRenderer spriteRenderer;

    public Transform head;
    private bool hasHead;

    public Vector3 from;
    public Vector3 to;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hasHead = head;
    }
    
    void Start()
    {
        hasFromTransform = fromTransform;
    }

    void Update()
    {
        UpdateTransform();
    }

    void UpdateTransform()
    {
        if (hasFromTransform)
        {
            from = fromTransform.position;
        }
        to = transform.parent.position;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, to - from);
        Vector3 position = (from + to) / 2f;
        transform.SetPositionAndRotation(position, rotation);
        transform.Rotate(0, 0, 90);

        float scaleX = (from - to).magnitude;
        Vector2 tentacleSize = spriteRenderer.size;
        spriteRenderer.size = new Vector2(scaleX, tentacleSize.y);

        if (hasHead)
        {
            head.transform.localPosition = Vector3.right * (scaleX / 2f);
        }
    }
}
