using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // Singleton instance
    public static PlayerController Instance { get; private set; }

    [SerializeField] private TMPro.TMP_Text interactionPopUp;

    [SerializeField] private float jumpForce = 5f;

    // Camera to use for camera-relative movement
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 10f;

    // Input actions
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction interactAction;
    private InputAction pauseAction;

    public Animator animator;

    private Rigidbody rb;

    // Stats component
    private PlayerStats stats;

    // references to other objects
    private StoreActions currentStore;
    private BeggarActions currentBeggar;

    private bool jumpRequested;
    private bool isGrounded;

    public PauseManager pauseManager;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize PlayerInput and get the actions
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput?.actions.FindAction("Move", false);
        jumpAction = playerInput?.actions.FindAction("Jump", false);
        sprintAction = playerInput?.actions.FindAction("Sprint", false);
        interactAction = playerInput?.actions.FindAction("Interact", false);
        pauseAction = playerInput?.actions.FindAction("Pause", false);
        rb = GetComponent<Rigidbody>();

        // Cache stats (optional component on same GameObject)
        stats = GetComponent<PlayerStats>();

        // Smooth physics-driven motion for camera-following
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Prevent physics from rotating the body on X/Z 
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (cameraTransform == null)
        {
            Camera c = GetComponentInChildren<Camera>();
            if (c != null) cameraTransform = c.transform;
            else if (Camera.main != null) cameraTransform = Camera.main.transform;
        }

        if (interactionPopUp != null)
            interactionPopUp.enabled = false; 
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnEnable()
    {

        jumpAction.performed += OnJumpPerformed;

        interactAction.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        jumpAction.performed -= OnJumpPerformed;

        interactAction.performed -= OnInteractPerformed;
    }

    private void FixedUpdate()
    {
        if (moveAction != null)
        {
            Vector2 input = moveAction.ReadValue<Vector2>();

            // Determine speeds from stats if available
            float baseSpeed = stats != null ? stats.BaseMoveSpeed : 5f;
            float sprintSpeed = stats != null ? stats.SprintSpeed : 8f;
            float currentSpeed = (sprintAction != null && sprintAction.IsPressed()) ? sprintSpeed : baseSpeed;

            Vector3 moveWorld;
            if (cameraTransform != null)
            {
                Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
                Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
                moveWorld = (camRight * input.x + camForward * input.y) * currentSpeed;
            }
            else
            {
                // If no camera assigned, use local input direction relative to player transform
                Vector3 localMove = new Vector3(input.x, 0f, input.y);
                moveWorld = transform.TransformDirection(localMove) * currentSpeed;

                // Rotate player to face movement (optional when camera not used)
                Vector3 flat = new Vector3(localMove.x, 0f, localMove.z);
                if (flat.sqrMagnitude > 0.0001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(flat);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
                }
            }

            // Apply horizontal velocity while preserving vertical velocity
            rb.linearVelocity = new Vector3(moveWorld.x, rb.linearVelocity.y, moveWorld.z);
        }

        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }

        if (interactAction.IsPressed() && currentStore != null && !stats.IsInventoryFull())
        {
            currentStore.BuyFood();

            interactAction.Disable(); // Prevent multiple purchases in one press
            interactAction.Enable();
        }

        else if (interactAction.IsPressed() && currentBeggar != null)
        {
            GameObject item = stats.RemoveFirstFoodItem();
            if (item != null)
            {
                currentBeggar.Eat(item);
                interactAction.Disable(); // Prevent multiple purchases in one press
                interactAction.Enable();
            }
            else
            {
                Debug.Log("No item in inventory to give to beggar.");
            }
        }

        if (pauseAction.IsPressed())
        {
            pauseManager.PauseGame();
        }


        // Set Walking bool
        animator.SetBool("isWalking", moveAction.ReadValue<Vector2>().y > 0);

        // Set Running bool
        animator.SetBool("isRunning", moveAction.ReadValue<Vector2>().y > 0 && sprintAction.ReadValue<float>() > 0);

        // Set giveItem bool
        animator.SetBool("giveItem", interactAction != null && interactAction.IsPressed());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        interactionPopUp.enabled = true;

        if (other.gameObject.CompareTag("Store"))
        {
            Debug.Log("Entered store trigger");
            currentStore = other.gameObject.GetComponent<StoreActions>();
        }

        if (other.gameObject.CompareTag("Beggar"))
        {
            Debug.Log("Entered beggar trigger");
            currentBeggar = other.gameObject.GetComponent<BeggarActions>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currentStore = null;
        currentBeggar = null;
        interactionPopUp.enabled = false;
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("interacted called");
        if (currentStore != null)
        {
            currentStore.BuyFood();
        }
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
}