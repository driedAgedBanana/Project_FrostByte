using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    #region Archive movement code
    //[Header("Movement Speed")]
    //public float speed = 5f;
    //public float sprintSpeed = 10f;
    //public float crouchSpeed = 2.5f;

    //private Quaternion startRotation;
    //private Quaternion targetLeanRotation;

    //[Header("Jumping")]
    //public float jumpForce = 10;
    //bool isGrounded = false;

    //[Header("Leaning")]
    //public float rotationSpeed = 2;
    //public float amt;
    //public float slerpAMT;
    //public float leaningAmount = 20f;
    //public float leaningSpeed = 15f;

    //public Rigidbody rb;

    //private Camera cam;
    //private float camX;

    //private float _verticalLookRotation = 0f;
    //public Transform playerTransform;

    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;

    //    rb = GetComponent<Rigidbody>();
    //    cam = GetComponentInChildren<Camera>();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    playerMovement();
    //    camMovement();
    //    leaning();
    //}

    //private void camMovement()
    //{
    //    transform.Rotate(transform.up * Input.GetAxis("Mouse X") * rotationSpeed);

    //    camX -= Input.GetAxis("Mouse Y") * rotationSpeed;

    //    camX = Mathf.Clamp(camX, -75, 75);

    //    cam.transform.localEulerAngles = new Vector3(camX, 0, 0);
    //}

    //private void playerMovement()
    //{
    //    Vector3 MovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

    //    if (Input.GetKey(KeyCode.LeftShift))
    //    {
    //        MovementInput *= sprintSpeed;
    //    }
    //    else
    //    {
    //        MovementInput *= speed;
    //    }

    //    Vector3 MoveDir = (transform.forward * MovementInput.z + transform.right * MovementInput.x) / 2;
    //    rb.linearVelocity = new Vector3(MoveDir.x, rb.linearVelocity.y, MoveDir.z);

    //    isGrounded = Physics.Raycast(transform.position, -transform.up, 1.3f);

    //    if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
    //    {
    //        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    //    }
    //}

    //private void leaning()
    //{
    //    float leanInput = Input.GetAxis("Lean Axes");

    //    if (leanInput < 0) // Lean right
    //    {
    //        targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, -leaningAmount);
    //    }
    //    else if (leanInput > 0) // Lean left
    //    {
    //        targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, leaningAmount);
    //    }
    //    else // No lean
    //    {
    //        targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0 * leaningSpeed * Time.deltaTime);
    //    }

    //    transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLeanRotation, Time.deltaTime * leaningSpeed);
    //}
    #endregion

    [Header("Player Movements")]
    public float moveSpeed;
    public float sprintSpeed;
    public float mouseSensitivity = 3f;

    private float _verticalLookRotation = 0f;
    public Transform cameraTransform;

    [Header("Leaning Mechanic")]
    public float rotationSpeed = 2; // Speed of camera rotation
    public float amt; // Leaning amount, not used directly
    public float slerpAMT; // Slerp amount, not used directly
    public float leaningAmount = 20f; // Amount to lean left or right
    public float leaningSpeed = 15f; // Speed of leaning transition

    private Quaternion _startRotation; // Initial rotation of the player
    private Quaternion _targetLeanRotation; // Target rotation for leaning

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
        leaningMechanic();
    }

    private void playerMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX); // Rotate the player horizontally

        _verticalLookRotation -= mouseY;
        _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -85, 85f);
        cameraTransform.localRotation = Quaternion.Euler(_verticalLookRotation, 0f, 0f);

        Vector3 moveDir = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        transform.position += moveDir * speed * Time.deltaTime;
    }

    private void leaningMechanic()
    {
        // Get lean input from the user
        float leanInput = Input.GetAxis("Lean Axes");

        // Determine target lean rotation based on input
        if (leanInput < 0) // Lean right
        {
            _targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, -leaningAmount);
        }
        else if (leanInput > 0) // Lean left
        {
            _targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, leaningAmount);
        }
        else // No lean
        {
            _targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
        }

        // Smoothly transition to the target lean rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, _targetLeanRotation, Time.deltaTime * leaningSpeed);
    }
}
