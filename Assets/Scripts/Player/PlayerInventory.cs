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
        //InventoryController.Instance.inventory; // <- Item 이 담긴 List임
        foreach(Item item in InventoryController.Instance.inventory)
        {
            //Item들을 변환용 Inventory에 저장
            itemToInventoryItem(item);
        }
        //이 Item 들을 InventoryItem으로 변환해서 FireBase에 저장하면 됨
        StartCoroutine("InventorySynchronizeToDB");
    }

    public void LoadInventoryFromFirestore()
    {
        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("유저가 null");
            return;
        }

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
            Debug.Log("인벤토리 불러오기 완료! 아이템 개수: " + inventory.Count);
        });
    }


    public void AddItemToInventory(string itemName,string itemType, int addQuantity, float addDurability)
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
        //포션류 이기 때문에 Durability가 별로 필요 없음.
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
                inventory.Add(newItem);
            }
        }
    }

    private void itemToInventoryItem(Item item)
    {
        if(item != null)
        {
            AddItemToInventory(item.itemData.name, item.itemData.itemType, item.quantity, item.itemDurability);
        }
    }

    private IEnumerator InventorySynchronizeToDB()
    {
        var user = auth.CurrentUser;
        if (user == null) yield break;  // 로그인 안 됐으면 중단

        CollectionReference inventoryRef = db.Collection("Users")
                                             .Document(user.UserId)
                                             .Collection("Inventory");

        // 1) Firestore에서 현재 인벤토리(문서 목록) 불러오기
        var loadTask = inventoryRef.GetSnapshotAsync();
        yield return new WaitUntil(() => loadTask.IsCompleted); // Task 완료 대기

        if (loadTask.IsFaulted)
        {
            Debug.LogError("Failed to load DB inventory: " + loadTask.Exception);
            yield break;
        }

        QuerySnapshot snapshot = loadTask.Result;

        // DB 문서들을 처리했는지 추적하기 위해
        HashSet<string> processedDocIds = new HashSet<string>();

        // 2) Firestore에 있는 문서들 순회 → 로컬에 없는 아이템은 삭제 or 로컬 데이터로 업데이트
        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            string docId = doc.Id;
            // 로컬 인벤토리에서 같은 docId를 가진 아이템을 찾는다
            InventoryItem localItem = inventory.Find(i => i.itemDocId == docId);

            if (localItem == null)
            {
                // 로컬에는 없는데 DB만 존재 → 삭제
                Debug.Log($"[Sync] DB에만 존재하는 아이템 {docId} → 삭제 처리");
                inventoryRef.Document(docId).DeleteAsync();
            }
            else
            {
                // 로컬에도 있으므로, 부분 업데이트(혹은 Set)로 최신 상태 반영
                processedDocIds.Add(docId);

                if (localItem.quantity <= 0)
                {
                    // 수량이 0이하면 삭제
                    Debug.Log($"[Sync] {docId}의 수량이 0 이하 → 삭제");
                    inventoryRef.Document(docId).DeleteAsync();
                }
                else
                {
                    // 나머지 필드들(itemType, itemName 등)도 DB에 맞춰 저장
                    Dictionary<string, object> updateData = new Dictionary<string, object>() {
                    { "itemType", localItem.itemType },
                    { "itemName", localItem.itemName },
                    { "quantity", localItem.quantity },
                    { "durability", localItem.durability }
                };

                    // SetAsync(..., SetOptions.MergeAll) → 없는 필드는 추가, 있는 필드는 갱신
                    inventoryRef.Document(docId).SetAsync(updateData, SetOptions.MergeAll)
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

        // 3) 로컬에 있는데 DB에는 없었던 아이템(새 아이템) → 새 문서 생성
        foreach (InventoryItem localItem in inventory)
        {
            // 이미 처리한 docId는 넘어감
            if (!processedDocIds.Contains(localItem.itemDocId))
            {
                // 새 문서 생성
                Debug.Log($"[Sync] 로컬에만 있는 아이템 {localItem.itemDocId} → 새로 DB에 추가");
                DocumentReference newDocRef = inventoryRef.Document(localItem.itemDocId);

                // 필드 구성
                Dictionary<string, object> newData = new Dictionary<string, object>()
            {
                { "itemType", localItem.itemType },
                { "itemName", localItem.itemName },
                { "quantity", localItem.quantity },
                { "durability", localItem.durability }
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

        Debug.Log("[Sync] 인벤토리 동기화 완료!");
    }

    //public void DeleteItem(string itemName, int addQuantity)
    //{
    //    InventoryItem existing = inventory.Find(i => i.itemName == itemName);
    //    existing.quantity -= addQuantity;
    //    if(existing.quantity == 0)
    //    {
    //        inventory.Remove(existing);
    //        Debug.Log("아이템 "+existing.itemName+"이 "+existing.quantity+"개 제거되었습니다.");
    //    }
    //}

    //private void printLIst(List<InventoryItem> inventory)
    //{
    //    Debug.Log("trigger test4");
    //    foreach (InventoryItem item in inventory) 
    //    {
    //        Debug.Log($"현재 list에 저장된 아이템 : {item.itemName} , {item.quantity}");
    //    }
    //}

    //private void LoadItem()
    //{
    //    foreach(InventoryItem inventoryItem in inventory)
    //    {
    //        InventoryController.Instance.LoadInventoryItem(inventoryItem);
    //    }
    //}




}
