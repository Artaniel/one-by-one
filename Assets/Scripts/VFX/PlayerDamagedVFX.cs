using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamagedVFX : MonoBehaviour
{
    [SerializeField] private float animationSpeed = 5f;
    private List<Transform> evilTrails = new List<Transform>();
    private List<SpriteRenderer> evilEntity = new List<SpriteRenderer>();

    private List<float> selfSpeedMult = new List<float>();
    public Transform player = null;

    // Start is called before the first frame update
    void Awake()
    {
        var childCnt = transform.childCount;
        for (int i = 0; i < childCnt; i++)
        {
            evilTrails.Add(transform.GetChild(i));
            evilEntity.Add(evilTrails[i].GetComponent<SpriteRenderer>());
        }
    }

    void OnEnable()
    {
        timePassed = 0;
        selfSpeedMult.Clear();
        var childCnt = transform.childCount;
        for (int i = 0; i < childCnt; i++)
        {
            selfSpeedMult.Add(1f + Random.Range(-0.5f, 0.5f));
            evilTrails[i].localRotation = Quaternion.Euler(0, 0, 360f / childCnt * i);
            evilTrails[i].position = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= 7 / animationSpeed) return;

        for (int i = 0; i < evilTrails.Count; i++)
        {
            var trail = evilTrails[i];
            trail.Translate(trail.up * Time.deltaTime * animationSpeed * 2 * selfSpeedMult[i], Space.World);
            var trailAngles = trail.eulerAngles;
            trailAngles.z += (50 * Time.deltaTime * animationSpeed * selfSpeedMult[i]) + (5 * Mathf.Sin(20 * Time.time));
            trail.eulerAngles = trailAngles;
            trail.Translate(-(trail.position - player.position) * Time.deltaTime, Space.World);
        }
        foreach (var entity in evilEntity)
        {
            var newc = entity.color;
            newc.a -= Time.deltaTime * animationSpeed / 5;
            entity.color = newc;
        }
    }

    private float timePassed = 0;
}
