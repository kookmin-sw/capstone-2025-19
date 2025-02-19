using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Transform targetTransform;
    public Transform cameraTransform;
    public Transform cameraPivotTransform;
    private Transform myTransform;
    private Vector3 cameraTransformPosition;
    private LayerMask ignoreLayers;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public static CameraHandler singleton;

    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.03f;


    private float targetPosition;
    private float defaultPosition;
    private float lookAngle;
    private float pivotAngle;
    public float minimumPivot = -35;
    public float maximumPivot = 35;

    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;

    private void Awake()
    {
        singleton = this;
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }

    public void FollowTarget(float delta)
    {
        Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
        myTransform.position = targetPosition;

        HandleCameraCollisions(delta);
    }

    private Vector2 mouseAcceleration = Vector2.zero;
    private Vector2 currentMouseInput = Vector2.zero;
    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        // 마우스 입력값 업데이트 (이전 입력값과 비교하여 가속 적용)
        currentMouseInput.x = Mathf.Lerp(currentMouseInput.x, mouseXInput, delta * 5f);  // Lerp로 부드러운 가속
        currentMouseInput.y = Mathf.Lerp(currentMouseInput.y, mouseYInput, delta * 5f);

        // 마우스 속도 증가 (가속 적용)
        mouseAcceleration.x += (currentMouseInput.x * lookSpeed) * delta;
        mouseAcceleration.y += (currentMouseInput.y * pivotSpeed) * delta;

        // 가속도 기반 회전값 업데이트
        lookAngle += mouseAcceleration.x;
        pivotAngle -= mouseAcceleration.y;

        // 최소/최대 각도 제한
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

        // Y축 회전 (좌우)
        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        myTransform.rotation = targetRotation;

        // X축 회전 (상하)
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;

        // 감속 적용 (자연스럽게 움직임 멈추도록)
        mouseAcceleration *= 0.9f;  // 감속도 0.9 -> 90% 유지, 점진적으로 줄어듦
        /*lookAngle += (mouseXInput * lookSpeed) / delta;
        pivotAngle -= (mouseYInput * pivotSpeed) / delta;
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        myTransform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;*/
    }
    
    private void HandleCameraCollisions(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
        cameraTransform.localPosition = cameraTransformPosition;
    }
}
