using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidSpray : MonoBehaviour
{
    [SerializeField] private float sprayZoneHeight = 10; //h
    [SerializeField] private float sprayZoneWidth = 2; //w
    [SerializeField] private float dropsNumber = 30; //N
    [SerializeField] private float velocityNormal = 10; //V
    [SerializeField] private float velocityFluctoation = 10; //Vd

    [SerializeField] private GameObject[] dropsPrefabs = null;
    [SerializeField] private int blueZonesNumber = 2; //N2
    [SerializeField] private float blueZonesHight = 3f; //h2

    private List<GameObject> pool, poolReserve, poolActive;
    private float[] blueZonesPositions;

    private void Awake()
    {
        InitPool();
        SetBlueZones();
        //LaunchSpray(); // не забыть убрать, не должно запускаться при старте
    }

    private void InitPool()
    {
        pool = new List<GameObject>();
        poolReserve = new List<GameObject>();
        poolActive = new List<GameObject>();
        float[] probabilitysOfPrefabs = new float[dropsPrefabs.Length];
        float summ = 0;
        for (int i = 0; i < probabilitysOfPrefabs.Length; i++)
        { // get random numbers for [X,Y,Z]
            probabilitysOfPrefabs[i] = Random.Range(0f, 1f);
            summ += probabilitysOfPrefabs[i];
        }
        for (int i = 0; i < probabilitysOfPrefabs.Length; i++) // change to probabilities like [X/(X+Y+Z), Y/(X+Y+Z),...]
            probabilitysOfPrefabs[i] /= summ;
        float[] probabilitySteps = new float[dropsPrefabs.Length + 2];
        probabilitySteps[0] = 0;
        probabilitySteps[probabilitySteps.Length - 1] = 1;
        for (int i = 0; i < dropsPrefabs.Length; i++) // change to steps pf [robability, like [X, X+Y, X+Y+Z]
            probabilitySteps[i + 1] = probabilitySteps[i] + probabilitysOfPrefabs[i];

        GameObject currentDrop, currentPrefab;
        int j; // index for second loop
        float p; // probability roll
        for (int i = 0; i < dropsNumber; i++)
        {
            p = Random.Range(0f, 1f);
            j = 0;
            while (!(probabilitySteps[j] <= p && probabilitySteps[j + 1] >= p) && j < probabilitySteps.Length - 1)
            {
                j++;
            }
            currentPrefab = dropsPrefabs[j];
            currentDrop = Instantiate(currentPrefab);
            currentDrop.SetActive(false);
            currentDrop.transform.parent = transform; // to clear them
            pool.Add(currentDrop);
            poolReserve.Add(currentDrop);
        }
    }

    public void LaunchSpray() // sould be called from external source
    {
        SetupDrops();
    }

    private void SetBlueZones() {
        blueZonesPositions = new float[blueZonesNumber];
        float randomPosition = 0; bool overlap; int brakeCounter = 0;
        if (blueZonesNumber * blueZonesHight >= sprayZoneHeight)
            Debug.LogError("Spray zone setup error. Blue zones total bigger than spray zone.");
        else
        {
            for (int i = 0; i < blueZonesNumber; i++)
            {
                overlap = true;
                while (overlap && (brakeCounter < 1000))
                {
                    brakeCounter++;
                    randomPosition = Random.Range(blueZonesHight / 2f, sprayZoneHeight - (blueZonesHight / 2f));
                    overlap = false;
                    if (i > 0)
                        for (int j = 0; j < i; j++)
                        {
                            if (Mathf.Abs(blueZonesPositions[j] - randomPosition) < blueZonesHight)
                            {
                                overlap = true;
                                j = i + 2; // brake loop
                            }
                        }
                }
                if (brakeCounter == 1000) Debug.LogError("Error in AcidSpray, 1st loop. Not enough space to place all blue zones");
                blueZonesPositions[i] = randomPosition;
            }
        }
    }

    private void SetupDrops() {
        bool overlap; float randomPosition = 0; int brakeCounter = 0; Vector2 targetPos;
        foreach (GameObject drop in pool)
        {
            drop.SetActive(true);
            drop.transform.position = transform.position;
            drop.transform.parent = null; // set free from parent rotation
            drop.GetComponent<AcidDrop>().velocity = velocityNormal + (velocityFluctoation * Random.Range(-1f, 1f));

            overlap = true;
            while (overlap && (brakeCounter < 1000))
            {
                overlap = false;
                randomPosition = Random.Range(0f, sprayZoneHeight);
                foreach (float blueZone in blueZonesPositions)
                    if (Mathf.Abs(randomPosition - blueZone) < blueZonesHight / 2f) // if in blue zone
                        overlap = true;
            }
            if (brakeCounter == 1000) Debug.LogError("Error in AcidSpray, 2nd loop. Probably no space between blue zones available.");
            targetPos = transform.position + (transform.up * randomPosition) + (transform.right * Random.Range(-0.5f, 0.5f) * sprayZoneWidth);
            drop.GetComponent<AcidDrop>().targetPosition = targetPos;
            drop.GetComponent<AcidDrop>().StartMove();
            poolReserve.Remove(drop);
            poolActive.Add(drop);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        DebugDraw();
#endif
    }

    private void DebugDraw() {
        ///green rectangle for spray zone
        Debug.DrawLine(transform.position + (transform.right * sprayZoneWidth / 2), transform.position - (transform.right * sprayZoneWidth / 2), Color.green);
        Debug.DrawLine(transform.position + (transform.right * sprayZoneWidth / 2), transform.position + (transform.right * sprayZoneWidth / 2) + (transform.up * sprayZoneHeight), Color.green);
        Debug.DrawLine(transform.position - (transform.right * sprayZoneWidth / 2), transform.position - (transform.right * sprayZoneWidth / 2) + (transform.up * sprayZoneHeight), Color.green);
        Debug.DrawLine(transform.position + (transform.right * sprayZoneWidth / 2) + (transform.up * sprayZoneHeight), 
            transform.position - (transform.right * sprayZoneWidth / 2) + (transform.up * sprayZoneHeight), Color.green);
        
        foreach (var blueZonePosition in blueZonesPositions)
        {//blue rectangle for blue zone
            Debug.DrawLine(transform.position + (transform.up * (blueZonePosition - blueZonesHight / 2)) + (transform.right * sprayZoneWidth / 2),
                transform.position + (transform.up * (blueZonePosition - blueZonesHight / 2)) - (transform.right * sprayZoneWidth / 2), Color.blue);
            Debug.DrawLine(transform.position + (transform.up * (blueZonePosition + blueZonesHight / 2)) + (transform.right * sprayZoneWidth / 2),
                transform.position + (transform.up * (blueZonePosition + blueZonesHight / 2)) - (transform.right * sprayZoneWidth / 2), Color.blue);
            Debug.DrawLine(transform.position + (transform.up * (blueZonePosition - blueZonesHight / 2)) + (transform.right * sprayZoneWidth / 2),
                transform.position + (transform.up * (blueZonePosition + blueZonesHight / 2)) + (transform.right * sprayZoneWidth / 2), Color.blue);
            Debug.DrawLine(transform.position + (transform.up * (blueZonePosition - blueZonesHight / 2)) - (transform.right * sprayZoneWidth / 2),
                transform.position + (transform.up * (blueZonePosition + blueZonesHight / 2)) - (transform.right * sprayZoneWidth / 2), Color.blue);
        }
    }
}
