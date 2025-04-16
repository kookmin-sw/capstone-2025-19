using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class WeaponHolderSlot : MonoBehaviour
{
    public Transform parentOverride;
    public bool isLeftHandSlot;
    public bool isRightHandSlot;

    public GameObject currentWeaponModel;

    private Animator animator;

    PhotonView photonView;

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
        photonView = GetComponent<PhotonView>();
    }

    public void UnloadWeapon()
    {
        if(currentWeaponModel != null)
        {
            currentWeaponModel.SetActive(false);
        }
    }

    public void UnloadWeaponAndDestroy()
    {
        if (currentWeaponModel != null)
        {
            if(SceneController.Instance.GetCurrentSceneName() == "MultiPlayTestScene")
            {
                PhotonNetwork.Destroy(currentWeaponModel);
            }
            else
            {
                Destroy(currentWeaponModel);
            }
        }
    }
    public void LoadWeaponModel(WeaponStats weaponStats)
    {
        UnloadWeaponAndDestroy();

        if (weaponStats == null)
        {
            UnloadWeapon();
            return;
        }
        GameObject weapon = null;
        
        if (SceneController.Instance.GetCurrentSceneName() == "MultiPlayTestScene") //Multiplay function
        {
            Debug.Log($"photon network test");
            int ownerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            if (photonView == null) { photonView = GetComponent<PhotonView>(); }
            int parentViewID = -1;
            Debug.Log($"transform name {gameObject.name}, parentOverride {parentOverride.name}");
            PhotonView parentPV = (parentOverride != null)
                ? parentOverride.GetComponent<PhotonView>()
                : transform.GetComponent<PhotonView>();

            if (parentPV != null)
            {
                parentViewID = parentPV.ViewID;
            }
            bool islayer = (transform.root.CompareTag("Player"));
            photonView.RPC(nameof(LoadWeaponModelRPC), RpcTarget.All, ownerActorNumber, weaponStats.name, islayer,
                 parentViewID);

        }
        else // Singleplay function
        {
            weapon = Instantiate(weaponStats.weaponPrefab) as GameObject;
            if (!weaponStats.isRanged)
            {
                DamageCollider weaponCollider = weapon.GetComponentInChildren<DamageCollider>();
                weaponCollider.damage = weaponStats.damage;
                weaponCollider.tenacity = weaponStats.tenacity;
                weaponCollider.hitEffect = weaponStats.hitEffect;
                weaponCollider.tag = transform.root.tag == "Player" ? "PlayerWeapon" : "EnemyWeapon";
            }

            if (transform.root.tag == "Player") animator.runtimeAnimatorController = weaponStats.weaponOverride;

            if (weapon != null)
            {
                if (parentOverride != null)
                {
                    weapon.transform.parent = parentOverride;
                }
                else
                {
                    weapon.transform.parent = transform;
                }

                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
                weapon.transform.localScale = Vector3.one;
            }

            currentWeaponModel = weapon;
        }

    }

    [PunRPC]
    private void LoadWeaponModelRPC(
                                    int ownerActorNumber,
                                   string weaponStatsName,
                                   bool isPlayer,
                                   int parentViewID,
                                   PhotonMessageInfo info)
    {
        Debug.Log($"photon network test1 {parentViewID}");
        PhotonView pv = PhotonView.Find(parentViewID);
        if (pv == null)
        {
            return;
        }
        // 1) 기존 무기 제거
        UnloadWeaponAndDestroy();
        Debug.Log($"photon network test2");
        // 2) 무기 Instantiate
        GameObject weapon = null;
 
        // 네트워크 객체 생성
        weapon = PhotonNetwork.Instantiate(
            $"Prefabs/PlayerWeapon/Multiplay/{weaponStatsName}",
            Vector3.zero,
            Quaternion.identity
        );
        WeaponStats weaponStats = Resources.Load<WeaponStats>($"WeaponStats/{weaponStatsName}");
        /*else
        {
            // 오프라인(싱글) Instantiate
            // 만약 Resources 폴더 구조 다르다면 바꿔야 함
            var prefab = Resources.Load<GameObject>($"Prefabs/PlayerWeapon/Singleplay/{weaponPrefabName}");
            weapon = Instantiate(prefab);
        }*/

        // 3) 근접 무기라면 데미지 컬라이더 정보 세팅
        if (!weaponStats.isRanged)
        {
            DamageCollider weaponCollider = weapon.GetComponentInChildren<DamageCollider>();
            weaponCollider.damage = weaponStats.damage;
            weaponCollider.tenacity = weaponStats.tenacity;
            // hitEffectName 문자열을 이용해 Resources.Load 등으로 이펙트를 불러올 수도 있음
            weaponCollider.hitEffect = weaponStats.hitEffect;
            weaponCollider.tag = isPlayer ? "PlayerWeapon" : "EnemyWeapon";
        }

        // 4) 플레이어 무기라면 애니메이터 컨트롤러 오버라이드 (overrideControllerName으로 로드 가능)
        if (isPlayer)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == ownerActorNumber)
            {
                // WeaponHolderSlot이 포함된 캐릭터/오브젝트의 Animator
                // 예: (이 스크립트 바로 위에 있는 Animator라 가정)
                var animator = GetComponentInParent<Animator>();

                // 무기 별로 RuntimeAnimatorController가 다를 경우
                if (weaponStats.weaponOverride != null)
                {
                    animator.runtimeAnimatorController = weaponStats.weaponOverride;
                }
            }
        }

        // 5) 부모 지정 (parentViewID가 있을 경우)
        if (parentViewID != -1)
        {
            if (pv != null)
            {
                weapon.transform.SetParent(pv.transform, false);
            }
        }
        else
        {
            // parentOverride가 없으면, 현재 스크립트의 transform에 붙이거나 etc.
            weapon.transform.SetParent(this.transform, false);
        }

        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;

        // 6) 현재 무기 저장
        currentWeaponModel = weapon;
    }
}
