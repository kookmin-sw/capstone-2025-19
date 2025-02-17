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
    public float rotationSmoothTime = 0.1f; // �ε巯�� ȸ�� �ӵ�

    void Update()
    {
        if (virtualCamera == null || player == null) return;
        // ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        // Debug.Log($"test {mouseX} {mouseY}");

        // ���콺 �̵��� ���� ȸ�� ����
        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -30f, 60f);  // ��/�Ʒ� ���� ����

        currentRotation = Vector2.SmoothDamp(
            currentRotation,
            new Vector2(rotationX, rotationY),
            ref rotationVelocity,
            rotationSmoothTime
        );

        // **�÷��̾��� ȸ�� ���� (Y��)**
        //player.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
        player.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);

        // ī�޶� ȸ�� ����
        player.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        //Debug.Log(player.rotation);
    }
}
