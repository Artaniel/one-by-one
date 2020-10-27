using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLaserTurret : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f; //degree in sec
    [SerializeField] private EnemyLaser[] lasers = null;

    private void Awake()
    {
        foreach (EnemyLaser laser in lasers)
        {
            if (laser == null)
                Debug.Log("LaserTurret cant find laser");
            else
                laser.ShootStartDirection(laser.transform.position, transform.up);
        }
    }

    private void Start()
    {
        foreach (EnemyLaser laser in lasers)        
            laser.line.enabled = true;        
    }

    private void Update()
    {        
        if (!Pause.Paused) {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            foreach (EnemyLaser laser in lasers)            
                laser.UpdateLaser(laser.transform.position, transform.up);
        }
    }
}
