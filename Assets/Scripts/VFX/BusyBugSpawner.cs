using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusyBugSpawner : MonoBehaviour
{
    public GameObject[] busyBugs;
    public ZoneScript[] zoneScripts;
    public ZoneScript busyBugDirection;
    public float spawnEverySeconds = 1f;
    public Vector2 sizeRange = new Vector2(2f, 3.5f);
    public Vector2 blurRange = new Vector2(0.1f, 0.2f);
    public Vector2 speedRange = new Vector2(420, 900);

    void Start()
    {
        foreach (var zone in zoneScripts)
        {
            zone.UseZone();
        }
        busyBugDirection.UseZone();
    }

    void Update()
    {
        if (timeToNextSpawn <= 0)
        {
            timeToNextSpawn = spawnEverySeconds;
            SpawnBusyBug();
        }
        timeToNextSpawn -= Time.deltaTime;
    }

    void SpawnBusyBug()
    {
        var randomPosition = zoneScripts[Random.Range(0, zoneScripts.Length)].RandomZonePosition();
        var busyBug = PoolManager.GetPool(busyBugs[Random.Range(0, busyBugs.Length)], randomPosition, Quaternion.identity);
        var vectorDirection = (Vector3)busyBugDirection.RandomZonePosition() - busyBug.transform.position;
        busyBug.transform.rotation = Quaternion.LookRotation(Vector3.forward, vectorDirection);

        // Visual block. Illusion of a sprite being in the air
        var spriteComps = busyBug.GetComponentsInChildren<SpriteRenderer>();
        var lerpParam = Random.value;
        var size = Mathf.Lerp(sizeRange.x, sizeRange.y, lerpParam);
        spriteComps[0].transform.localScale = new Vector3(size, size, size);
        foreach (var spriteComp in spriteComps)
        {
            spriteComp.material.SetFloat("_BlurAmount", Mathf.Lerp(blurRange.x, blurRange.y, lerpParam));
            busyBug.GetComponent<AIAgent>().maxSpeed = Mathf.Lerp(speedRange.x, speedRange.y, lerpParam);
            spriteComp.sortingOrder = 7 + (int)(lerpParam * 7);
        }
    }

    private float timeToNextSpawn = 0;
}
