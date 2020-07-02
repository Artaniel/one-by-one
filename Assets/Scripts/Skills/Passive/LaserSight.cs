using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        if (!Pause.Paused && !CharacterLife.isDeath)
        {
            Vector3 gunPoint = player.GetComponent<CharacterShooting>().weaponTip.position;
            line.SetPosition(0, gunPoint);
            RaycastHit2D[] hits = Physics2D.RaycastAll(gunPoint, camera.ScreenToWorldPoint(Input.mousePosition) + (Vector3.forward * 20) - gunPoint, 100f);

            hits = (from t in hits
                    where t.transform.gameObject.tag == "Environment"
                    select t).ToArray();

            if (hits.Length > 0)
            {
                float minDist = 99999f;
                RaycastHit2D correctHit = hits[0];
                foreach (RaycastHit2D hit in hits)
                {
                    if (Vector3.Distance(player.transform.position, hit.point) < minDist)
                    {
                        minDist = Vector3.Distance(player.transform.position, hit.point);
                        correctHit = hit;
                    }
                }

                line.SetPosition(1, correctHit.point);
            }
            else
            {
                line.SetPosition(1, 
                    (Vector3.Normalize(camera.ScreenToWorldPoint(Input.mousePosition) + (Vector3.forward * 20f) - player.transform.position) * 100f) 
                    + camera.ScreenToWorldPoint(Input.mousePosition) + (Vector3.forward * 20));
                // backup variant, ignoring the wall if cant find it
            }
        }
        else {
            line.SetPosition(0, player.transform.position); // hide line
            line.SetPosition(1, player.transform.position);
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