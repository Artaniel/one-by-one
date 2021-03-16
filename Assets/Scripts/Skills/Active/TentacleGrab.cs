using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TentacleGrabSkill", menuName = "ScriptableObject/ActiveSkill/TentacleGrab", order = 1)]
public class TentacleGrab : ActiveSkill
{
    public GameObject tentaclePrefab;
    public float damage = 5f;
    public float tentacleMovespeed = 12f;

    public override void InitializeSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void ActivateSkill()
    {
        cursor = CharacterShooting.GetCursor();

        playerPosition = player.transform.position;
        tentacle = PoolManager.GetPool(tentaclePrefab, playerPosition, Quaternion.identity);
        tentaclePosition = tentacle.transform.position;
        spriteRenderer = tentacle.GetComponent<SpriteRenderer>();
        coll = tentacle.GetComponent<BoxCollider2D>();
        tentacleGrabber = tentacle.GetComponent<TentacleGrabber>();
        tentacleGrabber.SetDamage(damage);

        UpdateTransform();
    }

    public override void UpdateEffect()
    {
        if (tentacleGrabber.broken) InterruptSkill();
        else UpdateTransform();
    }

    private void UpdateTransform()
    {
        var vectorTo = cursor.position;

        Vector3 newPlayerPosition = player.transform.position;
        Vector3 playerDelta = newPlayerPosition - playerPosition;
        tentaclePosition = tentaclePosition + playerDelta;
        playerPosition = newPlayerPosition;

        Vector3 desiredPosition = vectorTo;
        Vector3 nextPosition = Vector3.MoveTowards(tentaclePosition, desiredPosition, tentacleMovespeed / 2f * Time.deltaTime);

        Quaternion nextRotation = Quaternion.LookRotation(Vector3.forward, nextPosition - playerPosition);

        tentacle.transform.SetPositionAndRotation(nextPosition, nextRotation);
        tentaclePosition = nextPosition;
        tentacle.transform.Rotate(0, 0, 90);

        float scaleX = (tentaclePosition - player.transform.position).magnitude * 2f;
        Vector2 tentacleSize = spriteRenderer.size;
        spriteRenderer.size = new Vector2(scaleX, tentacleSize.y);

        tentacle.transform.GetChild(0).localPosition = new Vector3(scaleX / 2f - 0.5f, 0, 0);
    }

    public override void EndOfSkill()
    {
        PoolManager.ReturnToPool(tentacle);
    }

    Vector3 tentaclePosition;
    GameObject player;
    Vector3 playerPosition;
    GameObject tentacle;
    SpriteRenderer spriteRenderer;
    BoxCollider2D coll;
    Transform cursor;
    TentacleGrabber tentacleGrabber;
}
