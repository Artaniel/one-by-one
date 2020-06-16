using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentEnemyHint : MonoBehaviour
{
    [SerializeField] private float timeToHint = 3f;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (parent)
        {
            timeParameter += Time.deltaTime;
            transform.position = parent.position;
            UpdateVisual(timeParameter / timeToHint);
        }
    }

    public virtual void SetupHint(Transform parent)
    {
        particleSystem.Stop();
        timeParameter = 0;

        this.parent = parent;
    }

    protected virtual void UpdateVisual(float timeFraction)
    {
        if (timeFraction >= 1 && !particleSystem.isPlaying)
        {
            particleSystem.Play();
        }
    }

    private float timeParameter = 0;
    private Transform parent = null;
    new private ParticleSystem particleSystem;
}
