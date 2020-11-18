using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEnemyHint : CurrentEnemyHint
{
    protected override void SetupHintVisual(Transform parent)
    {
        particleSystem.Stop();
        this.parent = parent;
    }

    protected override void UpdateVisual(float timeFraction)
    {
        if (parent)
        {
            transform.position = parent.position;
            if (timeFraction >= 1 && !particleSystem.isPlaying)
            {
                particleSystem.Play();
            }
        }
    }
}
