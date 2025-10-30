using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerMouseLook : MonoBehaviour
{
    [SerializeField] private Transform target; // player is the target

    // Mouse look settings
    [SerializeField] private float sensitivity = 200f;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 75f;

    // Input attach
    private PlayerInput playerInput;
    private InputAction lookAction;
    private float yaw;   // (y axis)
    private float pitch; // (x axis)

    private void Start()
    {
        if (target == null && transform.parent != null)
            target = transform.parent;

        // initialize yaw/pitch from current rotations and check for target or player values
        yaw = target != null ? target.eulerAngles.y : transform.eulerAngles.y;
        pitch = NormalizeAngle(transform.localEulerAngles.x);

        playerInput = GetComponentInParent<PlayerInput>();
        if (playerInput != null)
            lookAction = playerInput.actions.FindAction("Look", false);
    }

    private void LateUpdate()
    {
        Vector2 lookDelta = Vector2.zero;

        if (lookAction != null && lookAction.enabled)
        {
            lookDelta = lookAction.ReadValue<Vector2>();
        }
        else if (Mouse.current != null)
        {
            // fallback if action not found/enabled: use raw mouse delta
            lookDelta = Mouse.current.delta.ReadValue();
        }
        else
        {
            return; // nothing to read
        }

        // Apply sensitivity and time
        float dx = lookDelta.x * sensitivity * Time.deltaTime;
        float dy = lookDelta.y * sensitivity * Time.deltaTime;

        // Yaw rotates the player (world Y), pitch rotates the camera locally (X)
            yaw += dx;
        pitch -= dy; // conventional mapping: moving mouse up looks up

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Apply yaw to target (player)
        if (target != null)
        {
            Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);
            target.rotation = Quaternion.Slerp(target.rotation, targetRot, smoothSpeed * Time.deltaTime);
        }

        // Apply pitch to camera (local)
        Quaternion camLocal = Quaternion.Euler(pitch, 0f, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, camLocal, smoothSpeed * Time.deltaTime);
    }

    private static float NormalizeAngle(float a)
    {
        if (a > 180f) a -= 360f;
        return a;
    }
}
