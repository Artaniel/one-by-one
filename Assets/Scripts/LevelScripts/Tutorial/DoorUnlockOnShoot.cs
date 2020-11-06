using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlockOnShoot : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.GetComponent<BulletLife>())
        {
            TutorialManager.instance.DoorStage0Hit();
        }
    }
}
