using System.Collections;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [Header("Aiming Mechanics")]
    public Transform weapon;
    public Transform defaultPos;
    public Transform aimingPos;
    public float aimSpeed;
    private float _aimTime;

    private bool _isAiming;

    [Header("Ammo Stats")]
    private int _currentAmmo;
    public int maxAmmo;
    public float reloadingTime;
    private bool _isReloading;
    public AudioClip reloadingSFX;

    [Header("Shooting Mechanics")]
    public int damage;
    public float fireRate;
    public float range;
    public ParticleSystem muzzleFlash;
    public GameObject shootingLine;
    public Transform shootingPoint;
    private Camera _cam;

    public Vector3 recoilPos;
    public Quaternion recoilRot;

    private float recoilAmount; 
    public float adsBulletSpread;
    public float bulletSpread;
    public AudioClip shootingSFX;

    public AudioSource audioSource;

    private void Start()
    {
        _cam = GetComponentInParent<Camera>();

        weapon.position = defaultPos.position;
        weapon.rotation = defaultPos.rotation;

        _currentAmmo = maxAmmo;

        audioSource = GetComponent<AudioSource>();

    }

    private void Update()
    {
        Aiming();
        Shoot();
    }

    private void Aiming()
    {
        _isAiming = Input.GetKey(KeyCode.Mouse1);

        if (_isAiming && _aimTime < 1)
        {
            _aimTime += Time.deltaTime * aimSpeed;
        }
        else if(!_isAiming && _aimTime > 0)
        {
            _aimTime -= Time.deltaTime * aimSpeed;
        }

        Vector3 targetPos = defaultPos.position;
        Quaternion targetRot = defaultPos.rotation;

        if(_aimTime <= 0.5f)
        {
            targetPos = Vector3.Lerp(defaultPos.position, defaultPos.position + recoilPos, recoilAmount);
        }

        if(recoilAmount > 0)
        {
            recoilAmount -= Time.deltaTime;
        }

        weapon.position = Vector3.Lerp(targetPos, aimingPos.position, _aimTime);
        weapon.rotation = Quaternion.Lerp(defaultPos.rotation, aimingPos.rotation, _aimTime);
    }

    private void Shoot()
    {
        if (_isReloading) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _currentAmmo--;
            muzzleFlash.Play(); 

            Vector3 forward = _cam.transform.forward; // get the forward direction of the camera
            if (_isAiming)
            {
                // Generate random value within the X and Y axes spread range and convert from local to world space
                forward = forward + _cam.transform.TransformDirection(new Vector3(Random.Range(-adsBulletSpread, adsBulletSpread), Random.Range(-adsBulletSpread, adsBulletSpread)));
            }
            else
            {
                forward = forward + _cam.transform.TransformDirection(new Vector3(Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread)));
            }

            if (Physics.Raycast(_cam.transform.position, forward, out RaycastHit Hit))
            {
                // What would happen when the raycast hit something?
            }
            
            GameObject Tracer = Instantiate(shootingLine, shootingPoint.position, shootingPoint.rotation);
            Tracer.transform.LookAt(forward * 100);
            audioSource.PlayOneShot(shootingSFX);

            if(recoilAmount < 0.9f)
            {
                recoilAmount += 0.1f;
            }
        }

        if ((Input.GetKeyDown(KeyCode.R) && _currentAmmo <= maxAmmo) || _currentAmmo <= 0)
        {
            StartCoroutine(Reloading());
        }
        Debug.Log($"Ammo left: " + _currentAmmo);
    }

    private IEnumerator Reloading()
    {
        _isReloading = true;

        audioSource.PlayOneShot(reloadingSFX);
        yield return new WaitForSeconds(reloadingTime);
        _currentAmmo = maxAmmo;
        Debug.Log($"Ammo refilled " + _currentAmmo);
        _isReloading = false;
    }
}
