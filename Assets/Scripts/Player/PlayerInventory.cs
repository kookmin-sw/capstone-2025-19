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
    private FirebaseAuth auth;
    private FirebaseFirestore db;

    [System.Serializable]
    public class InventoryItem
    {
        public string itemDocId;      // Firestore ���� ID (AutoID or UniqueID)
        public string itemType;
        public string itemName;
        public int quantity;
        public float durability;
    }
    //InventoryController�� Inventory ��ȯ��
    public List<InventoryItem> inventory = new List<InventoryItem>();

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
        // Firestore���� �κ��丮 �ҷ����� �� InventoryController�� Inventory�� ����
        LoadInventoryFromFirestore();

    }
    private void OnApplicationQuit()
    {
        Debug.Log($"OnApplicationQuit, count : {InventoryController.Instance.inventory.Count}");
        //InventoryController.Instance.inventory; // <- Item �� ��� List��
        foreach(Item item in InventoryController.Instance.inventory)
        {
            //Item���� ��ȯ�� Inventory�� ����
            itemToInventoryItem(item);
        }
        //DB�� ����ȭ
        InventorySynchronizeToDB();
    }

    public void LoadInventoryFromFirestore()
    {
        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("������ null");
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
                item.itemDocId = doc.Id; // ���� ID
                item.itemType = dict["itemType"].ToString();
                item.itemName = dict["itemName"].ToString();
                item.quantity = System.Convert.ToInt32(dict["quantity"]);
                item.durability = float.Parse(dict["durability"].ToString());

                InventoryController.Instance.LoadInventoryItem(item);
            }
            Debug.Log("�κ��丮 �ҷ����� �Ϸ�! ������ ����: " + InventoryController.Instance.inventory.Count);
        });
    }


    public void AddItemToInventory(string itemName, string itemType, int addQuantity = 1, float addDurability = 1f)
    {
        Debug.Log($"itemType {itemType}");
        if (itemType.Equals("Equipment")) 
        {
            //����� ��� ������ �����ϴ� ��� -> ���� �ʿ�
            for (int i = 0; i < addQuantity; i++)
            {
                string uniqueId = System.Guid.NewGuid().ToString();

                InventoryItem newEquip = new InventoryItem()
                {
                    itemDocId = uniqueId,
                    itemName = itemName,
                    quantity = 1,           // ���� ���� 1�� ����
                    durability = addDurability
                };
                inventory.Add(newEquip);
            }
        }
        else
        {
            // 1) ���� �κ��丮���� ���� �̸��� ������ ã��
            InventoryItem existing = inventory.Find(i => i.itemName == itemName);
            if (existing != null)
            {
                // �̹� �κ��丮�� ������ ���� ����
                existing.quantity += addQuantity;
            }
            else
            {
                //���� itemId
                string uniqueId = System.Guid.NewGuid().ToString();

                // ���� ����Ʈ�� �߰�
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
        AddItemToInventory(item.itemData.name, item.itemData.itemType, item.quantity, item.itemDurability);
    }

    public void InventorySynchronizeToDB()
    {
        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("No logged-in user.");
            return;
        }

        CollectionReference inventoryRef = db.Collection("Users")
            .Document(user.UserId)
            .Collection("Inventory");

        // 1) Firestore���� ���� �κ��丮 ���� ��� ��������
        inventoryRef.GetSnapshotAsync().ContinueWithOnMainThread(loadTask =>
        {
            if (loadTask.IsFaulted)
            {
                Debug.LogError("Failed to load DB inventory: " + loadTask.Exception);
                return;
            }

            QuerySnapshot snapshot = loadTask.Result;
            HashSet<string> processedDocIds = new HashSet<string>();

            // 2) DB ������ ��ȸ
            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                string docId = doc.Id;
                InventoryItem localItem = inventory.Find(i => i.itemDocId == docId);

                if (localItem == null)
                {
                    // ���ÿ� ���µ� DB�� ������ ����
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

            // 3) ���ÿ��� �ְ� DB�� ���� ������ �� �� ���� ����
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
                            Debug.Log($"[Sync] �� ������ ����: {localItem.itemDocId}, {localItem.itemName}, qty={localItem.quantity}");
                    });
                }
            }

            Debug.Log("[Sync] �κ��丮 ����ȭ �Ϸ�! (No Coroutine)");
        });
    }

}
