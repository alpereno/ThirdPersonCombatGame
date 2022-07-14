using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;
    public Transform targetTransform;   // The object the camera will follow
    public Transform cameraPivot;       // The object the camera uses to pivot (look up and down)

    Vector3 cameraFollowVelocity = Vector3.zero;
    public float cameraFollowSpeed = .2f;
    public float cameraLookSpeed = 6;
    public float cameraPivotSpeed = 6;
    public float lookAngle; // Camera looking up and down
    public float pivotAngle;    // Camera looking left and right
    public Vector2 pivotAngleMinMax; // -35 / 35

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
    }
    private  void FollowTarget()
    {
        Vector3 targetPos = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPos;
    }

    private  void RotateCamera()
    {
        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed*Time.deltaTime);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed*Time.deltaTime);
        pivotAngle = Mathf.Clamp(pivotAngle, pivotAngleMinMax.x, pivotAngleMinMax.y);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }
}
