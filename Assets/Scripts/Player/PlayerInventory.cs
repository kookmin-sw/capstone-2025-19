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
        //InventoryController.Instance.inventory; // <- Item �� ��� List��
        foreach(Item item in InventoryController.Instance.inventory)
        {
            //Item���� ��ȯ�� Inventory�� ����
            itemToInventoryItem(item);
        }
        //�� Item ���� InventoryItem���� ��ȯ�ؼ� FireBase�� �����ϸ� ��
        StartCoroutine("InventorySynchronizeToDB");
    }

    public void LoadInventoryFromFirestore()
    {
        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("������ null");
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
                item.itemDocId = doc.Id; // ���� ID
                item.itemType = dict["itemType"].ToString();
                item.itemName = dict["itemName"].ToString();
                item.quantity = System.Convert.ToInt32(dict["quantity"]);
                item.durability = float.Parse(dict["durability"].ToString());

                InventoryController.Instance.LoadInventoryItem(item);
            }
            Debug.Log("�κ��丮 �ҷ����� �Ϸ�! ������ ����: " + inventory.Count);
        });
    }


    public void AddItemToInventory(string itemName,string itemType, int addQuantity, float addDurability)
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
        //���Ƿ� �̱� ������ Durability�� ���� �ʿ� ����.
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
        if (user == null) yield break;  // �α��� �� ������ �ߴ�

        CollectionReference inventoryRef = db.Collection("Users")
                                             .Document(user.UserId)
                                             .Collection("Inventory");

        // 1) Firestore���� ���� �κ��丮(���� ���) �ҷ�����
        var loadTask = inventoryRef.GetSnapshotAsync();
        yield return new WaitUntil(() => loadTask.IsCompleted); // Task �Ϸ� ���

        if (loadTask.IsFaulted)
        {
            Debug.LogError("Failed to load DB inventory: " + loadTask.Exception);
            yield break;
        }

        QuerySnapshot snapshot = loadTask.Result;

        // DB �������� ó���ߴ��� �����ϱ� ����
        HashSet<string> processedDocIds = new HashSet<string>();

        // 2) Firestore�� �ִ� ������ ��ȸ �� ���ÿ� ���� �������� ���� or ���� �����ͷ� ������Ʈ
        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            string docId = doc.Id;
            // ���� �κ��丮���� ���� docId�� ���� �������� ã�´�
            InventoryItem localItem = inventory.Find(i => i.itemDocId == docId);

            if (localItem == null)
            {
                // ���ÿ��� ���µ� DB�� ���� �� ����
                Debug.Log($"[Sync] DB���� �����ϴ� ������ {docId} �� ���� ó��");
                inventoryRef.Document(docId).DeleteAsync();
            }
            else
            {
                // ���ÿ��� �����Ƿ�, �κ� ������Ʈ(Ȥ�� Set)�� �ֽ� ���� �ݿ�
                processedDocIds.Add(docId);

                if (localItem.quantity <= 0)
                {
                    // ������ 0���ϸ� ����
                    Debug.Log($"[Sync] {docId}�� ������ 0 ���� �� ����");
                    inventoryRef.Document(docId).DeleteAsync();
                }
                else
                {
                    // ������ �ʵ��(itemType, itemName ��)�� DB�� ���� ����
                    Dictionary<string, object> updateData = new Dictionary<string, object>() {
                    { "itemType", localItem.itemType },
                    { "itemName", localItem.itemName },
                    { "quantity", localItem.quantity },
                    { "durability", localItem.durability }
                };

                    // SetAsync(..., SetOptions.MergeAll) �� ���� �ʵ�� �߰�, �ִ� �ʵ�� ����
                    inventoryRef.Document(docId).SetAsync(updateData, SetOptions.MergeAll)
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

        // 3) ���ÿ� �ִµ� DB���� ������ ������(�� ������) �� �� ���� ����
        foreach (InventoryItem localItem in inventory)
        {
            // �̹� ó���� docId�� �Ѿ
            if (!processedDocIds.Contains(localItem.itemDocId))
            {
                // �� ���� ����
                Debug.Log($"[Sync] ���ÿ��� �ִ� ������ {localItem.itemDocId} �� ���� DB�� �߰�");
                DocumentReference newDocRef = inventoryRef.Document(localItem.itemDocId);

                // �ʵ� ����
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
                        Debug.Log($"[Sync] �� ������ ����: {localItem.itemDocId}, {localItem.itemName}, qty={localItem.quantity}");
                });
            }
        }

        Debug.Log("[Sync] �κ��丮 ����ȭ �Ϸ�!");
    }

    //public void DeleteItem(string itemName, int addQuantity)
    //{
    //    InventoryItem existing = inventory.Find(i => i.itemName == itemName);
    //    existing.quantity -= addQuantity;
    //    if(existing.quantity == 0)
    //    {
    //        inventory.Remove(existing);
    //        Debug.Log("������ "+existing.itemName+"�� "+existing.quantity+"�� ���ŵǾ����ϴ�.");
    //    }
    //}

    //private void printLIst(List<InventoryItem> inventory)
    //{
    //    Debug.Log("trigger test4");
    //    foreach (InventoryItem item in inventory) 
    //    {
    //        Debug.Log($"���� list�� ����� ������ : {item.itemName} , {item.quantity}");
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
