using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayer : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] float moveSpeed = 5f;  // THE PLAYERS MOVEMENTSPEED
    [SerializeField] public float mouseSensitivityX = 100f;
    [SerializeField] public float mouseSensitivityY = 100f;
    [SerializeField] public float gravity = -5f;
    public Animator hands;

    [Header("Combat Settings")]
    public Weapon currentWeapon;
    public List<Weapon> weaponsAvailable = new List<Weapon>();
    public bool reloading;

    [Header("RayCast Settings")]
    [SerializeField] bool useRayCasting;    // ENABLED THE USE OF RAYCAST IN THE FPSCAMERA SCRIPT
    public float rangeCameraRay = 4;
    
    FPSCamera FPSCamera;
    UserInterfaceManager uim;
    CharacterController cc;

    void Start()
    {
        FPSCamera = FindObjectOfType<FPSCamera>();  // FINDING THE FPSCAMERA SCRIPT TO ACCESS THE RAYCAST FUNCTION
        cc = GetComponent<CharacterController>();
        uim = GetComponent<UserInterfaceManager>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (currentWeapon)
        {
            UpdateUI();
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

    void Combat()
    {
        if (reloading)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R) && currentWeapon.mags > 0)
        {
            reloading = true;
            hands.Play("Reload");
        }
        if (Input.GetMouseButton(0) && currentWeapon.cooldown == false && currentWeapon.ammo > 0)
        {
            hands.Play("Shoot");
            currentWeapon.Shoot();
            uim.UpdateAmmo(currentWeapon.ammo);
        }
    }

    public void SwitchWeapon(Weapon newWeap)
    {
        if (newWeap.weaponName == currentWeapon.weaponName)
        {
            AddMag();
            UpdateUI();
            newWeap.gameObject.SetActive(false);
            return;
        }
        foreach (var w in weaponsAvailable)
        {
            if (w.weaponName == newWeap.weaponName)
            {
                newWeap.gameObject.SetActive(false);
                currentWeapon.gameObject.SetActive(false);
                currentWeapon = w;
                currentWeapon.gameObject.SetActive(true);
            }
        }
        UpdateUI();
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

}