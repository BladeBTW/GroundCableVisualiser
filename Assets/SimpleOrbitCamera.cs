using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleOrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Orbit")]
    public float distance = 12f;
    public float minDistance = 1f;
    public float maxDistance = 50f;
    public float rotationSpeed = 0.18f;

    [Header("Zoom")]
    public float keyboardZoomSpeed = 8f;
    public float mouseWheelZoomSpeed = 0.02f;

    [Header("Pan")]
    public float panSpeed = 6f;
    public float mousePanSpeed = 0.03f;
    public float fastMoveMultiplier = 2.5f;

    [Header("Angles")]
    public float yaw = 45f;
    public float pitch = 30f;
    public float minPitch = -89f;
    public float maxPitch = 89f;

    [Header("Reset Defaults")]
    public float defaultDistance = 12f;
    public float defaultYaw = 45f;
    public float defaultPitch = 30f;

    [Tooltip("If enabled, the camera uses the current CameraTarget position in the scene as its reset/start position.")]
    public bool useCurrentTargetPositionAsDefault = true;

    public Vector3 defaultTargetPosition = new Vector3(0f, 0.8f, 0f);

    private Vector3 targetPosition;
    private Vector3 runtimeDefaultTargetPosition;

    private void Start()
    {
        if (target != null)
        {
            targetPosition = target.position;

            if (useCurrentTargetPositionAsDefault)
            {
                runtimeDefaultTargetPosition = target.position;
            }
            else
            {
                runtimeDefaultTargetPosition = defaultTargetPosition;
                target.position = runtimeDefaultTargetPosition;
                targetPosition = runtimeDefaultTargetPosition;
            }
        }
        else
        {
            runtimeDefaultTargetPosition = defaultTargetPosition;
            targetPosition = runtimeDefaultTargetPosition;
        }

        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        UpdateCameraPosition();
    }

    private void Update()
    {
        HandleRotation();
        HandleKeyboardControls();
        HandleMouseWheelZoom();
        HandleMiddleMousePan();
        UpdateCameraPosition();
    }

    public void ResetCamera()
    {
        distance = defaultDistance;
        yaw = defaultYaw;
        pitch = defaultPitch;

        targetPosition = runtimeDefaultTargetPosition;

        if (target != null)
        {
            target.position = targetPosition;
        }

        UpdateCameraPosition();
    }

    private void HandleRotation()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        bool shouldRotate =
            mouse.leftButton.isPressed ||
            mouse.rightButton.isPressed;

        if (!shouldRotate)
        {
            return;
        }

        Vector2 mouseDelta = mouse.delta.ReadValue();

        yaw += mouseDelta.x * rotationSpeed;
        pitch -= mouseDelta.y * rotationSpeed;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void HandleKeyboardControls()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        float speedMultiplier = 1f;

        if (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed)
        {
            speedMultiplier = fastMoveMultiplier;
        }

        HandleKeyboardZoom(keyboard, speedMultiplier);
        HandleKeyboardPan(keyboard, speedMultiplier);
    }

    private void HandleKeyboardZoom(Keyboard keyboard, float speedMultiplier)
    {
        float zoomDirection = 0f;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            zoomDirection -= 1f;
        }

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            zoomDirection += 1f;
        }

        if (Mathf.Abs(zoomDirection) < 0.01f)
        {
            return;
        }

        distance += zoomDirection * keyboardZoomSpeed * speedMultiplier * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void HandleKeyboardPan(Keyboard keyboard, float speedMultiplier)
    {
        Vector3 panDirection = Vector3.zero;

        Quaternion flatRotation = Quaternion.Euler(0f, yaw, 0f);
        Vector3 right = flatRotation * Vector3.right;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            panDirection -= right;
        }

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            panDirection += right;
        }

        if (panDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        targetPosition += panDirection.normalized * panSpeed * speedMultiplier * Time.deltaTime;

        if (target != null)
        {
            target.position = targetPosition;
        }
    }

    private void HandleMiddleMousePan()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null || !mouse.middleButton.isPressed)
        {
            return;
        }

        Vector2 mouseDelta = mouse.delta.ReadValue();

        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 right = cameraRotation * Vector3.right;
        Vector3 up = cameraRotation * Vector3.up;

        Vector3 panMove =
            (-right * mouseDelta.x) +
            (-up * mouseDelta.y);

        targetPosition += panMove * mousePanSpeed;

        if (target != null)
        {
            target.position = targetPosition;
        }
    }

    private void HandleMouseWheelZoom()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        float scrollValue = mouse.scroll.ReadValue().y;

        if (Mathf.Abs(scrollValue) < 0.01f)
        {
            return;
        }

        distance -= scrollValue * mouseWheelZoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void UpdateCameraPosition()
    {
        if (target != null)
        {
            targetPosition = target.position;
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 cameraDirection = rotation * Vector3.back;
        Vector3 cameraPosition = targetPosition + cameraDirection * distance;

        transform.position = cameraPosition;
        transform.LookAt(targetPosition);
    }
}