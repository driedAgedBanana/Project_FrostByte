using Unity.Mathematics;
using UnityEngine;

public abstract class WeaponBaseManager : MonoBehaviour
{
    //[Header("Weapon Setting")]
    //public string weaponName;
    //[SerializeField] private Transform _mainCamera;
    //[SerializeField] private Camera _cam;

    //[Header("Reloading")]
    //public int maxAmmo;
    //[SerializeField] private int _currentAmmo;
    //public float fireRate;
    //public float reloadTime;
    //protected bool p_canFire;

    //[Header("Aiming, Shooting and Recoil")]
    //private KeyCode _shootKey = KeyCode.Mouse0;
    private KeyCode _aimKey = KeyCode.Mouse1;

    //// Aiming and shooting
    //public GameObject bulletPrefab;
    //public Transform shootingPoint;

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
    }

    public abstract void HandleFireModes();

    protected void Aiming()
    {
        _isAiming = Input.GetKey(_aimKey);

        weapon.localPosition = Vector3.Lerp(weapon.localPosition, _isAiming ? aimingPosition.localPosition : defaultPosition.localPosition, Time.deltaTime * aimingSpeed);
        weapon.localRotation = Quaternion.Slerp(weapon.localRotation, _isAiming ? aimingPosition.localRotation : defaultPosition.localRotation, Time.deltaTime * aimingSpeed);

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
