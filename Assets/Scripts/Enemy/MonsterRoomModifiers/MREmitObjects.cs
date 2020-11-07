using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnknownEmitMod", menuName = "ScriptableObject/MRMods/EmitMod", order = 1)]
public class MREmitObjects : MonsterRoomModifier
{
    [SerializeField] private Vector2 emitCooldown = new Vector2(0.1f, 0.25f);
    [SerializeField] private float probability = 0.5f;
    [SerializeField] private GameObject objectToSpawn = null;
    [SerializeField] private GameObject infusedVFX = null;

    public override void ApplyModifier(MonsterLife monster)
    {
        base.ApplyModifier(monster);
        if (Random.Range(0, 1f) <= probability)
        {
            var comp = monster.gameObject.AddComponent<TimedShootWithOffset>();
            comp.bullet = objectToSpawn;
            comp.randomShotAngle = 360f;
            comp.proximityCheckOption = new List<AIAgent.ProximityCheckOption>() { AIAgent.ProximityCheckOption.Always };
            comp.SetCooldownRange(emitCooldown);
            comp.castTime = 0;
            comp.shouldShift = false;
            Instantiate(infusedVFX, monster.transform);
        }
    }
}
