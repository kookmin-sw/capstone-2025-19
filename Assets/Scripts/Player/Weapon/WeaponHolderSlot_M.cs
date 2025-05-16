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
            // weaponType 또는 고유 ID 를 string/int 로 보냄
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
        // weaponTypeName 으로 WeaponStats 찾아오기
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

        // 4) ?÷???? ?????? ???????? ?????? ????????? (overrideControllerName???? ?ε? ????)
        if (isPlayer)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == ownerActorNumber)
            {
                // WeaponHolderSlot?? ????? ĳ????/????????? Animator
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

        // 5) ?θ? ???? (parentViewID?? ???? ???)
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
        *//*else
        {
            // 오프라인(싱글) Instantiate
            // 만약 Resources 폴더 구조 다르다면 바꿔야 함
            var prefab = Resources.Load<GameObject>($"Prefabs/PlayerWeapon/Singleplay/{weaponPrefabName}");
            weapon = Instantiate(prefab);
        }*//*

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
    }*/
}
