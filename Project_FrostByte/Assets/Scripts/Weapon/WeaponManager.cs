using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngineInternal;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Transform _mainCamera;
    [SerializeField] private Camera _cam;

    [Header("Related Shooting Mechanic")]

    // Aiming Mechanics
    private KeyCode _aimKey = KeyCode.Mouse1;

    public Transform gun;
    public Transform aimingPosition;
    public Transform originalWeaponPosition;

    private bool _isAiming = false;

    private Animator _animator;
    private AudioSource _shootingSFX;

    // Shooting Mechanics
    [SerializeField] private bool _canFire;

    private KeyCode _shootKey = KeyCode.Mouse0;
    private KeyCode _switchingFireMode = KeyCode.B;
    
    public WeaponData currentWeapon;
    private int _currentAmmo;
    private float _nextFireTime;

    [SerializeField] private Transform _bulletSpawnPoint;

    // Burst Mode Mechanics
    private int _shotsInBursts;
    private bool _canFireBurst;
    private float _lastBurstTime;

    // Reload Mechanics
    private float _reloadingTime;
    private bool _isReloading;


    public Transform _weapon;
    [SerializeField] private float _recoilSpeedReset = 5f;
    private Vector3 _originalRotation;
    private float _currentRecoil = 0f;

    [Header("Weapon Sway mechanic")]
    public float Drag = 2.5f;
    public float DragThreshold = 6f;
    public float Smoothness = 5f;
    public Transform Parent;

    private Quaternion _localRotation;

    private Vector3 _originalPosition;

    private void Start()
    {
        _currentAmmo = currentWeapon.maxAmmo;
        _mainCamera = GameObject.FindWithTag("MainCamera").transform;

        _localRotation = transform.localRotation;
        _originalRotation = _weapon.localEulerAngles;

        gun.position = originalWeaponPosition.position;
        gun.rotation = originalWeaponPosition.rotation;

        _animator = GetComponentInChildren<Animator>();
        _shootingSFX = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Aiming();
        WeaponSway();

        HandleFireModes();

        if (_canFire && !_isReloading && _currentAmmo > 0 && Input.GetKey(_shootKey))
        {
            Shoot();
            _nextFireTime = Time.time + currentWeapon.fireRate;
            ApplyRecoil();
        }

        _animator.SetBool("IsShooting", _canFire);
        ResetRecoil();
    }


    private void HandleFireModes()
    {
        if (!_isReloading && _currentAmmo > 0 && Time.time >= _nextFireTime)
        {
            _canFire = true;
        }
        else
        {
            _canFire = false;
        }

        // Handle automatic fire mode
        if (currentWeapon.isAutomatic)
        {
            _canFire = CanFireAutomatically();
        }
        // Handle burst fire mode
        else if (currentWeapon.isBurst && !currentWeapon.isPistolWeapon)
        {
            _canFire = CanFireBurst();
        }
        // Handle pistol mode
        else if (currentWeapon.isPistolWeapon)
        {
            // For pistols, fire on press and only once per press (no automatic fire)
            if (Input.GetKeyDown(_shootKey) && Time.time >= _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + currentWeapon.fireRate; 
                _canFire = true;
            }
        }
    }


    private bool CanFireAutomatically()
    {
        return Input.GetKey(_shootKey) && Time.time >= _nextFireTime; 
    }

    private bool CanFireBurst()
    {
        if (Input.GetKeyDown(_shootKey) && Time.time >= _lastBurstTime + currentWeapon.burstCooldown)
        {
            StartCoroutine(BurstFire());
            return true;
        }
        return false;
    }


    private void Shoot()
    {
        if (_currentAmmo <= 0) return;

        GameObject bullet = BulletPool.instance.GetBullet();

        // Set bullet's position and rotation
        bullet.transform.position = _bulletSpawnPoint.position;
        bullet.transform.rotation = _bulletSpawnPoint.rotation;
        bullet.SetActive(true); // Make sure the bullet is active

        PlayShootingSFX();

        _currentAmmo--;

        if (_currentAmmo == 0 || Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reloading());
        }

        Vector3 targetPoint;
        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of the screen
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            // If the raycase does not hit anything, use a point far away in the camera's orward direction
            targetPoint = ray.GetPoint(1000);
        }

        // Calculate the direction
        Vector3 direction = (targetPoint - _bulletSpawnPoint.position).normalized;

        // Apply a random range as recoil if not ADS
        float spreadAngle = _isAiming ? currentWeapon.spreadAngle : currentWeapon.hipfireSpreadAngle;

        if (spreadAngle > 0)
        {
            // Giving random rotation within the spread angle
            direction = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0) * direction;
        }

        // Set the bullet's rotation to face the target rotation
        bullet.transform.rotation = Quaternion.LookRotation(-direction); // This is some diabolical work but I do not care

        // Clear the TrailRenderer right after the bullet is activated
        TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear(); // This will clear the trail when the bullet is fired
        }
    }

    private IEnumerator BurstFire()
    {
        for (_shotsInBursts = 0; _shotsInBursts < currentWeapon.burstCount; _shotsInBursts++)
        {
            Shoot();
            yield return new WaitForSeconds(currentWeapon.burstFireRate);
        }
        _lastBurstTime = Time.time;
    }

    private void PlayShootingSFX()
    {
        if (currentWeapon.shootingSFX != null)
        {
            // One shot won't overlap with previous sounds when shooting auto
            _shootingSFX.PlayOneShot(currentWeapon.shootingSFX);
        }
    }

    private void ApplyRecoil()
    {
        _currentRecoil += currentWeapon.recoilAmount;
        _weapon.localEulerAngles = new Vector3(_originalRotation.x - _currentRecoil, _originalRotation.y, _originalRotation.x);
    }

    private void ResetRecoil()
    {
        if (_currentRecoil > 0)
        {
            _currentRecoil = Mathf.Lerp(_currentRecoil, 0, Time.deltaTime * _recoilSpeedReset);
            _weapon.localEulerAngles = new Vector3(_originalRotation.x - _currentRecoil, _originalRotation.y, _originalRotation.x);
        }
    }

    IEnumerator Reloading()
    {
        _canFire = false;
        _isReloading = true;

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        _currentAmmo = currentWeapon.maxAmmo;
        _isReloading = false;
        _canFire = true;
    }

    private void WeaponSway()
    {
        float z = (Input.GetAxis("Mouse Y")) * Drag;
        float y = (Input.GetAxis("Mouse X")) * Drag;

        if (Drag >= 0)
        {
            y = Mathf.Clamp(y, -DragThreshold, DragThreshold);
            z = Mathf.Clamp(z, -DragThreshold, DragThreshold);
        }
        else
        {
            y = Mathf.Clamp(y, DragThreshold, -DragThreshold);
            z = Mathf.Clamp(z, DragThreshold, -DragThreshold);
        }

        Quaternion newRotation = Quaternion.Euler(_localRotation.x, _localRotation.y + y, Parent.localEulerAngles.z + _localRotation.z + z);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, Time.deltaTime * Smoothness);
    }

    private void Aiming()
    {
        if (Input.GetKeyDown(_aimKey))
        {
            _isAiming = true;
            Debug.Log("Aim!");
        }
        else if (Input.GetKeyUp(_aimKey))
        {
            _isAiming = false;
            Debug.Log("Stop aiming");
        }

        if (_isAiming)
        {
            // Lerp the position and rotation based on aiming
            gun.position = Vector3.Lerp(gun.position, aimingPosition.position, Time.deltaTime * currentWeapon.aimingSpeed);
            gun.rotation = Quaternion.Lerp(gun.rotation, aimingPosition.rotation, Time.deltaTime * currentWeapon.aimingSpeed);
        }
        else
        {
            // Lerp back to the original position and rotation
            gun.position = Vector3.Lerp(gun.position, originalWeaponPosition.position, Time.deltaTime * currentWeapon.aimingSpeed);
            gun.rotation = Quaternion.Lerp(gun.rotation, originalWeaponPosition.rotation, Time.deltaTime * currentWeapon.aimingSpeed);
        }
    }
}
