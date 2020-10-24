using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnailLoaderUI : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform parentRectTransform;
    private float speed;
    private float rightMostPoint;
    private float leftMostPoint;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
        speed = GetComponentInParent<CanvasScaler>().referenceResolution.x * 0.01f;
        leftMostPoint = parentRectTransform.rect.xMin;
        rightMostPoint = parentRectTransform.rect.xMax; // why? But it works
        print($"{leftMostPoint} {rightMostPoint}");
    }

    void Update()
    {
        var position = rectTransform.localPosition;
        position.x += Time.deltaTime * speed;
        if (position.x > rightMostPoint)
        {
            position.x = leftMostPoint;
        }
        rectTransform.localPosition = position;
    }
}
