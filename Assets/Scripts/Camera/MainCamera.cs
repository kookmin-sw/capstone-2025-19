using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;
    void Start()
    {
        //virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if(player != null)
        {
            rotationY = player.eulerAngles.y;
        }
    }
    //float mouseX, mouseY;
    public float mouseSensitivity = 3f;
    public CinemachineVirtualCamera virtualCamera;
    // Update is called once per frame

    float rotationX = 0f;
    float rotationY = 0f;
    private Vector2 currentRotation;
    private Vector2 rotationVelocity;
    public float rotationSmoothTime = 0.1f; // 부드러운 회전 속도

    void Update()
    {
        if (virtualCamera == null || player == null) return;
        // 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        Debug.Log($"test {mouseX} {mouseY}");

        // 마우스 이동에 따라 회전 적용
        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -30f, 60f);  // 위/아래 각도 제한

        currentRotation = Vector2.SmoothDamp(
            currentRotation,
            new Vector2(rotationX, rotationY),
            ref rotationVelocity,
            rotationSmoothTime
        );

        // **플레이어의 회전 조작 (Y축)**
        //player.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
        player.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);

        // 카메라 회전 적용
        player.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        Debug.Log(player.rotation);
    }
}
