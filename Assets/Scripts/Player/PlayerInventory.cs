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
        public string itemDocId;      // Firestore 문서 ID (AutoID or UniqueID)
        public string itemType;
        public string itemName;
        public int quantity;
        public float durability;
    }
    //InventoryController의 Inventory 변환용
    public List<InventoryItem> inventory = new List<InventoryItem>();

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
        // Firestore에서 인벤토리 불러오기 및 InventoryController의 Inventory에 저장
        LoadInventoryFromFirestore();

    }
    private void OnApplicationQuit()
    {
        Debug.Log($"OnApplicationQuit, count : {InventoryController.Instance.inventory.Count}");
        //InventoryController.Instance.inventory; // <- Item 이 담긴 List임
        foreach(Item item in InventoryController.Instance.inventory)
        {
            //Item들을 변환용 Inventory에 저장
            itemToInventoryItem(item);
        }
        //DB에 동기화
        InventorySynchronizeToDB();
    }

    public void LoadInventoryFromFirestore()
    {
        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("유저가 null");
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
                item.itemDocId = doc.Id; // 문서 ID
                item.itemType = dict["itemType"].ToString();
                item.itemName = dict["itemName"].ToString();
                item.quantity = System.Convert.ToInt32(dict["quantity"]);
                item.durability = float.Parse(dict["durability"].ToString());

                InventoryController.Instance.LoadInventoryItem(item);
            }
            Debug.Log("인벤토리 불러오기 완료! 아이템 개수: " + InventoryController.Instance.inventory.Count);
        });
    }


    public void AddItemToInventory(string itemName, string itemType, int addQuantity = 1, float addDurability = 1f)
    {
        Debug.Log($"itemType {itemType}");
        if (itemType.Equals("Equipment")) 
        {
            //장비의 경우 개별로 저장하는 방식 -> 논의 필요
            for (int i = 0; i < addQuantity; i++)
            {
                string uniqueId = System.Guid.NewGuid().ToString();

                InventoryItem newEquip = new InventoryItem()
                {
                    itemDocId = uniqueId,
                    itemName = itemName,
                    quantity = 1,           // 장비는 보통 1개 단위
                    durability = addDurability
                };
                inventory.Add(newEquip);
            }
        }
        else
        {
            // 1) 로컬 인벤토리에서 동일 이름의 아이템 찾기
            InventoryItem existing = inventory.Find(i => i.itemName == itemName);
            if (existing != null)
            {
                // 이미 인벤토리에 있으면 수량 갱신
                existing.quantity += addQuantity;
            }
            else
            {
                //고유 itemId
                string uniqueId = System.Guid.NewGuid().ToString();

                // 로컬 리스트에 추가
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

        // 1) Firestore에서 현재 인벤토리 문서 목록 가져오기
        inventoryRef.GetSnapshotAsync().ContinueWithOnMainThread(loadTask =>
        {
            if (loadTask.IsFaulted)
            {
                Debug.LogError("Failed to load DB inventory: " + loadTask.Exception);
                return;
            }

            QuerySnapshot snapshot = loadTask.Result;
            HashSet<string> processedDocIds = new HashSet<string>();

            // 2) DB 문서들 순회
            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                string docId = doc.Id;
                InventoryItem localItem = inventory.Find(i => i.itemDocId == docId);

                if (localItem == null)
                {
                    // 로컬에 없는데 DB만 있으면 삭제
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
                                            Debug.Log($"[Sync] 아이템 갱신: {docId} => qty={localItem.quantity}, dur={localItem.durability}");
                                    });
                    }
                }
            }

            // 3) 로컬에만 있고 DB에 없던 아이템 → 새 문서 생성
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
                            Debug.Log($"[Sync] 새 아이템 생성: {localItem.itemDocId}, {localItem.itemName}, qty={localItem.quantity}");
                    });
                }
            }

            Debug.Log("[Sync] 인벤토리 동기화 완료! (No Coroutine)");
        });
    }

}
