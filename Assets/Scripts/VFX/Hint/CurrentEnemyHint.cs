using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CurrentEnemyHint : MonoBehaviour
{
    [SerializeField] protected float timeToHint = 3f;

    protected virtual void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        timeParameter += Time.deltaTime;
        UpdateVisual(timeParameter / timeToHint);
    }

    public void SetupHint(Transform parent)
    {
        if (parent == this.parent) return;
        timeParameter = 0;
        SetupHintVisual(parent);
    }

    protected abstract void SetupHintVisual(Transform parent);

    protected abstract void UpdateVisual(float timeFraction);

    protected float timeParameter = 0;
    protected Transform parent = null;
    new protected ParticleSystem particleSystem;
}
