using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyInSpecificVersion : MonoBehaviour
{
    public bool onlyInBrowser = true;
    
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL
#else
        if (onlyInBrowser) {
            gameObject.SetActive(false);
        }
#endif
    }
}
