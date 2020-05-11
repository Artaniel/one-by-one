using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfSpawningContainer : Chest
{
    protected override void Start()
    {
        if (Labirint.instance.blueprints[Labirint.instance.currentRoomID].containerWasOpened)
        {
            Destroy(gameObject);
        }
        base.Start();
    }
}
