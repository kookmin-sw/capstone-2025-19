using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WeaponHolderSlot_M : WeaponHolderSlot
{
    PhotonView photonView;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void LoadWeaponModel(WeaponStats weaponStats)
    {
        /*UnloadWeaponAndDestroy();

        if (weaponStats == null)
        {
            UnloadWeapon();
            return;
        }
        GameObject weapon = null;

        weapon = PhotonNetwork.Instantiate($"Prefabs/PlayerWeapon/Multiplay/{weaponStats.weaponPrefab.name}", transform.position, transform.rotation);


        Debug.Log($"photon network test");
        int ownerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        if (photonView == null) { photonView = GetComponent<PhotonView>(); }
        int parentViewID = -1;
        Debug.Log($"transform name {gameObject.name}, parentOverride {parentOverride.name}");
        PhotonView parentPV = (parentOverride != null)
            ? parentOverride.GetComponent<PhotonView>()
            : transform.GetComponent<PhotonView>();
*/
        /*if (parentPV != null)
        {
            parentViewID = parentPV.ViewID;
        }
        bool islayer = (transform.root.CompareTag("Player"));
        photonView.RPC(nameof(LoadWeaponModelRPC), RpcTarget.All, ownerActorNumber, weaponStats.name, islayer,
             parentViewID);
*/
        UnloadWeaponAndDestroy();

        if (weaponStats == null)
        {
            UnloadWeapon();
            return;
        }
        GameObject weapon = null;

        weapon = Instantiate(weaponStats.weaponPrefab) as GameObject;
        if (photonView.IsMine)
        {
            if (!weaponStats.isRanged)
            {
                DamageCollider weaponCollider = weapon.GetComponentInChildren<DamageCollider>();
                weaponCollider.damage = weaponStats.damage;
                weaponCollider.tenacity = weaponStats.tenacity;
                weaponCollider.hitEffect = weaponStats.hitEffect;
                weaponCollider.tag = transform.root.tag == "Player" ? "PlayerWeapon" : "EnemyWeapon";
            }
        }
        

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

            if (animationHandler != null)
            {
                print("animator updated");
                animationHandler.UpdateOverride(weaponStats.weaponType);
            }
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = Vector3.one;
        }

        currentWeaponModel = weapon;






        if (photonView.IsMine && weaponStats != null)
        {
            // weaponType �Ǵ� ���� ID �� string/int �� ����
            photonView.RPC(
                nameof(RPC_LoadWeaponModel),
                RpcTarget.OthersBuffered,
                weaponStats.name
            );


        }
        //Internal_LoadWeapon(weaponStats);
    }

    protected override void Internal_LoadWeapon(WeaponStats weaponStats)
    {
        UnloadWeaponAndDestroy();
        if (weaponStats == null)
        {
            UnloadWeapon();
            return;
        }

        var weapon = Instantiate(weaponStats.weaponPrefab);

        weapon.transform.SetParent(parentOverride != null ? parentOverride : transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;
        currentWeaponModel = weapon;

        if (photonView.IsMine)
        {
            if (!weaponStats.isRanged)
            {
                DamageCollider weaponCollider = weapon.GetComponentInChildren<DamageCollider>();
                weaponCollider.damage = weaponStats.damage;
                weaponCollider.tenacity = weaponStats.tenacity;
                weaponCollider.hitEffect = weaponStats.hitEffect;
                weaponCollider.tag = transform.root.tag == "Player" ? "PlayerWeapon" : "EnemyWeapon";
            }
        }

        if (photonView.IsMine)
        {
            
        }
        if (animationHandler != null)
            animationHandler.UpdateOverride(weaponStats.weaponType);

    }

    public override void UnloadWeaponAndDestroy()
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
        
    }

    [PunRPC]
    private void RPC_LoadWeaponModel(string weaponTypeName)
    {
        // weaponTypeName ���� WeaponStats ã�ƿ���
        Debug.Log($"RPC Load weaponModel {weaponTypeName}");
        WeaponStats stats = Resources.Load<WeaponStats>($"WeaponStats/{weaponTypeName}");
        Debug.Log($"Load WeaponStats {stats}");
        Internal_LoadWeapon(stats);
    }

    /*[PunRPC]
    private void RPC_LoadAndOverride(string weaponType) 
    {
        WeaponStats weapon = Resources.Load<WeaponStats>($"WeaponStats/{weaponType}")
    }*/


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
        // 1) ???? ???? ????
        UnloadWeaponAndDestroy();
        Debug.Log($"photon network test2");
        // 2) ???? Instantiate
        GameObject weapon = null;

        // ?????? ??? ????
        weapon = PhotonNetwork.Instantiate(
            $"Prefabs/PlayerWeapon/Multiplay/{weaponStatsName}",
            Vector3.zero,
            Quaternion.identity
        );
        WeaponStats weaponStats = Resources.Load<WeaponStats>($"WeaponStats/{weaponStatsName}");
        /*else
        {
            // ????????(???) Instantiate
            // ???? Resources ???? ???? ?????? ???? ??
            var prefab = Resources.Load<GameObject>($"Prefabs/PlayerWeapon/Singleplay/{weaponPrefabName}");
            weapon = Instantiate(prefab);
        }*/

        // 3) ???? ?????? ?????? ?????? ???? ????
        if (!weaponStats.isRanged)
        {
            DamageCollider weaponCollider = weapon.GetComponentInChildren<DamageCollider>();
            weaponCollider.damage = weaponStats.damage;
            weaponCollider.tenacity = weaponStats.tenacity;
            // hitEffectName ??????? ????? Resources.Load ?????? ??????? ????? ???? ????
            weaponCollider.hitEffect = weaponStats.hitEffect;
            weaponCollider.tag = isPlayer ? "PlayerWeapon" : "EnemyWeapon";
        }

        // 4) ?��???? ?????? ???????? ?????? ????????? (overrideControllerName???? ?��? ????)
        if (isPlayer)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == ownerActorNumber)
            {
                // WeaponHolderSlot?? ????? ��????/????????? Animator
                // ??: (?? ?????? ??? ???? ??? Animator?? ????)
                var animator = GetComponentInParent<Animator>();

                // ???? ???? RuntimeAnimatorController?? ??? ???
                /*
                if (weaponStats.weaponOverride != null)
                {
                    animator.runtimeAnimatorController = weaponStats.weaponOverride;
                }*/
            }
        }

        // 5) ?��? ???? (parentViewID?? ???? ???)
        if (parentViewID != -1)
        {
            if (pv != null)
            {
                weapon.transform.SetParent(pv.transform, false);
            }
        }
        else
        {
            // parentOverride?? ??????, ???? ???????? transform?? ?????? etc.
            weapon.transform.SetParent(this.transform, false);
        }

        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;

        // 6) ???? ???? ????
        currentWeaponModel = weapon;
    }


    /*[PunRPC]
    protected void LoadWeaponModelRPC(
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
        *//*else
        {
            // ��������(�̱�) Instantiate
            // ���� Resources ���� ���� �ٸ��ٸ� �ٲ�� ��
            var prefab = Resources.Load<GameObject>($"Prefabs/PlayerWeapon/Singleplay/{weaponPrefabName}");
            weapon = Instantiate(prefab);
        }*//*

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
    }*/
}
