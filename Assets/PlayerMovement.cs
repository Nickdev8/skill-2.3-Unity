using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    public Camera mainCamera;
    public float cameraSpeed = 100f;
    public float moveSpeed = 5f;
    public Rigidbody rb;
    public float lowermostY = -3f;

    private Vector3 _movementInput;
    private float _rotationY;
    private Vector3 _startpos;

    private void Start()
    {
        // Lock and hide the cursor for a better experience
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize rotation to the current Y rotation
        _rotationY = transform.eulerAngles.y;
        
        _startpos = transform.localPosition;
    }

    void Update()
    {
        HandleInput();
        HandleCameraRotation();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        Resetifdown();
    }

    private void HandleInput()
    {
        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal"); // A and D
        float vertical = Input.GetAxis("Vertical");     // W and S

        // Calculate movement direction relative to the camera
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // Ignore vertical components to keep movement on a flat plane
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // Normalize directions
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Combine inputs with camera directions
        _movementInput = (cameraForward * vertical + cameraRight * horizontal).normalized;
    }

    private void MovePlayer()
    {
        float newMoveSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            newMoveSpeed = moveSpeed * 2f;
        }
        
        if (_movementInput.magnitude > 0)
        {
            Vector3 newVelocity = _movementInput * newMoveSpeed;
            newVelocity.y = rb.linearVelocity.y; // Preserve gravity's effect
            rb.linearVelocity = newVelocity;
        }
        if (_movementInput.magnitude <= 0.2)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    private void HandleCameraRotation()
    {
        // Rotate the player based on mouse X-axis input
        float mouseX = Input.GetAxis("Mouse X");
        if (mouseX != 0)
        {
            _rotationY += mouseX * cameraSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0f, _rotationY, 0f);
        }

        // Rotate the camera vertically (pitch) with mouse Y-axis input
        float mouseY = Input.GetAxis("Mouse Y");
        if (mouseY != 0)
        {
            float cameraPitch = mainCamera.transform.localEulerAngles.x - mouseY * cameraSpeed * Time.deltaTime;
            
            // Clamp camera pitch to prevent flipping
            if (cameraPitch > 180f) cameraPitch -= 360f;
            cameraPitch = Mathf.Clamp(cameraPitch, -60f, 60f);

            mainCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }
    }

    private void Resetifdown()
    {
        if (transform.localPosition.y < lowermostY)
        {
            transform.localPosition = _startpos;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
