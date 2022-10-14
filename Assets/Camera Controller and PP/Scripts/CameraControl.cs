using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// A simple free camera to be added to a Unity game object.
/// 
/// Keys:
///	wasd / arrows	- movement
///	q/e 			- up/down (local space)
///	hold shift		- enable fast movement mode
///	right mouse  	- enable free look
///	mouse			- free look / rotation

public class CameraControl : MonoBehaviour
{
<<<<<<< Updated upstream
    bool controlEnabled;

    public Transform anchor;
    Transform target;
    Quaternion targetOrientationOffset;
    Quaternion orientationOffset;
    float timeOfTargetAttachment = float.MinValue;
    const float timeToSnapToTarget = 2f;

    float initialDistanceFromAnchor;
    float distanceFromAnchor;
    float targetDistanceFromAnchor;
    float targetDistanceFromAnchorLastFollow;
    float minDistance = 0.5f;
    float maxDistance = 8f;
    float distanceChangeVelocity;

    const int orientationControlButton = 0; // Left Mouse button
    public float orientationSensitivity = 0.5f;
    public float orientationVerticalFreedom = 60;
    [Range(0, 1f)]
    public float orientationChangeDuration = 0.02f;
    Vector2 orientation;
    Vector2 orientationTarget;
    Vector2 orientationChangeVelocity;

    const int pedestalControlButton = 1; // Right Mouse button
    public Vector2 pedestalFreedom;
    public float pedestalSensitivity = 0.1f; // Could be calculated out of mouse position projection to focus axis to have 1:1 motion
    public float pedestalAndDistanceChangeDuration = 0.1f;
    float pedestal;
    float pedestalTarget;
    float pedestalTargetLastFollow;
    float pedestalChangeVelocity;

    public float zoomDragSensitivity = 0.1f;
    public float zoomScrollSensitivity = 1.0f;

    public bool farClipPlaneTrimming = false;
    [Min(1.0f)]
    public float farClipPlaneMinimum = 1.0f;

    Vector3 previousMousePos;

    Camera attachedCamera;

    float DeltaTime => Time.deltaTime;

    public float focusPedestalSlack = 0.001f; // was 0.2f;
    public float focusDistanceSlack = 0.001f; // was 1.5f;

    private void Awake()
    {
        if (!anchor)
            return;
=======
  
    public float movementSpeed = 10f;
>>>>>>> Stashed changes

    public float fastMovementSpeed = 100f;


    public float freeLookSensitivity = 3f;


    public float zoomSensitivity = 10f;

    public float fastZoomSensitivity = 50f;

  
    private bool looking = false;

    void Update()
    {
        var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var movementSpeed = fastMode ? fastMovementSpeed : this.movementSpeed;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = transform.position + (-transform.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = transform.position + (transform.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = transform.position + (transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = transform.position + (-transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = transform.position + (transform.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position = transform.position + (-transform.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.PageUp))
        {
            transform.position = transform.position + (Vector3.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.PageDown))
        {
            transform.position = transform.position + (-Vector3.up * movementSpeed * Time.deltaTime);
        }

        if (looking)
        {
            float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
            float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0)
        {
            var zoomSensitivity = fastMode ? this.fastZoomSensitivity : this.zoomSensitivity;
            transform.position = transform.position + transform.forward * axis * zoomSensitivity;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartLooking();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopLooking();
        }
    }

    void OnDisable()
    {
        StopLooking();
    }


    public void StartLooking()
    {
        looking = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void StopLooking()
    {
        looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}