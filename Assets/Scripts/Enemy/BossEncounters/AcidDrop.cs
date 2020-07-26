using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidDrop : MonoBehaviour
{
    public Vector3 targetPosition;
    public float velocity;
    private bool isMoving = false;

    private void Update()
    {
        if (isMoving && !Pause.Paused) {
            if (Vector3.Distance(transform.position, targetPosition) > velocity * Time.deltaTime)            
                transform.position += (targetPosition - transform.position).normalized * velocity * Time.deltaTime;
            else {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    public void StartMove() {
        isMoving = true;
    }
}
