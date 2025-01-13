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

    // Start is called before the first frame update
    void Start()
    {
        localRotation = transform.localRotation;
        _originalPosition = weaponTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        weaponSway();
        weaponBob();
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

}
