using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Box : Container
{
    private void OnTriggerEnter2D(UnityEngine.Collider2D coll)
    {
        if (coll.TryGetComponent(out BulletLife bulletLife))
        {
            OnBulletHit();
        }
    }
 
    public void OnBulletHit() {
        //SFX/VFX? 
        Open();
        Destroy(gameObject);
    }
}
