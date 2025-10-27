using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //Player movement speed and jump, adjustable in the inspector
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    // Input actions
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Rigidbody rb;

    // Set by the performed callback, consumed in FixedUpdate
    private bool jumpRequested;
    private bool isGrounded;

    private void Awake()
    {
        // Initialize PlayerInput and get the actions
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move", false);
        jumpAction = playerInput.actions.FindAction("Jump", false);
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (jumpAction != null)
            jumpAction.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        if (jumpAction != null)
            jumpAction.performed -= OnJumpPerformed;
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        // prevent multiple jumps until the player hits the ground again
        if (isGrounded)
        {
            jumpRequested = true;
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        if (moveAction != null)
        {
            Vector2 input = moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, input.y) * moveSpeed;

            // Move the player (physics-based)
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        }

        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
