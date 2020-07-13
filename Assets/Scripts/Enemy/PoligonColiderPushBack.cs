using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoligonColiderPushBack : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private float reactionDistance = 1f;
    [SerializeField] private float pushBackSpeed = 10f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(player.transform.position,reactionDistance,Vector3.forward,0f);
        foreach (RaycastHit2D hit in hits) {
            if (hit.collider.gameObject == gameObject) {
                Debug.DrawRay(hit.point, hit.normal, Color.red);
                player.GetComponent<Rigidbody2D>().velocity = hit.normal * pushBackSpeed;
            }
        }
    }
}
