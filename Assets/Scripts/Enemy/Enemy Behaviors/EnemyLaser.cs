using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    public LineRenderer line;
    private GameObject player;
    [SerializeField] private float laserWight = 0.1f;
    
    private Vector3 laserStartPos;
    private Vector3 laserEndPos;

    [SerializeField] private GameObject laserEndPrefab;
    private GameObject laserEndInstance = null;

    private void Awake()
    {
        if (line == null) line = GetComponent<LineRenderer>();
        if (line == null) Debug.LogError("Laser can't find LineRenderer");
        else line.enabled = false;
        player = GameObject.FindWithTag("Player");
    }

    public void ShootStart(Vector3 fromPosition, Vector3 toPosition) {
        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(0, fromPosition);
        line.SetPosition(1, toPosition);
        line.startWidth = laserWight;
        line.endWidth = laserWight;
        laserStartPos = fromPosition;
        laserEndPos = toPosition;

        if (TryGetComponent(out ParticleSystem particleSystem))
        {
            particleSystem.Play();
        }

        if (laserEndPrefab) {
            laserEndInstance = PoolManager.Instantiate(laserEndPrefab, laserEndPos, Quaternion.identity);
        }
    }

    public void ShootStartDirection(Vector3 fromPosition, Vector3 direction) {
        laserEndPos = GetLaserHitPoint(fromPosition, direction);
        ShootStart(fromPosition, laserEndPos);
    }

    private Vector3 GetLaserHitPoint(Vector3 fromPosition, Vector3 direction) {
        RaycastHit2D[] hits = Physics2D.RaycastAll(fromPosition, direction);
        float minDistance = Mathf.Infinity;
        Vector3 closeWallHitPoint = Vector3.zero;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "Environment")
            {
                if (Vector2.Distance(transform.position, hit.point) < minDistance) {
                    minDistance = Vector2.Distance(transform.position, hit.point);
                    closeWallHitPoint = hit.point;
                }
            }
        }
        return closeWallHitPoint;
    }

    public void UpdateLaser(Vector3 fromPosition, Vector3 direction) {
        if (line.enabled) {
            laserEndPos = GetLaserHitPoint(fromPosition, direction);
            line.SetPosition(0, fromPosition);
            line.SetPosition(1, laserEndPos);
            if (laserEndPrefab)
                laserEndInstance.transform.position = laserEndPos;
        }
    }

    public void ShootStop()
    {
        line.enabled = false;
        if (TryGetComponent(out ParticleSystem particleSystem))
        {
            particleSystem.Stop();
        }
        if (laserEndInstance)
            PoolManager.Destroy(laserEndInstance);
    }

    private void Update()
    {
        if (line.enabled && !Pause.Paused) {
            if (PlayerInTheRay())
            {
                player.GetComponent<CharacterLife>().Damage();                
            }
        }
    }

    private bool PlayerInTheRay()
    {
        bool result = false;
        RaycastHit2D[] hitArray = Physics2D.BoxCastAll(laserStartPos, new Vector2(laserWight,laserWight), 0, laserEndPos - laserStartPos, Vector3.Distance(laserEndPos, laserStartPos));
        foreach (RaycastHit2D hit in hitArray) {
            if (hit.collider.gameObject.tag == "Player") result = true;
        }
        return result; 
    }
}
