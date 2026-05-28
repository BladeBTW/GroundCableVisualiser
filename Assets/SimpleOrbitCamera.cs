using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleOrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Orbit")]
    public float distance = 12f;
    public float minDistance = 4f;
    public float maxDistance = 30f;
    public float rotationSpeed = 0.18f;

    [Header("Zoom")]
    public float keyboardZoomSpeed = 8f;
    public float mouseWheelZoomSpeed = 0.02f;

    [Header("Pan")]
    public float panSpeed = 6f;
    public float fastMoveMultiplier = 2.5f;

    [Header("Angles")]
    public float yaw = 45f;
    public float pitch = 30f;
    public float minPitch = 10f;
    public float maxPitch = 80f;

    [Header("Reset Defaults")]
    public float defaultDistance = 12f;
    public float defaultYaw = 45f;
    public float defaultPitch = 30f;
    public Vector3 defaultTargetPosition = new Vector3(0f, 0.8f, 0f);

    private Vector3 targetPosition;

    private void Start()
    {
        ResetCamera();
    }

    private void Update()
    {
        HandleRotation();
        HandleKeyboardControls();
        HandleMouseWheelZoom();
        UpdateCameraPosition();
    }

    public void ResetCamera()
    {
        distance = defaultDistance;
        yaw = defaultYaw;
        pitch = defaultPitch;

        if (target != null)
        {
            target.position = defaultTargetPosition;
            targetPosition = target.position;
        }
        else
        {
            targetPosition = defaultTargetPosition;
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
        float panDirection = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            panDirection -= 1f;
        }

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            panDirection += 1f;
        }

        if (Mathf.Abs(panDirection) < 0.01f)
        {
            return;
        }

        Quaternion flatRotation = Quaternion.Euler(0f, yaw, 0f);
        Vector3 right = flatRotation * Vector3.right;

        targetPosition += right * panDirection * panSpeed * speedMultiplier * Time.deltaTime;

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
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 cameraDirection = rotation * Vector3.back;
        Vector3 cameraPosition = targetPosition + cameraDirection * distance;

        transform.position = cameraPosition;
        transform.LookAt(targetPosition);
    }
}