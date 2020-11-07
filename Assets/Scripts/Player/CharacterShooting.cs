using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterShooting : MonoBehaviour
{
    public Transform weaponTip = null;

    [HideInInspector] public bool shotFrame = false; //flag for reactions on shot
    [HideInInspector] public SkillManager.EquippedWeapon currentWeapon;

    [SerializeField] private GameObject mouseCursorObj = null;
    new private Rigidbody2D rigidbody;

    [HideInInspector] public UnityEvent firstBulletShot = new UnityEvent();

    public void LoadNewWeapon(SkillManager.EquippedWeapon weapon, bool instant = false)
    {
        currentWeapon = weapon;
        timeBetweenAttacks = instant ? 0 : Mathf.Max(0.7f, weapon.logic.timeBetweenAttacks);
        weaponSwitchTime = timeBetweenAttacks;
        weaponTipDynamic.ChoosePosition(weapon.logic.weaponType);
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerAnim = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
        cameraShaker = mainCamera.GetComponent<CameraShaker>();
        gunfireAnimator = GetComponentInChildren<GunfireAnimator>();
        Cursor.visible = false;
        gameCursor = Instantiate(mouseCursorObj);
        skillManager = GetComponent<SkillManager>();
        weaponTipDynamic = weaponTip.GetComponent<WeaponTipDynamic>();
    }

    private void FixedUpdate()
    {
        RotateCharacterTowardsCursor();
    }

    private void Update()
    {
        if (Pause.Paused)
        {
            return;
        }

        shotFrame = false;

        weaponSwitchTime = Mathf.Clamp(weaponSwitchTime - Time.deltaTime, 0, 100);
        if (timeBetweenAttacks > 0)
        {
            timeBetweenAttacks -= Time.deltaTime;
        }
        else if (currentWeapon == null || currentWeapon.logic == null) return;
        else if (Input.GetButton("Fire1"))
        {
            Vector3 mousePos = Input.mousePosition;
            var ammoNeeded = currentWeapon.logic.AmmoConsumption();
            if (currentWeapon.ammoLeft >= ammoNeeded)
            {
                if (currentWeapon.ammoLeft == currentWeapon.logic.ammoMagazine) firstBulletShot.Invoke();

                timeBetweenAttacks = currentWeapon.logic.timeBetweenAttacks / attackSpeedMult;
                currentWeapon.reloadTimeLeft = 0;
                currentWeapon.ammoLeft -= ammoNeeded;
                currentWeapon.logic.Attack(this, mousePos);
                if (currentWeapon.logic is ShootingWeapon)
                {
                    var shootingWeapon = currentWeapon.logic as ShootingWeapon;
                    cameraShaker.ShakeCamera(shootingWeapon.GunfireDestructivePower());
                    gunfireAnimator.LightenUp(0.07f, maxPower: shootingWeapon.GunfirePower());
                    playerAnim.SetBool("Attacks", true);
                }
                
                shotFrame = true;
            }
            
            if (currentWeapon.ammoLeft == 0)
            {
                skillManager.ReloadWeaponIfNeeded(playSound: true);
                timeBetweenAttacks = 1f; // WARNING: MAGIC CONSTANT TO PREVENT PLAYER FROM FIRING WHEN HE STARTED RELOADING
            }
        }
        if (Input.GetKeyDown(reloadButton))
        {
            skillManager.ReloadWeaponIfNeeded(playSound: true);
        }
        if (!shotFrame)
        {
            playerAnim.SetBool("Attacks", false);
        }
    }

    private void RotateCharacterTowardsCursor()
    {
        Vector3 difference = mainCamera.ScreenToWorldPoint(Input.mousePosition)-transform.position;
        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        rigidbody.MoveRotation(Quaternion.Euler(0f, 0f, angle-90f));
    }

    public void AddToAttackSpeed(float addToAttackSpeedValue)
    {
        attackSpeedMult += addToAttackSpeedValue;
    }

    public bool IsSwitching()
    {
        return weaponSwitchTime > 0;
    }

    public bool CanShoot()
    {
        return currentWeapon != null && currentWeapon.ammoLeft > 0 && (timeBetweenAttacks <= 0 || currentWeapon.reloadTimeLeft <= 0);
    }
    
    public static Transform GetCursor() => gameCursor.transform;

    private float timeBetweenAttacks = 0;

    private Camera mainCamera;
    private CameraShaker cameraShaker;

    private float attackSpeedMult = 1f;

    private GunfireAnimator gunfireAnimator;

    private KeyCode reloadButton = KeyCode.R;
    private SkillManager skillManager;

    private WeaponTipDynamic weaponTipDynamic;

    private static GameObject gameCursor;

    private Animator playerAnim;

    private float weaponSwitchTime = 0;
}
