using UnityEngine;

public class WeaponScript : MonoBehaviour
{

    [SerializeField] private float _smooth;
    [SerializeField] private float _swayMultiplier;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _swayMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * _swayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseX, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion tagerRotation = rotationX * rotationY;

        transform.rotation = Quaternion.Slerp(transform.rotation, tagerRotation, _smooth * Time.deltaTime);
    }
}
