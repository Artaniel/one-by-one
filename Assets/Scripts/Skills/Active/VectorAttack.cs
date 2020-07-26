using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Vector Attack", menuName = "ScriptableObject/ActiveSkill/VectorAttack", order = 1)]
public class VectorAttack : ActiveSkill
{
    public GameObject aimPrefab;
    public GameObject attackPrefab;
    public float aimLength = 4f;

    public override void InitializeSkill()
    {
        base.InitializeSkill();
        cursor = CharacterShooting.GetCursor();
    }

    public override void ActivateSkill()
    {
        aimStart = cursor.transform.position;
        aimInstance = PoolManager.GetPool(aimPrefab, cursor.position, Quaternion.identity).GetComponent<LineRenderer>();
        aimInstance.SetPosition(0, aimStart);
    }

    public override void UpdateEffect()
    {
        base.UpdateEffect();
        var secondPointLimited = aimStart + Vector3.ClampMagnitude(cursor.position - aimStart, aimLength);
        aimInstance.SetPosition(1, secondPointLimited);
    }

    public override void EndOfSkill()
    {
        base.EndOfSkill();
        var attack = PoolManager.GetPool(attackPrefab, aimStart, Quaternion.LookRotation(Vector3.forward, cursor.position - aimStart));
        attack.transform.Rotate(0, 0, 90);
         
        aimInstance.SetPosition(0, Vector3.zero);
        aimInstance.SetPosition(1, Vector3.zero);
        PoolManager.ReturnToPool(aimInstance.gameObject);
    }

    private Vector3 aimStart;
    private LineRenderer aimInstance;
    private Transform cursor;
}
