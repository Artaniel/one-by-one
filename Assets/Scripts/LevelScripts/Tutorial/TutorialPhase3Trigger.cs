using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPhase3Trigger : MonoBehaviour
{
    private void OnTriggerEnter2D(UnityEngine.Collider2D other)
    {
        if (other.GetComponent<CharacterLife>()) {
            TutorialManager.instance.Phase3StartTrigger();
        }
    }
}
