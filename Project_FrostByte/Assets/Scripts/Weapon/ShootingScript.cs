using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    [SerializeField] private Transform _mainCamera;
    [SerializeField] private Camera _cam;

    [Header("Aiming Mechanics")]
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode ADSKey = KeyCode.Mouse1;

    public float aimingSpeed = 12f;
    public Transform gun;
    public Transform aimingPosition;
    public Transform originalWeaponPosition;

    [SerializeField] private bool _isAiming = false;
    //[SerializeField] private Image crosshairImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCamera = GameObject.FindWithTag("MainCamera").transform;

        gun.position = originalWeaponPosition.position;
        gun.rotation = originalWeaponPosition.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        Aiming();
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
            gun.position = Vector3.Lerp(gun.position, aimingPosition.position, Time.deltaTime * aimingSpeed);
            gun.rotation = Quaternion.Lerp(gun.rotation, aimingPosition.rotation, Time.deltaTime * aimingSpeed);
            //crosshairImage.gameObject.SetActive(false);
        }
        else
        {
            gun.position = Vector3.Lerp(gun.position, originalWeaponPosition.position, Time.deltaTime * aimingSpeed);
            gun.rotation = Quaternion.Lerp(gun.rotation, originalWeaponPosition.rotation, Time.deltaTime * aimingSpeed);
            //crosshairImage.gameObject.SetActive(true);
        }
    }
}
