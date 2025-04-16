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
        // 1) ���� ���� ����
        UnloadWeaponAndDestroy();
        Debug.Log($"photon network test2");
        // 2) ���� Instantiate
        GameObject weapon = null;
 
        // ��Ʈ��ũ ��ü ����
        weapon = PhotonNetwork.Instantiate(
            $"Prefabs/PlayerWeapon/Multiplay/{weaponStatsName}",
            Vector3.zero,
            Quaternion.identity
        );
        WeaponStats weaponStats = Resources.Load<WeaponStats>($"WeaponStats/{weaponStatsName}");
        /*else
        {
            // ��������(�̱�) Instantiate
            // ���� Resources ���� ���� �ٸ��ٸ� �ٲ�� ��
            var prefab = Resources.Load<GameObject>($"Prefabs/PlayerWeapon/Singleplay/{weaponPrefabName}");
            weapon = Instantiate(prefab);
        }*/

        // 3) ���� ������ ������ �ö��̴� ���� ����
        if (!weaponStats.isRanged)
        {
            DamageCollider weaponCollider = weapon.GetComponentInChildren<DamageCollider>();
            weaponCollider.damage = weaponStats.damage;
            weaponCollider.tenacity = weaponStats.tenacity;
            // hitEffectName ���ڿ��� �̿��� Resources.Load ������ ����Ʈ�� �ҷ��� ���� ����
            weaponCollider.hitEffect = weaponStats.hitEffect;
            weaponCollider.tag = isPlayer ? "PlayerWeapon" : "EnemyWeapon";
        }

        // 4) �÷��̾� ������ �ִϸ����� ��Ʈ�ѷ� �������̵� (overrideControllerName���� �ε� ����)
        if (isPlayer)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == ownerActorNumber)
            {
                // WeaponHolderSlot�� ���Ե� ĳ����/������Ʈ�� Animator
                // ��: (�� ��ũ��Ʈ �ٷ� ���� �ִ� Animator�� ����)
                var animator = GetComponentInParent<Animator>();

                // ���� ���� RuntimeAnimatorController�� �ٸ� ���
                if (weaponStats.weaponOverride != null)
                {
                    animator.runtimeAnimatorController = weaponStats.weaponOverride;
                }
            }
        }

        // 5) �θ� ���� (parentViewID�� ���� ���)
        if (parentViewID != -1)
        {
            if (pv != null)
            {
                weapon.transform.SetParent(pv.transform, false);
            }
        }
        else
        {
            // parentOverride�� ������, ���� ��ũ��Ʈ�� transform�� ���̰ų� etc.
            weapon.transform.SetParent(this.transform, false);
        }

        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;

        // 6) ���� ���� ����
        currentWeaponModel = weapon;
    }
}
