using UnityEngine;

public class WeaponMechanics : MonoBehaviour
{
    [Header("Weapon Sway mechanic")]
    public float Drag = 2.5f;
    public float DragThreshold = -5f;
    public float Smoothness = 5f;
    public Transform Parent;

    private Quaternion localRotation;

    [Header("Weapon Bobbing mechanic")]
    public Transform weaponTransform;
    public float bounceAmount = 0.05f;
    public float bounceSpeed = 5;

    private Vector3 _originalPosition;

    [Header("Aiming Mechanics")]

    [SerializeField] private Transform _mainCamera;
    [SerializeField] private Camera _cam;

    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode ADSKey = KeyCode.Mouse1;

    public float aimingSpeed = 12f;
    public Transform gun;
    public Transform aimingPosition;
    public Transform originalWeaponPosition;
    [SerializeField] private bool _isAiming = false;

    // Start is called before the first frame update
    void Start()
    {
        localRotation = transform.localRotation;
        _originalPosition = weaponTransform.localPosition;

        _mainCamera = GameObject.FindWithTag("MainCamera").transform;

        gun.position = originalWeaponPosition.position;
        gun.rotation = originalWeaponPosition.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        weaponSway();
        Aiming();
    }

    private void weaponSway()
    {
        float z = (Input.GetAxis("Mouse Y")) * Drag;
        float y = (Input.GetAxis("Mouse X")) * Drag;

        if (Drag >= 0)
        {
            y = (y > DragThreshold) ? DragThreshold : y;
            y = (y < -DragThreshold) ? -DragThreshold : y;

            z = (z > DragThreshold) ? DragThreshold : z;
            z = (z < -DragThreshold) ? -DragThreshold : z;
        }
        else
        {
            y = (y < DragThreshold) ? DragThreshold : y;
            y = (y > -DragThreshold) ? -DragThreshold : y;

            z = (z < DragThreshold) ? -DragThreshold : z;
            z = (z > -DragThreshold) ? -DragThreshold : z;
        }

        Quaternion newRotation = Quaternion.Euler(localRotation.x, localRotation.y + y, Parent.localEulerAngles.z + localRotation.z + z);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, (Time.deltaTime * Smoothness));
    }

    public void weaponBob()
    {
        float playerSpeed = new Vector3(GetComponentInParent<Rigidbody>().linearVelocity.x, 0, GetComponentInParent<Rigidbody>().linearVelocity.z).magnitude;

        if (playerSpeed > 0.1f)
        {
            // Only modify the Y position for the bobbing
            float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
            weaponTransform.localPosition = new Vector3(weaponTransform.localPosition.x, _originalPosition.y + bounce, weaponTransform.localPosition.z);
        }
        else
        {
            // Reset to the original position
            weaponTransform.localPosition = _originalPosition;
        }
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
