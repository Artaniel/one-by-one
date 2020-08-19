using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorRandomFromList : MonoBehaviour
{
    public RuntimeAnimatorController[] animators;

    void Start()
    {
        var animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animators[Random.Range(0, animators.Length)];
    }
}
