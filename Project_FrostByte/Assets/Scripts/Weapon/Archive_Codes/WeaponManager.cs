using System.Collections;
using UnityEngine;

public abstract class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    public string weaponName;
    public GameObject bulletPrefab; 

    [Header("Stats")]
    public int maxAmmo = 10;
    public float fireRate = 0.2f;
    public float reloadTime = 2f;

    [Header("Aiming, Shooting & Recoil")]
    public float aimingSpeed = 10f;
    public Transform aimingPosition;
    public Transform originalPosition;
    public AudioClip shootingSFX;
    public AudioClip reloadingSFX;
    public AudioSource audioSource;

    [Header("Weapon Sway Mechanic")]
    public float drag;
    public float dragThresHold;
    public float smoothness;
    public Transform parent;
    private Quaternion _localRotation;


    [Header("References")]
    [SerializeField] protected Transform shootingPoint;
    protected KeyCode shootKey = KeyCode.Mouse0;
    protected KeyCode aimKey = KeyCode.Mouse1;
    protected KeyCode reloadKey = KeyCode.R;

    protected int currentAmmo;
    protected bool isReloading;
    protected float nextFireTime;

    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
        transform.position = originalPosition.position;
        transform.rotation = originalPosition.rotation;

        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        HandleFire();
        HandleAiming();
        HandleWeaponSway();
    }

    protected void HandleWeaponSway()
    {
        float l_z = Input.GetAxis("Mouse Y") * drag;
        float l_y = Input.GetAxis("Mouse X") * drag;

        l_y = Mathf.Clamp(l_y, -dragThresHold, dragThresHold);
        l_z = Mathf.Clamp(l_z, -dragThresHold, dragThresHold);

        Quaternion swayRotation = Quaternion.Euler(l_z, l_y, 0);

        // Apply sway relative to camera direction
        transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * swayRotation, Time.deltaTime * smoothness);
    }

    public abstract void HandleFire(); // Implemented in child classes

    protected virtual void Shoot()
    {
        if (currentAmmo <= 0 || isReloading) return;

        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();

        if (bulletScript != null)
        {
            bulletScript.SetDirection(shootingPoint.forward);
        }

        PlayShootingSFX();
        currentAmmo--;

        if (currentAmmo == 0 || Input.GetKey(reloadKey))
        {
            StartCoroutine(Reload());
        }
    }

    protected IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        
        currentAmmo = maxAmmo;
        isReloading = false;
    }
    protected void HandleAiming()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        if (Input.GetKey(aimKey))
        {
            transform.position = Vector3.Lerp(transform.position, aimingPosition.position, Time.deltaTime * aimingSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mainCam.transform.forward), Time.deltaTime * aimingSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition.position, Time.deltaTime * aimingSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, originalPosition.rotation, Time.deltaTime * aimingSpeed);
        }
    }



    protected void PlayShootingSFX()
    {
        if (shootingSFX != null)
        {
            audioSource.PlayOneShot(shootingSFX);
        }
    }
}