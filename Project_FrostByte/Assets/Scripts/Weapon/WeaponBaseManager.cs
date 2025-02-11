using Unity.Mathematics;
using UnityEngine;

public abstract class WeaponBaseManager : MonoBehaviour
{
    //[Header("Weapon Setting")]
    //public string weaponName;
    [SerializeField] private Transform _mainCamera;
    [SerializeField] private Camera _cam;

    //[Header("Reloading")]
    //public int maxAmmo;
    //[SerializeField] private int _currentAmmo;
    public float fireRate;
    //public float reloadTime;
    protected bool p_canFire;

    //[Header("Aiming, Shooting and Recoil")]
    protected KeyCode _shootKey = KeyCode.Mouse0;
    protected KeyCode _aimKey = KeyCode.Mouse1;

    // Aiming and shooting
    public GameObject bulletPrefab;
    public Transform shootingPoint;
    public float bulletSpeed;

    public Transform aimingPosition;
    public Transform defaultPosition;
    public Transform weapon;
    public float aimingSpeed;
    
    private bool _isAiming;

    //// Recoil
    //private float _currentRecoil;
    //public float recoilAmount;
    //public float spreadAngle;
    //public float hipfireSpreadingAngle;
    //[SerializeField] protected float p_recoilSpeedReset;

    //// Audio
    //public AudioClip shootingSFX;
    //public AudioClip reloadingSFX;
    //public AudioSource audioSource;

    [Header("Weapon Sway")]
    public float swayAmount;
    public float maxSwayAmount;
    public float swaySmoothness;

    // Rotation sway
    public float rotationSwayAmount;
    public float maxRotationSway;
    public float rotationSmoothness;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    protected virtual void Start()
    {
        //_currentAmmo = maxAmmo;
        //_mainCamera = GameObject.FindWithTag("MainCamera").transform;

        weapon.position = defaultPosition.position;
        weapon.rotation = defaultPosition.rotation;

        _isAiming = false;

        //audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        Aiming();
        HandleWeaponSway();
        HandleFireModes();
    }

    public abstract void HandleFireModes();

    protected void Aiming()
    {
        _isAiming = Input.GetKey(_aimKey);

        weapon.localPosition = Vector3.Lerp(weapon.localPosition, _isAiming ? aimingPosition.localPosition : defaultPosition.localPosition, Time.deltaTime * aimingSpeed);
        weapon.localRotation = Quaternion.Slerp(weapon.localRotation, _isAiming ? aimingPosition.localRotation : defaultPosition.localRotation, Time.deltaTime * aimingSpeed);

    }

    protected void Shooting()
    {
        if (bulletPrefab == null || shootingPoint == null)
        {
            Debug.LogError("Bullet Prefab or Shooting Point is not assigned!");
            return;
        }

        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of the screen
        RaycastHit hit;

        // If the ray hits something, use that point as the target; otherwise, set a distant point
        Vector3 l_targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            l_targetPoint = hit.point;
        }
        else
        {
            // If no hit, use a point far in front of the camera
            l_targetPoint = ray.GetPoint(1000);
        }

        Vector3 l_direction = (l_targetPoint - shootingPoint.position).normalized;

        if(_isAiming)
        {
            l_direction = _cam.transform.forward;
        }

        GameObject l_bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);

        l_bullet.transform.rotation = Quaternion.LookRotation(l_direction);

        // Clear the trail
        TrailRenderer trail = l_bullet.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear();
        }
    }


    protected void HandleWeaponSway()
    {
        float l_mouseX = Input.GetAxis("Mouse X");
        float l_mouseY = Input.GetAxis("Mouse Y");

        // Position Sway
        Vector3 l_targetPosition = new Vector3(Mathf.Clamp(l_mouseX * swayAmount, -maxSwayAmount, swayAmount), Mathf.Clamp(l_mouseY * swayAmount, -maxSwayAmount, maxSwayAmount), 0f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, _initialPosition + l_targetPosition, Time.deltaTime * swaySmoothness);

        //Rotation Sway
        Quaternion l_targetRotation = Quaternion.Euler(Mathf.Clamp(-l_mouseY * rotationSwayAmount, -maxRotationSway, maxRotationSway),
            Mathf.Clamp(l_mouseX * rotationSwayAmount, -maxRotationSway, maxRotationSway),
            Mathf.Clamp(l_mouseX * rotationSwayAmount, -maxRotationSway, maxRotationSway));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, _initialRotation * l_targetRotation, Time.deltaTime * rotationSmoothness);
    }
}
