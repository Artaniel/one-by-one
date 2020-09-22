using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Paw Weapon", menuName = "ScriptableObject/Weapon/Predator Paw Weapon", order = 1)]
public class PredatorPaws : ShootingWeapon
{
    [Header("Paw-specific settings")]
    public float pushPower = 30f;

    public override void InitializeSkill()
    {
        base.InitializeSkill();
        rigidbody2D = Player.GetComponent<Rigidbody2D>();
    }

    public override void ShootingWeaponAttack(CharacterShooting attackManager, Transform shotFrom)
    {
        var mousePos = CharacterShooting.GetCursor().position;
        base.ShootingWeaponAttack(attackManager, shotFrom);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        rigidbody2D.velocity += ((Vector2)(mousePos - shotFrom.position)).normalized * pushPower;
    }

    protected Rigidbody2D rigidbody2D;
}
