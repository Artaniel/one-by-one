using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachGameObject : MonoBehaviour
{
    public bool toParentOfParent = true;

    void Awake()
    {
        if (toParentOfParent)
            transform.SetParent(transform.parent.parent);
        else
            transform.SetParent(null);
    }
}
