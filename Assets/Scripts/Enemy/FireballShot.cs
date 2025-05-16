using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FireballShot : StateMachineBehaviour
{
    [Header("Prefab & Offset")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float forwardOffset = 1.0f;   // Z��(+forward)���� ��?
    [SerializeField] private float heightOffset = 0.5f;   // Y�� ���� ��?

    private FireballMove loadedFireball;

    public override void OnStateEnter(Animator animator,
                                      AnimatorStateInfo stateInfo,
                                      int layerIndex)
    {
        // �� ���� ��ġ ��� : ���� ��ġ + ���� Z/Y ������
        Vector3 spawnPos =
            animator.transform.position                       // ���� ��ġ
          + animator.transform.forward * forwardOffset        // ����
          + Vector3.up * heightOffset;                        // �ణ ��

        // �� ���̾ ����
        /*GameObject go = Object.Instantiate(
            fireballPrefab,
            spawnPos,
            animator.transform.rotation       // ���Ͱ� ���� ����
        );*/

        GameObject go = PhotonNetwork.InstantiateRoomObject("Prefabs/Enemys/MultiPlay/Fireball", spawnPos, animator.transform.rotation);




        loadedFireball = go.GetComponent<FireballMove>();
        Debug.Log("���̾ ����");
    }

    public override void OnStateExit(Animator animator,
                                     AnimatorStateInfo stateInfo,
                                     int layerIndex)
    {
        if (loadedFireball == null) return;

        // �� �߻� ����(Exit ����)�� �ٽ� �� �� ���� ����Ʈ ����
        Vector3 spawnPos =
            animator.transform.position
          + animator.transform.forward * forwardOffset
          + Vector3.up * heightOffset;

        loadedFireball.transform.position = spawnPos;
        loadedFireball.transform.rotation = animator.transform.rotation;

        loadedFireball.Shot();      // �̵� ����
        loadedFireball = null;      // ���� ����
    }
}
