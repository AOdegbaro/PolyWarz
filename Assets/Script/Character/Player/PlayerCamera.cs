using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public Camera cameraObject;
    public PlayerManager player;
    [SerializeField] private Transform cameraPivotTransform;

    // Change these to tweak camera performance
    [Header("CAMERA SETTINGS")] 
    private float cameraSmoothSpeed = 1;  // The bigger this number, the longer for the camera to reach its position during movement
    [SerializeField] float leftAndRightRotationSpeed = 220;
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float minimumPivot = -30; // The lowest point you are able to look down
    [SerializeField] float maximumPivot = 60; // The highest point you are able to look up
    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] private LayerMask collideWithLayers;
    
    [Header("CAMERA VALUES")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition; // Used for camera collisions( Moves the camera object to this position upon colliding) 
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    private float cameraZPosition; // Values used for Camera Collisions
    private float targetCameraZPosition; // Values used for camera Collisions
    private void Awake()
    {
        if (instance == null)
        {
            instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotations();
            HandleCollisions();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        // If locked on, force rotation towards target
        // else rotate regularly
        
        // Normal rotations
        // Rotate left and right based on horizontal movement on the right joystick
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
        // Rotate up and down based on vertical movement on the right joystick
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        // Clamp the up and down look angle between a min and max value
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;
        
        // Rotate this gameobject left and right
        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        // Rotate the pivot gameobject up and down
        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        // We check if there is an object in front of our desired direction ^ (See above)
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit,
                Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            // If ther is, we get our distance from it
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            // We then equate our target Z position to the following
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        // If our target position is less than our collision radius, we subtract our collision radius (Making it snap back)
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        // We then apply our final position using a lerp over time of 0.2f
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition,
            cameraCollisionRadius);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
}