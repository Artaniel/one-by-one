﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Experimental.Rendering.LWRP;

public class RoomLighting : MonoBehaviour
{
    // Swamp = enemy spawner VFX
    [SerializeField]
    private Material swampMatPrefab = null;
    [SerializeField, Tooltip("Leave empty if not needed")]
    private GameObject swampPrefab = null;

    [SerializeField] bool StandartLightIncrease = true;
    [SerializeField] private float maxvalue = 1;
    [SerializeField] private float roomClearedLight = 0.8f;
    public float DefaultLight = 0.13f;

    private void Awake()
    {
        SetSceneLight();
    }

    /// <summary>
    /// The function changes the "light" parameter that
    /// is later used to calculate scene lighting as
    /// a current to maximum percentage
    /// </summary>
    /// <param name="val">Value to add to light</param>
    public void AddToLight(float val, bool automatic = true)
    {
        if (automatic != StandartLightIncrease) return;
        TotalValue = TotalValue + val;
        RecalculateLight();
    }

    private void AddOneToLight()
    {
        AddToLight(1);
    }

    private void RecalculateLight()
    {
        previousLight = Light;
        Light = Mathf.Lerp(DefaultLight, roomClearedLight, Mathf.Pow(Mathf.Clamp01(TotalValue / maxvalue), 1.3f));
        t = 0.0f;
    }

    private void Update()
    {
        CurrentVal = Mathf.Lerp(previousLight, Light, t / maxT);

        NewLight(CurrentVal);
        if (swampInstance) NewSwampLight();
        
        t += Time.deltaTime;
    }

    private void NewLight(float light)
    {
        sceneLight.intensity = light;
    }

    /// <summary>
    /// Swamp initialization
    /// </summary>
    private void SetSwampMaterial()
    {
        swampMat = new Material(swampMatPrefab);
        if (Labirint.instance != null)
        { // in labirint mode, to delete with parent
            var borders = GetComponent<Room>()?.GetBordersFromTilemap();
            if (borders != null)
            {
                if (Mathf.Abs(borders[Direction.Side.LEFT] - borders[Direction.Side.RIGHT]) < 46 &&
                    Mathf.Abs(borders[Direction.Side.UP] - borders[Direction.Side.DOWN]) < 30)
                {
                    swampInstance = Instantiate(swampPrefab);
                    swampInstance.transform.parent = transform;
                    swampInstance.transform.localPosition = Vector3.zero;
                }
            }
        }
        else
        {
            swampInstance = Instantiate(swampPrefab);
        }
        if (swampInstance != null)
        {
            var sprites = swampInstance.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sprite in sprites)
            {
                sprite.sharedMaterial = swampMat;
            }
            var emitters = swampInstance.GetComponentsInChildren<ParticleSystemRenderer>();
            foreach (var emitter in emitters)
            {
                emitter.sharedMaterial = swampMat;
            }
        }
    }
    
    private void NewSwampLight()
    {
        var alpha = Mathf.InverseLerp(roomClearedLight, DefaultLight, CurrentVal);
        var color = swampMat.color;
        color.a = alpha;
        swampMat.color = color;
        if (alpha == 0) Destroy(swampInstance);
    }

    public void LabirintRoomEnterDark(int enemyCount)
    {
        if (swampPrefab)
        {
            SetSwampMaterial();
        }

        enabled = true;
        maxvalue = enemyCount + 1;
        TotalValue = 1;

        RecalculateLight();
        previousLight = sceneLight.intensity;
    }
    
    // Тоже работает на костылях. 06.06 был плохой день для программирования. Однако, 
    // мне очень хотелось завершить это дело. В ход пошли нелегальные действия
    public void LightsOut()
    {
        enabled = true;
        maxvalue = 1;
        TotalValue = 0;

        var savedDefaultLight = DefaultLight;
        DefaultLight = 0;
        RecalculateLight();
        previousLight = sceneLight.intensity;
        DefaultLight = savedDefaultLight;
    }

    // В данном случае освещение работает на костыле. Берется default light как минимум света
    // Когда будет готов плавный переход персонажа, выпилить к чертям
    // В светлых комнатах максимум достигается быстро
    public void LabirintRoomEnterBright()
    {
        enabled = true;
        maxvalue = 1;
        TotalValue = 1;

        RecalculateLight();
        previousLight = sceneLight.intensity;
    }

    public void LabirintRoomAddLight()
    {
        AddToLight(1);
    }

    public void SetSceneLight() {
        if (sceneLight == null) {
            var arena = GetComponent<ArenaEnemySpawner>();
            sceneLight = Labirint.instance.GetComponentInChildren<Light2D>();
            if (Labirint.instance && !Labirint.instance.OneRoomMode)
            {
                previousLight = sceneLight.intensity;
            }
            else // initial light
            {
                Light = DefaultLight;
                if (arena) maxvalue = arena.EnemyCount() - 1;

                if (swampPrefab) SetSwampMaterial();

                MonsterLife.OnEnemyDead.AddListener(AddOneToLight);
            }

            if (StandartLightIncrease) RecalculateLight();
        }
    }

    private const float maxT = 0.65f;

    private float previousLight = 0;
    private float TotalValue = 0;
    private float CurrentVal;
    private float t = 0.0f;
    private float Light;

    private GameObject swampInstance;
    private Material swampMat;

    private Light2D sceneLight;
}
