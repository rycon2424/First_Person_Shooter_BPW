using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayer : Actor
{
    [Header("Player Settings")]
    [SerializeField] float moveSpeed = 5f;  // THE PLAYERS MOVEMENTSPEED
    [SerializeField] public float mouseSensitivityX = 100f;
    [SerializeField] public float mouseSensitivityY = 100f;
    [SerializeField] public float gravity = -5f;
    public GameObject weaponHands;
    public Animator hands;

    [Header("Combat Settings")]
    public Weapon currentWeapon;
    public Weapon primary;
    public Weapon secondary;
    public List<Weapon> weaponsAvailable = new List<Weapon>();
    public bool reloading;

    [Header("RayCast Settings")]
    [SerializeField] bool useRayCasting;    // ENABLED THE USE OF RAYCAST IN THE FPSCAMERA SCRIPT
    public float rangeCameraRay = 4;

    private int footStepInterval;
    FPSCamera FPSCamera;
    UserInterfaceManager uim;
    CharacterController cc;
    Enemy[] enemiesOnMap;

    void Start()
    {
        FPSCamera = FindObjectOfType<FPSCamera>();  // FINDING THE FPSCAMERA SCRIPT TO ACCESS THE RAYCAST FUNCTION
        cc = GetComponent<CharacterController>();
        uim = GetComponent<UserInterfaceManager>();
        enemiesOnMap = FindObjectsOfType<Enemy>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (currentWeapon)
        {
            weaponHands.SetActive(true);
            currentWeapon.gameObject.SetActive(true);
            UpdateUI();
        }
        else
        {
            weaponHands.SetActive(false);
        }
    }

    void Update()
    {
        if (useRayCasting)
        {
            FPSCamera.Raycast();
        }
        if (currentWeapon)
        {
            Combat();
        }
        Move();
    }

    private void Move() // THE MOVEMENT OF THE CHARACTER WITH WASD AND ARROW KEYS
    {
        Vector3 ySpeed;
        Vector3 xSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && reloading == false)
        {
            hands.SetBool("Sprinting", true);
            ySpeed = (transform.forward * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
            xSpeed = (transform.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime);
        }
        else
        {
            hands.SetBool("Sprinting", false);
            ySpeed = (transform.forward * Input.GetAxis("Vertical") * (moveSpeed / 3) * Time.deltaTime);
            xSpeed = (transform.right * Input.GetAxis("Horizontal") * (moveSpeed / 3) * Time.deltaTime);
        }

        Vector3 gravityCalculation = new Vector3(0, gravity, 0);

        Vector3 motion = ySpeed + xSpeed + gravityCalculation;

        cc.Move(motion);

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (hands.GetBool("Sprinting") == true)
        {
            footStepInterval++;
            if (footStepInterval > 12)
            {
                footSource.clip = footsteps[Random.Range(0, footsteps.Length)];
                footSource.Play();
                footStepInterval = 0;
            }
        }
    }

    void Combat()
    {
        if (reloading)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!currentWeapon.primary)
            {
                if (primary)
                {
                    SwitchWeapon(primary);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentWeapon.primary)
            {
                if (secondary)
                {
                    SwitchWeapon(secondary);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && currentWeapon.mags > 0)
        {
            reloading = true;
            hands.Play("Reload");
        }
        if (currentWeapon.auto == true)
        {
            if (Input.GetMouseButton(0) && currentWeapon.cooldown == false && currentWeapon.ammo > 0)
            {
                ShootWithGun();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && currentWeapon.cooldown == false && currentWeapon.ammo > 0)
            {
                ShootWithGun();
            }
        }
    }

    void ShootWithGun()
    {
        hands.Play("Shoot");
        currentWeapon.Shoot();
        currentWeapon.ammo--;
        uim.UpdateAmmo(currentWeapon.ammo);
        GunAlert(currentWeapon.alertSoundRange);
    }

    public void GrabNewWeapon(Weapon newWeap)
    {
        weaponHands.SetActive(true);
        newWeap.gameObject.SetActive(false);
        if (newWeap.primary)
        {
            primary = newWeap;
            if (currentWeapon == null)
            {
                SwitchWeapon(newWeap);
            }
            else if (currentWeapon.primary == true)
            {
                SwitchWeapon(newWeap);
            }
        }
        else
        {
            secondary = newWeap;
            if (currentWeapon == null)
            {
                SwitchWeapon(newWeap);
            }
            else if (currentWeapon.primary == false)
            {
                SwitchWeapon(newWeap);
            }
        }
        UpdateUI();
    }

    void SwitchWeapon(Weapon newWeap)
    {
        if (currentWeapon)
        {
            if (newWeap.weaponName == currentWeapon.weaponName)
            {
                AddMag();
                UpdateUI();
                return;
            }
        }
        foreach (var w in weaponsAvailable)
        {
            if (w.weaponName == newWeap.weaponName)
            {
                if (currentWeapon)
                {
                    currentWeapon.gameObject.SetActive(false);
                }
                currentWeapon = w;
                currentWeapon.gameObject.SetActive(true);
                UpdateUI();
                return;
            }
        }
        Debug.LogError("Weapon not found in list");
    }

    void UpdateUI()
    {
        uim.UpdateAmmo(currentWeapon.ammo);
        uim.UpdateMags(currentWeapon.mags);
        uim.UpdateWeaponName(currentWeapon.weaponName);
    }

    void AddMag()
    {
        if (currentWeapon.mags < 7)
        {
            currentWeapon.mags += 2;
            uim.UpdateMags(currentWeapon.mags);
        }
    }

    //Called in the ReloadHelper script
    public void FinishReload()
    {
        reloading = false;
        currentWeapon.mags--;
        currentWeapon.ammo = currentWeapon.maxInMag;
        UpdateUI();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        uim.UpdateHealth(health);
    }

    void GunAlert(float alertRange)
    {
        List<Enemy> notAlertedEnemies = new List<Enemy>();
        foreach (var enemy in enemiesOnMap)
        {
            if (!enemy.statemachine.IsInState("CoverState"))
            {
                if (enemy.isAlive == true)
                {
                    if (Vector3.Distance(transform.position, enemy.transform.position) < alertRange)
                    {
                        notAlertedEnemies.Add(enemy);
                    }
                }
            }
        }
        if (notAlertedEnemies.Count > 0)
        {
            StartCoroutine(AlertEnemies(notAlertedEnemies));
        }
    }

    IEnumerator AlertEnemies(List<Enemy> enemiesToAlert)
    {
        foreach (var e in enemiesToAlert)
        {
            e.GunShotAlert();
            yield return new WaitForEndOfFrame();
        }
    }

}