using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using Unity.VisualScripting;
using static PlayerInventory;

public class PlayerInventory : MonoBehaviour
{

    [System.Serializable]
    public class InventoryItem
    {
        public string itemDocId;      
        public string itemType;
        public string itemName;
        public int quantity;
        public float durability;
    }
    
    public List<InventoryItem> inventory = new List<InventoryItem>();

    void Start()
    {

        // 일정 주기로 Firebase가 준비되었는지 검사
        InvokeRepeating(nameof(CheckFirebaseReady), 0.5f, 0.5f);
        
        LoadInventoryFromFirestore();

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
        
        foreach (Item item in InventoryController.Instance.inventory)
        {
            
            itemToInventoryItem(item);
        }

        InventorySynchronizeToDB();
    }

    public void LoadInventoryFromFirestore()
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

        CollectionReference inventoryRef = db.Collection("Users")
                                             .Document(user.UserId)
                                             .Collection("Inventory");

        inventoryRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to load inventory: " + task.Exception);
                return;
            }
            var snapshot = task.Result;
            inventory.Clear();

            foreach (var doc in snapshot.Documents)
            {
                Dictionary<string, object> dict = doc.ToDictionary();

                InventoryItem item = new InventoryItem();
                item.itemDocId = doc.Id;
                item.itemType = dict["itemType"].ToString();
                item.itemName = dict["itemName"].ToString();
                item.quantity = System.Convert.ToInt32(dict["quantity"]);
                item.durability = float.Parse(dict["durability"].ToString());

                InventoryController.Instance.LoadInventoryItem(item);
            }

        });
    }


    public void AddItemToInventory(string itemName, string itemType, int addQuantity = 1, float addDurability = 1f)
    {
        Debug.Log($"itemType {itemType}");
        if (itemType.Equals("Equipment"))
        {
            
            for (int i = 0; i < addQuantity; i++)
            {
                string uniqueId = System.Guid.NewGuid().ToString();

                InventoryItem newEquip = new InventoryItem()
                {
                    itemDocId = uniqueId,
                    itemName = itemName,
                    quantity = 1,           
                    durability = addDurability
                };
                inventory.Add(newEquip);
            }
        }
        else
        {
            
            InventoryItem existing = inventory.Find(i => i.itemName == itemName);
            if (existing != null)
            {
                
                existing.quantity += addQuantity;
            }
            else
            {
                
                string uniqueId = System.Guid.NewGuid().ToString();

                
                InventoryItem newItem = new InventoryItem()
                {
                    itemDocId = uniqueId,
                    itemName = itemName,
                    itemType = itemType,
                    quantity = addQuantity,
                    durability = addDurability
                };
                Debug.Log($"AddItem : {newItem.itemName}, {newItem.durability}");
                inventory.Add(newItem);
            }
        }
    }

    private void itemToInventoryItem(Item item)
    {
        AddItemToInventory(item.itemData.name, item.itemData.itemType, item.quantity, item.durability);
    }

    public void InventorySynchronizeToDB()
    {
        var auth = FirebaseManager.Instance.Auth;
        var db = FirebaseManager.Instance.Db;
        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("No logged-in user.");
            return;
        }

        CollectionReference inventoryRef = db.Collection("Users")
            .Document(user.UserId)
            .Collection("Inventory");

        
        inventoryRef.GetSnapshotAsync().ContinueWithOnMainThread(loadTask =>
        {
            if (loadTask.IsFaulted)
            {
                Debug.LogError("Failed to load DB inventory: " + loadTask.Exception);
                return;
            }

            QuerySnapshot snapshot = loadTask.Result;
            HashSet<string> processedDocIds = new HashSet<string>();

            
            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                string docId = doc.Id;
                InventoryItem localItem = inventory.Find(i => i.itemDocId == docId);

                if (localItem == null)
                {
                    
                    inventoryRef.Document(docId).DeleteAsync();
                }
                else
                {
                    processedDocIds.Add(docId);
                    if (localItem.quantity <= 0)
                    {
                        inventoryRef.Document(docId).DeleteAsync();
                    }
                    else
                    {
                        Dictionary<string, object> updateData = new Dictionary<string, object>()
                    {
                        {"itemType", localItem.itemType},
                        {"itemName", localItem.itemName},
                        {"quantity", localItem.quantity},
                        {"durability", localItem.durability}
                    };

                        inventoryRef.Document(docId)
                                    .SetAsync(updateData, SetOptions.MergeAll)
                                    .ContinueWithOnMainThread(t =>
                                    {
                                        if (t.IsFaulted)
                                            Debug.LogError("Update failed: " + t.Exception);
                                        else
                                            Debug.Log($"[Sync] ������ ����: {docId} => qty={localItem.quantity}, dur={localItem.durability}");
                                    });
                    }
                }
            }

            
            foreach (InventoryItem localItem in inventory)
            {
                if (!processedDocIds.Contains(localItem.itemDocId))
                {
                    DocumentReference newDocRef = inventoryRef.Document(localItem.itemDocId);
                    Dictionary<string, object> newData = new Dictionary<string, object>()
                {
                    {"itemType", localItem.itemType},
                    {"itemName", localItem.itemName},
                    {"quantity", localItem.quantity},
                    {"durability", localItem.durability}
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

            Debug.Log("[Sync! (No Coroutine)");
        });
    }

}