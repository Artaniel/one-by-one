using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAppearOnStart : MonoBehaviour
{
    [SerializeField] Color color = Color.white;

    bool executeOnce = true;
    
    // Has to be called after "Start"
    void Update()
    {
        if (executeOnce)
        {
            executeOnce = false;
            GetComponent<TMPro.TextMeshPro>().color = color;
        }
    }
}
