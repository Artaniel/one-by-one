using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningTree : MonoBehaviour
{
    public Material burningMat;
    public Material burntMat;

    public void Burn()
    {
        GetComponent<SpriteRenderer>().material = burningMat;
    }

    public void FinishBurning()
    {
        GetComponent<SpriteRenderer>().material = burntMat;
    }
}
