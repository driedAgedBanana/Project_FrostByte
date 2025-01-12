using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speed")]
    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;

    private Quaternion startRotation;
    private Quaternion targetLeanRotation;

    [Header("Jumping")]
    public float jumpForce = 10;
    bool isGrounded = false;

    [Header("Leaning")]
    public float rotationSpeed = 2;
    public float amt;
    public float slerpAMT;
    public float leaningAmount = 20f;
    public float leaningSpeed = 15f;

    public Rigidbody rb;

    private Camera cam;
    private float camX;

    private float _verticalLookRotation = 0f;
    public Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
        camMovement();
        leaning();
    }

    private void camMovement()
    {
        transform.Rotate(transform.up * Input.GetAxis("Mouse X") * rotationSpeed);

        camX -= Input.GetAxis("Mouse Y") * rotationSpeed;

        camX = Mathf.Clamp(camX, -75, 75);

        cam.transform.localEulerAngles = new Vector3(camX, 0, 0);
    }

    private void playerMovement()
    {
        Vector3 MovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift))
        {
            MovementInput *= sprintSpeed;
        }
        else
        {
            MovementInput *= speed;
        }

        Vector3 MoveDir = (transform.forward * MovementInput.z + transform.right * MovementInput.x) / 2;
        rb.linearVelocity = new Vector3(MoveDir.x, rb.linearVelocity.y, MoveDir.z);

        isGrounded = Physics.Raycast(transform.position, -transform.up, 1.3f);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    private void leaning()
    {
        float leanInput = Input.GetAxis("Lean Axes");

        if (leanInput < 0) // Lean right
        {
            targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, -leaningAmount);
        }
        else if (leanInput > 0) // Lean left
        {
            targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, leaningAmount);
        }
        else // No lean
        {
            targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0 * leaningSpeed * Time.deltaTime);
        }

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLeanRotation, Time.deltaTime * leaningSpeed);
    }
}
