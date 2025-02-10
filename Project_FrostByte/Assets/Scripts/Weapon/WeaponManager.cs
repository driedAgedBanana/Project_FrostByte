//using System.Collections;
//using System.Runtime.CompilerServices;
//using Unity.VisualScripting;
//using UnityEngine;

//public abstract class WeaponManager : MonoBehaviour
//{
//    [Header("Weapon Settings")]
//    public string weaponName;
//    public GameObject bulletPrefab;

//    [Header("Stats")]
//    public int maxAmmo = 10;
//    public float fireRate = 0.2f;
//    public float reloadTime = 2f;

//    [Header("Aiming, Shooting & Recoil")]
//    public float aimingSpeed = 10f;
//    public Transform aimingPosition;
//    public Transform originalPosition;
//    public AudioClip shootingSFX;
//    public AudioClip reloadingSFX;
//    public AudioSource audioSource;

//    [Header("Weapon Sway Mechanic")]
//    public float drag;
//    public float dragThresHold;
//    public float smoothness;
//    public Transform parent;
//    private Quaternion _localRotation;


//    [Header("References")]
//    [SerializeField] protected Transform shootingPoint;
//    protected KeyCode shootKey = KeyCode.Mouse0;
//    protected KeyCode aimKey = KeyCode.Mouse1;
//    protected KeyCode reloadKey = KeyCode.R;

//    protected int currentAmmo;
//    protected bool isReloading;
//    protected float nextFireTime;

//    public virtual void Start()
//    {

//    }
//}