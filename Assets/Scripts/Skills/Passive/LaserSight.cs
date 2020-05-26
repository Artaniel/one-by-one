using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LaserSight", menuName = "ScriptableObject/PassiveSkill/LaserSight", order = 11)]
public class LaserSight : PassiveSkill
{
    private bool active = false;
    private LineRenderer line;
    public GameObject laserRendererPrefab;
    private GameObject player;
    private Camera camera;

    public override void UpdateEffect()
    {
        line.SetPosition(0, player.transform.position);
        RaycastHit2D[] hits = Physics2D.RaycastAll(player.transform.position, camera.ScreenToWorldPoint(Input.mousePosition) + (Vector3.forward * 20) - player.transform.position, 100f);

        if (hits.Length > 1)
        {
            line.SetPosition(1, hits[1].point);//second point because first is player
        }
        else
        {
            line.SetPosition(1, (Vector3.Normalize(camera.ScreenToWorldPoint(Input.mousePosition) + (Vector3.forward * 20f) - player.transform.position)*100f) + camera.ScreenToWorldPoint(Input.mousePosition)+ (Vector3.forward * 20));  
            // backup variant, ignoring the wall if cant find it
        }
    }

    public override void InitializeSkill()
    {        
        player = GameObject.FindWithTag("Player");
        camera = Camera.main;
        if (line == null)
        {
            line = Instantiate(laserRendererPrefab).GetComponent<LineRenderer>();
            line.transform.parent = player.transform;
        }
    }

}
