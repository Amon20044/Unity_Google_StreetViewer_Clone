using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float touchSensitivity = 0.2f; // Sensitivity for touch movement
    public Transform playerCamera; // Reference to the camera

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector2 lastTouchPosition;
    private bool isTouching = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleTouchInput(); // Handle touch for camera movement
        HandleMovement();   // Handle movement with virtual joystick or UI buttons
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isTouching = true;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                Vector2 deltaTouch = touch.position - lastTouchPosition;
                lastTouchPosition = touch.position;

                // Convert touch movement to camera rotation
                float mouseX = deltaTouch.x * touchSensitivity;
                float mouseY = deltaTouch.y * touchSensitivity;

                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent camera from flipping

                playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Vertical look
                transform.Rotate(Vector3.up * mouseX); // Horizontal look
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isTouching = false;
            }
        }
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal"); // Use UI buttons for movement
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
