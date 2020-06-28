using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SniperScope", menuName = "ScriptableObject/PassiveSkill/SniperScope", order = 1)]
public class SniperScope : PassiveSkill
{
    public float scopeIncreasePerSecond = 1f;
    public float scopePower = 2f;

    public override void InitializeSkill()
    {
        base.InitializeSkill();
        cameraScript = Camera.main.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
    }

    public override void UpdateEffect()
    {
        base.UpdateEffect();
        float delta = Time.deltaTime * scopeIncreasePerSecond;
        if (scopeParameter + delta < scopePower)
        {
            scopeParameter += delta;
            cameraScript.m_Lens.OrthographicSize += delta;
        }
    }

    private Cinemachine.CinemachineVirtualCamera cameraScript = null;
    private float scopeParameter = 0;
}
