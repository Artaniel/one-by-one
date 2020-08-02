using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessDebugWeight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ppV = GetComponent<PostProcessVolume>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightBracket))
        {
            ppV.weight = Mathf.Clamp01(ppV.weight + Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftBracket))
        {
            ppV.weight = Mathf.Clamp01(ppV.weight - Time.deltaTime);
        }
    }

    PostProcessVolume ppV;
}
