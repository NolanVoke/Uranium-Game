using UnityEngine;
using UnityEngine.InputSystem;

// First-person movement + mouse look using the generated PlayerInputActions class.
// Requirements:
// - Attach this script to a GameObject that has a CharacterController.
// - Add a Camera as a child of that GameObject and assign it to 'playerCamera'.
// - Create an Input Actions asset with an action map named "player" containing Move (Vector2) and Look (Vector2), and generate the C# class PlayerInputActions.
public class PersonWalk : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float gravity = -9.81f;

    [Header("Look")]
    public float lookSensitivity = 0.15f; // adjust to taste
    public Transform playerCamera; // assign the child camera

    PlayerInputActions controls;
    CharacterController controller;
    float verticalVelocity = 0f;
    float cameraPitch = 0f; // up/down rotation

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            Debug.LogError("PersonWalk requires a CharacterController on the same GameObject.");

        if (playerCamera == null)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam != null) playerCamera = cam.transform;
        }

        controls = new PlayerInputActions();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        if (controls == null) return;

        Vector2 moveInput = controls.player.Move.ReadValue<Vector2>();
        Vector2 lookInput = controls.player.Look.ReadValue<Vector2>();

        // Move relative to player orientation
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 desiredMove = (forward * moveInput.y + right * moveInput.x) * speed;

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -1f;
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = desiredMove + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // Look
        if (playerCamera != null)
        {
            float yaw = lookInput.x * lookSensitivity;
            float pitch = lookInput.y * lookSensitivity;

            transform.Rotate(Vector3.up, yaw);
            cameraPitch -= pitch;
            cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
            playerCamera.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }
    }
}
