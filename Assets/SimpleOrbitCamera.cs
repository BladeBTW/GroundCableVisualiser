using UnityEngine;
using UnityEngine.EventSystems;
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
    public float zoomSpeed = 0.02f;

    [Header("Move")]
    public float moveSpeed = 6f;
    public float fastMoveMultiplier = 2.5f;

    [Header("Angles")]
    public float yaw = 45f;
    public float pitch = 30f;
    public float minPitch = 10f;
    public float maxPitch = 80f;

    private Vector3 targetPosition;

    private void Start()
    {
        if (target != null)
        {
            targetPosition = target.position;
        }
        else
        {
            targetPosition = Vector3.zero;
        }

        UpdateCameraPosition();
    }

    private void Update()
    {
        HandleRotation();
        HandleMovement();
        HandleZoom();
        UpdateCameraPosition();
    }

    private void HandleRotation()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        if (mouse.rightButton.isPressed)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();

            yaw += mouseDelta.x * rotationSpeed;
            pitch -= mouseDelta.y * rotationSpeed;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
    }

    private void HandleMovement()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            vertical += 1f;
        }

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            vertical -= 1f;
        }

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            horizontal += 1f;
        }

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            horizontal -= 1f;
        }

        float currentMoveSpeed = moveSpeed;

        if (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed)
        {
            currentMoveSpeed *= fastMoveMultiplier;
        }

        Quaternion flatRotation = Quaternion.Euler(0f, yaw, 0f);

        Vector3 forward = flatRotation * Vector3.forward;
        Vector3 right = flatRotation * Vector3.right;

        Vector3 moveDirection = forward * vertical + right * horizontal;

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        targetPosition += moveDirection * currentMoveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        float scrollValue = mouse.scroll.ReadValue().y;

        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            distance -= scrollValue * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
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