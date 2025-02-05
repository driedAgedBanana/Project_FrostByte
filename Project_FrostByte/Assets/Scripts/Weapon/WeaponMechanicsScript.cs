using UnityEngine;

public class WeaponMechanicsScript : MonoBehaviour
{
    [SerializeField] private Transform _mainCamera;
    [SerializeField] private Camera _cam;

    [Header("Shooting Mechanics")]
    public Transform bulletSpawnPoint;
    public float fireRate = 0.5f;
    private float _nextFireTime;
    public ParticleSystem muzzleFlash;

    private Animator _animator;
    [SerializeField] private AudioSource _shootingSFX;


    [Header("Recoil Mechanics")]
    public Transform _weapon;
    [SerializeField] private float _recoilAmount = 5f;
    [SerializeField] private float _recoilSpeedReset = 5f;
    private Vector3 _originalRotation;
    private float _currentRecoil = 0f;

    [Header("Aiming Mechanics")]
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode ADSKey = KeyCode.Mouse1;

    public float aimingSpeed = 12f;
    public Transform gun;
    public Transform aimingPosition;
    public Transform originalWeaponPosition;

    [SerializeField] private bool _isAiming = false;


    [Header("Weapon Sway mechanic")]
    public float Drag = 2.5f;
    public float DragThreshold = 6f;
    public float Smoothness = 5f;
    public Transform Parent;

    private Quaternion localRotation;

    private Vector3 _originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        localRotation = transform.localRotation;
        _originalRotation = _weapon.localEulerAngles;

        _mainCamera = GameObject.FindWithTag("MainCamera").transform;

        gun.position = originalWeaponPosition.position;
        gun.rotation = originalWeaponPosition.rotation;

        _animator = GetComponentInChildren<Animator>();

        _shootingSFX = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        weaponSway();
        Aiming();

        //Perform a check if the player is holding the shooting key 
        bool isFiring = Input.GetKey(shootKey) && Time.time >= _nextFireTime;

        if (isFiring)
        {
            Shoot();
            _nextFireTime = Time.time + fireRate;
            ApplyRecoil();

            muzzleFlash.Play();
            _shootingSFX.Play();
        }
        else
        {
            muzzleFlash.Stop();
        }
        
        ResetRecoil();        
        _animator.SetBool("IsShooting", isFiring);
    }

    private void Shoot()
    {
        GameObject bullet = BulletPool.instance.GetBullet();

        // Set bullet's position and rotation
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = bulletSpawnPoint.rotation;
        bullet.SetActive(true); // Make sure the bullet is active

        // Clear the TrailRenderer right after the bullet is activated
        TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear(); // This will clear the trail when the bullet is fired
        }
    }

    private void ApplyRecoil()
    {
        _currentRecoil += _recoilAmount;
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


    private void weaponSway()
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

        Quaternion newRotation = Quaternion.Euler(localRotation.x, localRotation.y + y, Parent.localEulerAngles.z + localRotation.z + z);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, Time.deltaTime * Smoothness);
    }

    private void Aiming()
    {
        if (Input.GetKeyDown(ADSKey))
        {
            _isAiming = true;
        }
        else if (Input.GetKeyUp(ADSKey))
        {
            _isAiming = false;
        }

        if (_isAiming)
        {
            // Lerp the position and rotation based on aiming
            gun.position = Vector3.Lerp(gun.position, aimingPosition.position, Time.deltaTime * aimingSpeed);
            gun.rotation = Quaternion.Lerp(gun.rotation, aimingPosition.rotation, Time.deltaTime * aimingSpeed);
        }
        else
        {
            // Lerp back to the original position and rotation
            gun.position = Vector3.Lerp(gun.position, originalWeaponPosition.position, Time.deltaTime * aimingSpeed);
            gun.rotation = Quaternion.Lerp(gun.rotation, originalWeaponPosition.rotation, Time.deltaTime * aimingSpeed);
        }
    }
}
