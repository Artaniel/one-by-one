using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerController : MonoBehaviour
{
    public float speed;
    private Vector3 destination = Vector3.zero;

    public void SetDestination(Vector3 destination)
    {
        speed = (destination - transform.position).magnitude / 0.35f;
        this.destination = destination;
        //RotateInstantlyTowardsTarget(destination);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, destination) > 0.05f)
        {
            transform.Translate((destination - transform.position).normalized * speed * Time.deltaTime, Space.World);
        }
        RotateCharacterTowardsCursor();
    }

    private void RotateInstantlyTowardsTarget(Vector3 destination)
    {
        Vector2 direction = transform.position - destination;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }

    private void RotateCharacterTowardsCursor()
    {
        var mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Quaternion rot = Quaternion.LookRotation(transform.position - mousepos, Vector3.forward);
        transform.eulerAngles = new Vector3(0, 0, rot.eulerAngles.z);
        Vector3 vectorToTarget = transform.position - mousepos;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }
}