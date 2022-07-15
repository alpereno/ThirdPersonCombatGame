using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;
    public Transform targetTransform;   // The object the camera will follow
    public Transform cameraPivot;       // The object the camera uses to pivot (look up and down)
    public Transform cameraTransform;   // The transform of actual camera object in the scene
    public LayerMask collisionLayers;   // The layers we want our camera to collide with

    Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraPosition;
    public float cameraCollisionOffSet = .2f;   // how much camera will jump off of objects its colliding with
    public float minimumCollisionOffSet = .2f;
    public float cameraFollowSpeed = .2f;
    public float cameraLookSpeed = 6;
    public float cameraPivotSpeed = 6;
    public float cameraCollisionRadius = 2;

    public float lookAngle; // Camera looking up and down
    public float pivotAngle;    // Camera looking left and right
    public Vector2 pivotAngleMinMax; // -35 / 35

    float defaultPosition;
    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }
    private  void FollowTarget()
    {
        Vector3 targetPos = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPos;
    }

    private  void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed*Time.deltaTime);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed*Time.deltaTime);
        pivotAngle = Mathf.Clamp(pivotAngle, pivotAngleMinMax.x, pivotAngleMinMax.y);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction = direction.normalized;

        if (Physics.SphereCast(cameraPivot.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float sqrdDistance = (cameraPivot.position - hit.point).magnitude;
            targetPosition = targetPosition - (sqrdDistance - cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffSet)
        {
            targetPosition = targetPosition - minimumCollisionOffSet;
        }
        cameraPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, .2f);
        cameraTransform.localPosition = cameraPosition;
    }
}
