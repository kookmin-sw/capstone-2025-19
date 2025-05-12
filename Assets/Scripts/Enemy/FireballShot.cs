using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FireballShot : StateMachineBehaviour
{
    [Header("Prefab & Offset")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float forwardOffset = 1.0f;   // Z축(+forward)으로 얼마?
    [SerializeField] private float heightOffset = 0.5f;   // Y축 위로 얼마?

    private FireballMove loadedFireball;

    public override void OnStateEnter(Animator animator,
                                      AnimatorStateInfo stateInfo,
                                      int layerIndex)
    {
        // ① 스폰 위치 계산 : 몬스터 위치 + 로컬 Z/Y 오프셋
        Vector3 spawnPos =
            animator.transform.position                       // 몬스터 위치
          + animator.transform.forward * forwardOffset        // 앞쪽
          + Vector3.up * heightOffset;                        // 약간 위

        // ② 파이어볼 생성
        /*GameObject go = Object.Instantiate(
            fireballPrefab,
            spawnPos,
            animator.transform.rotation       // 몬스터가 보는 방향
        );*/

        GameObject go = PhotonNetwork.InstantiateRoomObject("Prefabs/Enemys/MultiPlay/Fireball", spawnPos, animator.transform.rotation);




        loadedFireball = go.GetComponent<FireballMove>();
        Debug.Log("파이어볼 생성");
    }

    public override void OnStateExit(Animator animator,
                                     AnimatorStateInfo stateInfo,
                                     int layerIndex)
    {
        if (loadedFireball == null) return;

        // ③ 발사 직전(Exit 시점)에 다시 한 번 스폰 포인트 보정
        Vector3 spawnPos =
            animator.transform.position
          + animator.transform.forward * forwardOffset
          + Vector3.up * heightOffset;

        loadedFireball.transform.position = spawnPos;
        loadedFireball.transform.rotation = animator.transform.rotation;

        loadedFireball.Shot();      // 이동 시작
        loadedFireball = null;      // 참조 해제
    }
}
