using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using static PlayerInventoryDB;
using System;
using static UnityEditor.Progress;

public class WeaponDbSync : MonoBehaviour
{
    [System.Serializable]
    public class WearableItemDB
    {
        public string itemDocId;
        public string itemType;
        public string itemName;
        public float durability;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 일정 주기로 Firebase가 준비되었는지 검사
        InvokeRepeating(nameof(CheckFirebaseReady), 0.5f, 0.5f);

        LoadWearableItemFromFirestore();
    }

    void CheckFirebaseReady()
    {
        // FirebaseManager 싱글턴이 있는지 + IsReady인지 체크
        if (FirebaseManager.Instance != null && FirebaseManager.Instance.IsReady)
        {
            CancelInvoke(nameof(CheckFirebaseReady));
            Debug.Log("In PlayerInventory, Firebase is Ok");
        }
    }

    private void OnApplicationQuit()
    {
        Item item = InventoryController.Instance.weaponPanel.GetWeaponItemIcon().item;
        Debug.Log($"Save weapon item {item}");
        WearableItemDB weapon = EquippedToInventory(item.itemData.name, item.itemData.itemType.ToString(), item.durability);
        SyncToDB(weapon);
    }

    public void LoadWearableItemFromFirestore()
    {
        var auth = FirebaseManager.Instance.Auth;
        var db = FirebaseManager.Instance.Db;
        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User is null");
            return;
        }
        Debug.Log($"userName : {user.UserId}");

        CollectionReference equippedRef = db.Collection("Users")
                                             .Document(user.UserId)
                                             .Collection("Equipped");

        equippedRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to load inventory: " + task.Exception);
                return;
            }
            var snapshot = task.Result;


            foreach (var doc in snapshot.Documents)
            {
                Dictionary<string, object> dict = doc.ToDictionary();

                WearableItemDB item = new WearableItemDB();
                item.itemDocId = doc.Id;
                item.itemType = dict["itemType"].ToString();
                item.itemName = dict["itemName"].ToString();
                //item.quantity = System.Convert.ToInt32(dict["quantity"]);
                item.durability = float.Parse(dict["durability"].ToString());

                InventoryController.Instance.LoadEquippedItem(item);
            }

        });
    }

    public WearableItemDB EquippedToInventory(string itemName, string itemType,  float addDurability = 1f)
    {
        string uniqueId = System.Guid.NewGuid().ToString();
        WearableItemDB newEquip = new WearableItemDB()
        {
            itemDocId = uniqueId,
            itemName = itemName,
            itemType = itemType,
            durability = addDurability
        };

        return newEquip;
    }

    private void SyncToDB(WearableItemDB equippedItem)
    {
        var auth = FirebaseManager.Instance.Auth;
        var db = FirebaseManager.Instance.Db;
        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("No logged-in user.");
            return;
        }

        CollectionReference equippedRef = db.Collection("Users")
                                             .Document(user.UserId)
                                             .Collection("Equipped");

        DocumentReference newDocRef = equippedRef.Document(equippedItem.itemDocId);
        Dictionary<string, object> newData = new Dictionary<string, object>()
                {
                    {"itemType", equippedItem.itemType},
                    {"itemName", equippedItem.itemName},
                    //{"quantity", localItem.quantity},
                    {"durability", equippedItem.durability}
                };

        newDocRef.SetAsync(newData).ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted)
                Debug.LogError("New item add failed: " + t.Exception);
            else
                Debug.Log("Sync 완료");

        });
    }
}
