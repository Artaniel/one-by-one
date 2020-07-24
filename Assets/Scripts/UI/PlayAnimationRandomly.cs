using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationRandomly : MonoBehaviour
{
    public Vector2 timeInterval = new Vector2(15f, 35f);
    public string animationName = "UNKNOWN";

    void OnEnable()
    {
        timeToAnim = Random.Range(timeInterval.x, timeInterval.y);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToAnim <= 0)
        {
            timeToAnim = Random.Range(timeInterval.x, timeInterval.y);
            animator.Play(animationName);
        }
        timeToAnim -= Time.deltaTime;
    }

    private float timeToAnim;
    private Animator animator;
}
