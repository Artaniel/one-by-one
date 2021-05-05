using System.Collections;
using System.Collections.Generic;
using Game.Events;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.Events;

public class CharacterLife : MonoBehaviour
{
    public static bool isDeath = false;
    [SerializeField] private GameObject ShadowObject = null;
    [SerializeField] private GameObject hitEffect = null;
    public GameObject dummyPlayerPrefab = null;
    new private AudioSource audio;
    [SerializeField] private AudioClip[] damageSounds = null;

    [HideInInspector]public bool dashActiveSkill;
    [HideInInspector] public UnityEvent hpChangedEvent = new UnityEvent();
    [HideInInspector] public UnityEvent playerHitEvent = new UnityEvent();

    [SerializeField]
    private int maxHp = 4;

    public static GameObject player;

    public void Awake()
    {
        player = gameObject;
    }

    public void Start()
    {
        hp = maxHp;
        dashActiveSkill = false;
        isDeath = false;
        hpUIs = GameObject.FindGameObjectWithTag("Canvas").GetComponentsInChildren<PlayerHPIcon>();
        hpChangedEvent.AddListener(UpdateHPUI);
        UpdateHPUI();
        cameraShaker = Camera.main.GetComponent<CameraShaker>();
    }

    public void Damage(int damage = 1)
    {
        if (isDeath || invulTimeLeft > 0 || dashActiveSkill) return; // Already died or invul
        
        hp -= damage;
        PlayerHitVFX();
        hpChangedEvent.Invoke();
        playerHitEvent.Invoke();

        if (damageSounds.Length != 0)
        {
            var clip = damageSounds[Random.Range(0, damageSounds.Length)];
            AudioManager.Play(clip);
        }
        

        if (hp <= 0)
        {
            LogicDeathBlock();
            VisualDeathBlock();
        }
        else
        {
            AddTemporaryInvulnurability();
        }
    }

    private void Update()
    {
        if (invulTimeLeft > 0)
        {
            invulTimeLeft -= Time.deltaTime;
            if (invulTimeLeft <= 0)
            {
                GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }

        if (isDeath)
        {
            GameObject pressFGUI = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(1).gameObject;
            if (!pressFGUI.activeSelf)
                pressFGUI.SetActive(true);
        }
    }

    private void LogicDeathBlock()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        CharacterShooting shooting = GetComponent<CharacterShooting>();
        CharacterMovement movement = GetComponent<CharacterMovement>();
        if (movement)
            movement.enabled = false;
        //if (collider2D)
        //    collider2D.isTrigger = true;
        if (shooting)
            shooting.enabled = false;
        isDeath = true;
        audio = GetComponent<AudioSource>();
        AudioManager.PauseSource("Walk", audio);
        EventManager.OnMoneyChange.Invoke((int) (-MoneyManager.MoneyAmount * 0.05f));
        Metrics.OnDeath();
    }

    private void VisualDeathBlock()
    {
        lighter = GetComponentInChildren<Light2D>();
        glowIntense = lighter.intensity;

        mainCam = Camera.main;
        mainCam.GetComponent<Cinemachine.CinemachineBrain>().enabled = false;
        cameraScale = mainCam.orthographicSize;
        cameraStartPosition = mainCam.gameObject.transform.position;
        cameraMovePosition = gameObject.transform.position + new Vector3(0, 0, -20);

        StartCoroutine(StopGlow());

        GetComponentInChildren<Animator>().Play("Death");
        ShadowObject.GetComponent<Animator>().Play("Death");

        transform.eulerAngles = new Vector3(0, 0, 180);
        
        circleCollider.radius = 1.35f;
        GetComponent<Rigidbody2D>().drag = 10;
        GetComponent<Rigidbody2D>().mass = 100;

        GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;

        //var CameraFollow = Camera.main.GetComponent<CameraFollowScript>();
        //if (!CameraFollow) Camera.main.gameObject.AddComponent<CameraFollowScript>();
    }

    private IEnumerator StopGlow()
    {
        while (glowFadeTime >= 0)
        {
            glowFadeTime -= Time.fixedDeltaTime;
            lighter.intensity = Mathf.Lerp(0, glowIntense, glowFadeTime);
            
            // Also move camera "forward" together with glow fadeout
            mainCam.orthographicSize = Mathf.Lerp(cameraScale / 2, cameraScale, glowFadeTime);
            cameraMovePosition = gameObject.transform.position + new Vector3(0, 0, -20);
            mainCam.gameObject.transform.position = Vector3.Lerp(cameraMovePosition, cameraStartPosition, glowFadeTime);

            //ShadowObject.transform.localEulerAngles = 

            yield return new WaitForFixedUpdate();
        }
        circleCollider.radius = 0.4f;
        while (isDeath) { // still tracking camera after zoom
            mainCam.gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0, -20);
            yield return new WaitForFixedUpdate();
        }
    }

    public void Alive()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        CharacterShooting shooting = GetComponent<CharacterShooting>();
        CharacterMovement movement = GetComponent<CharacterMovement>();
        if (movement)
            movement.enabled = false;
        if (circleCollider)
            circleCollider.isTrigger = true;
        if (shooting)
            shooting.enabled = false;
        isDeath = false;
    }

    public void Heal(int healAmmount) {
        if (!isDeath) {
            hp += healAmmount;
            if (hp > maxHp) hp = maxHp;
            hpChangedEvent.Invoke();
        }
    }

    public int GetHp()
    {
        return hp;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    public float GetHpDropChanceAmplifier()
    {
        return HPDropChanceAmplifier;
    }

    public void AddToHPDropChanceAmp(float addValue)
    {
        HPDropChanceAmplifier += addValue;
    }

    private void AddTemporaryInvulnurability()
    {
        // visualise invulnururability ?
        invulTimeLeft = invulTime;
        GetComponentInChildren<SpriteRenderer>().color = Color.red;
    }

    private void UpdateHPUI()
    {
        hpUIs[0].UpdateHP(hp, maxHp);
        hpUIs[1].UpdateHP(hp, maxHp);
    }

    public void PlayerHitVFX()
    {
        if (hitEffect)
        {
            var hitEff = PoolManager.GetPool(hitEffect, transform.position, Quaternion.identity);
            PoolManager.ReturnToPool(hitEff, 5f);
            hitEff.GetComponent<PlayerDamagedVFX>().player = transform;
        }
        cameraShaker.ShakeCamera(2, 0.5f);
    }

    public void HidePlayer()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        savedSpriteColors = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = false;
        }
        var lights = GetComponentsInChildren<Light2D>();
        foreach (var light in lights)
        {
            light.enabled = false;
        }
    }

    public void RevealPlayer()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = true;
        }

        var lights = GetComponentsInChildren<Light2D>();
        foreach (var light in lights)
        {
            light.enabled = true;
        }
    }

    private float HPDropChanceAmplifier = 1f;

    private int hp;
    
    private float invulTime = 0.8f;
    private float invulTimeLeft = 0;

    private CircleCollider2D circleCollider;

    // Light
    private Light2D lighter;
    private float glowIntense;
    private float glowFadeTime = 3;

    //Camera
    private Camera mainCam;
    private CameraShaker cameraShaker;
    private float cameraScale;
    private Vector3 cameraStartPosition;
    private Vector3 cameraMovePosition;

    private PlayerHPIcon[] hpUIs;

    private SpriteRenderer[] sprites;
    private Color[] savedSpriteColors;
}
