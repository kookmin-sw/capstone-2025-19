using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Firebase.Firestore;
using PlayerControl;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using static WareHouseDB;
using static PlayerInventoryDB;

public class VillageManager : MonoBehaviour
{
    [Header("Player Spawn")]
    [SerializeField] Transform playerSpawnPosition;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject LoginCanvas;
    [SerializeField] GameObject Hp_Sp_Canvas;
    [SerializeField] CinemachineVirtualCamera loginVirtualCamera;
    [HideInInspector]
    public GameObject playerFollowCamera;

    [System.Serializable]
    public class DBItem
    {
        public string itemDocId;
        public string itemType;
        public string itemName;
        public int quantity;
        public float durability;
    }

    public class Status
    {
        public int level;
        public int exp;
        public float ap;
        public float sp;
        public float hp;
        public float wp;
    }


    [Header("Inventory")]
    [HideInInspector]
    public List<DBItem> inventory = new List<DBItem>();
    [HideInInspector]
    public List<DBItem> wareHouseList = new List<DBItem>();
    [HideInInspector]
    public List<DBItem> equippedItemList = new List<DBItem>();


    void Start()
    {
        Hp_Sp_Canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Login_PlayerSpawn
    public void CloseLoginCanvas()
    {
        LoginCanvas.SetActive(false);
        Debug.Log("LoginCanvas 지우기 완료");
        Hp_Sp_Canvas.SetActive(true);
    }

    public void SpawnPlayer()
    {
        Quaternion spawnRotation = Quaternion.Euler(0f, 200f, 0f);
        GameObject player = Instantiate(Resources.Load<GameObject>($"Prefabs/Player/Player_Village"), playerSpawnPosition.position, spawnRotation);
        InventoryController.Instance.SetPlayer(player.GetComponentInChildren<PlayerTrigger>());
        InventoryController.Instance.SetPlayerController(player.GetComponent<PlayerControl.PlayerController>());
        //TODO Player MainCamera 생성
        //GameObject mainCamera = Instantiate(Resources.Load<GameObject>($"Prefabs/Camera/MainCamera"));
        playerFollowCamera = Instantiate(Resources.Load<GameObject>("Prefabs/Camera/PlayerFollowCamera"));
        CinemachineVirtualCamera virtualCamera = playerFollowCamera.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = player.transform.Find("PlayerCameraRoot");
        player.GetComponent<PlayerController>().SetMainCamera(mainCamera);
        //player.GetComponent<PlayerController>().SetCinemachineTarget(player.transform.Find("PlayerCameraRoot").gameObject);
        //카메라 캐릭터에게로 회전
        OnLoginComplete(); 
        CloseLoginCanvas();
    }

    public void OnLoginComplete()
    {
        playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Priority = 11;
        loginVirtualCamera.Priority = 1;
    }
    #endregion

    /// <summary>
    /// Firebase Sync Util
    /// </summary>
    // Firebase 
    public void SynchronizeDBtoCash()
    {
        //Firebase variable
        var auth = FirebaseManager.Instance.Auth;
        var db = FirebaseManager.Instance.Db;
        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User is null");
            return;
        }
        GetPlayerNickname(auth, db, user);

        // Synchronize PlayerStatus
        PlayerStatusDBtoCash(auth, db, user);

        // Synchronize PlayerQuest
        //QuestDBtoCash(auth, db, user);
        
        // Synchronize money 
        MoneyDBtoCash(auth, db, user);
        
        // Synchronize PlayerInventory 
        InventoryDBtoCash(auth, db, user);
        
        // Synchronize Weapon DB
        EquippedItemDBtoCash(auth, db, user);
        
        // Synchronize WareHouse DB
        WareHouseDBtoCash(auth, db, user);
    }

    private void OnApplicationQuit()
    {
        //Firebase variable
        var auth = FirebaseManager.Instance.Auth;
        var db = FirebaseManager.Instance.Db;
        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User is null");
            return;
        }

        // Synchronize PlayerStatus
        PlayerStatusCashtoDB(auth, db, user);

        // Synchronize PlayerQuest
        //QuestCashtoDB(auth, db, user);

        // Synchronize money 
        MoneyCashtoDB(auth, db, user);

        // Synchronize PlayerInventory 
        InventoryCashtoDB(auth, db, user);

        // Synchronize Weapon DB
        EquippedItemCashtoDB(auth, db, user);

        // Synchronize WareHouse DB
        WareHouseCashtoDB(auth, db, user);
    }

    //Get UserNickname
    public void GetPlayerNickname(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
        DocumentReference userRef = db.Collection("Users")
                                             .Document(user.UserId);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to load User: " + task.Exception);
                return;
            }

            var doc = task.Result;

            Dictionary<string, object> dict = doc.ToDictionary();
            PlayerStatusController.Instance.playerNickname = dict["nickname"].ToString();

            Debug.Log($"{PlayerStatusController.Instance.playerNickname}-------------------");
        });

    }

    // PlayerStatus Sync
    #region PlayerStatus
    public void PlayerStatusDBtoCash(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
        DocumentReference StatusRef = db.Collection("Users")
                                             .Document(user.UserId)
                                             .Collection("Status")
                                             .Document("StatusDoc");

        StatusRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to load PlayerStatus: " + task.Exception);
                return;
            }
            var doc = task.Result;

            if (!doc.Exists)
            {
                Debug.Log("No status document!");
                Status initStat = new Status();
                initStat.level = 1;
                initStat.exp = 0;
                initStat.ap = 10;
                initStat.sp = 100;
                initStat.hp = 10;
                initStat.wp = 10;
                PlayerStatusController.Instance.LoadPlayerStatus(initStat);
                return;
            }

            Dictionary<string, object> dict = doc.ToDictionary();

            Status stat = new Status();
            stat.level = System.Convert.ToInt32(dict["level"]);
            stat.exp = System.Convert.ToInt32(dict["exp"]);
            stat.ap = float.Parse(dict["ap"].ToString());
            stat.sp = float.Parse(dict["sp"].ToString());
            stat.hp = float.Parse(dict["hp"].ToString());
            stat.wp = float.Parse(dict["wp"].ToString());

            PlayerStatusController.Instance.LoadPlayerStatus(stat);
        });
    }

    public void PlayerStatusCashtoDB(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
        VillageManager.Status currentStatus = PlayerStatusController.Instance.GetPlayerStatus();

        Dictionary<string, object> statusData = new Dictionary<string, object>()
    {
        { "level", currentStatus.level },
        { "exp", currentStatus.exp },
        { "hp", currentStatus.hp },
        { "sp", currentStatus.sp },
        { "ap", currentStatus.ap },
        { "wp", currentStatus.wp }
    };

        DocumentReference StatusRef = db.Collection("Users")
                                        .Document(user.UserId)
                                        .Collection("Status")
                                        .Document("StatusDoc");

        StatusRef.SetAsync(statusData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to save status: " + task.Exception);
            }
            else
            {
                Debug.Log("Status saved successfully!");
            }
        });
    }
    #endregion;

    //PlayerQuest Sync
    #region PlayerQuest
    public void QuestCashtoDB(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
        CollectionReference questCollection = db.Collection("Users").Document(user.UserId).Collection("Quests");

        // 기존 퀘스트 문서 삭제
        questCollection.GetSnapshotAsync().ContinueWithOnMainThread(deleteTask =>
        {
            if (deleteTask.IsFaulted)
            {
                Debug.LogError("QuestCashtoDB 실패: " + deleteTask.Exception);
            }
            else
            {
                QuerySnapshot snapshot = deleteTask.Result;
                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    doc.Reference.DeleteAsync();
                }

                // 현재 남아있는 퀘스트 정보를 저장
                foreach (QuestList questList in QuestManager.Instance.playerQuestList)
                {
                    Quest quest = questList.Quest;
                    Dictionary<string, object> questData = new Dictionary<string, object>()
                    {
                        { "id", quest.id },
                        { "title", quest.title },
                        { "type", quest.type.ToString() },
                        { "target", quest.target },
                        { "score", quest.score },
                        { "result", quest.result },
                        { "reward", quest.reward },
                        { "progress", quest.progress },
                        { "isCompleted", quest.isCompleted }
                    };

                    // quest.id를 문서 ID로 사용
                    DocumentReference questDoc = questCollection.Document(quest.id);
                    questDoc.SetAsync(questData).ContinueWithOnMainThread(writeTask =>
                    {
                        if (writeTask.IsFaulted)
                        {
                            Debug.LogError("퀘스트 저장 실패: " + writeTask.Exception);
                        }
                        else
                        {
                            Debug.Log("퀘스트 저장 완료: " + quest.title);
                        }
                    });
                }
            }
        });
    }

    public void QuestDBtoCash(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
        QuestManager.Instance.playerQuestList.Clear();
        CollectionReference questCollection = db.Collection("Users").Document(user.UserId).Collection("Quests");
        questCollection.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("퀘스트 불러오기 실패: " + task.Exception);
            }
            else
            {
                QuerySnapshot snapshot = task.Result;
                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    Dictionary<string, object> questData = doc.ToDictionary();

                    string questTitle = questData["title"].ToString();
                    string questTypeStr = questData["type"].ToString();
                    Quest.QuestType questType = (Quest.QuestType)System.Enum.Parse(typeof(Quest.QuestType), questTypeStr);
                    string target = questData["target"].ToString();
                    int score = int.Parse(questData["score"].ToString());
                    string result = questData["result"].ToString();
                    int reward = int.Parse(questData["reward"].ToString());
                    int progress = int.Parse(questData["progress"].ToString());
                    bool isCompleted = bool.Parse(questData["isCompleted"].ToString());

                    Quest loadedQuest = new Quest(questTitle, questType, target, score, result, reward);
                    loadedQuest.progress = progress;
                    loadedQuest.isCompleted = isCompleted;

                    // 불러온 퀘스트를 UI에 추가
                    QuestManager.Instance.AddQuestToUI(loadedQuest);
                }
                Debug.Log("Firestore에서 퀘스트 불러오기 완료!");
            }
        });
    }
    #endregion

    // Money Sync
    #region Money
    public void MoneyDBtoCash(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
        DocumentReference moneyRef = db.Collection("Users").Document(user.UserId);
        moneyRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to load money: " + task.Exception);
                return;
            }
            var snapshot = task.Result;
            if (snapshot.Exists && snapshot.ContainsField("money"))
            {
                int money = Convert.ToInt32(snapshot.GetValue<long>("money"));
                InventoryController.Instance.money = money;
                Debug.Log("Money loaded: " + money);
            }
            else
            {
                Debug.Log("No money field in user document. Setting money to 0.");
                InventoryController.Instance.money = 0;
            }
        });
    }

    public void MoneyCashtoDB(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
        DocumentReference moneyRef = db.Collection("Users").Document(user.UserId);
        Dictionary<string, object> moneyData = new Dictionary<string, object>()
        {
            {"money", InventoryController.Instance.money}
        };

        moneyRef.SetAsync(moneyData, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
                Debug.LogError("Failed to sync money: " + task.Exception);
            else
                Debug.Log("Money synchronized to DB: " + InventoryController.Instance.money);
        });
    }
    #endregion

    //Inventory sync
    #region Inventory
    public void InventoryDBtoCash(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
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

                DBItem item = new DBItem();
                item.itemDocId = doc.Id;
                item.itemType = dict["itemType"].ToString();
                item.itemName = dict["itemName"].ToString();
                item.quantity = System.Convert.ToInt32(dict["quantity"]);
                item.durability = float.Parse(dict["durability"].ToString());

                InventoryController.Instance.LoadInventoryItem(item);
            }

        });
    }

    public void InventoryCashtoDB(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
        //InventoryController.Instance.inventory -> VillageManager.inventory
        foreach (Item item in InventoryController.Instance.inventory)
        {
            AddItemToVillageManagerCashList(inventory, item.itemData.name, item.itemData.itemType.ToString(), item.quantity, item.durability);
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
                DBItem localItem = inventory.Find(i => i.itemDocId == docId);

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


            foreach (DBItem localItem in inventory)
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
    #endregion

    //Inventory, warehouse func
    #region Inventory_warehouse_func
    public void AddItemToVillageManagerCashList(List<DBItem> list, string itemName, string itemType, int addQuantity = 1, float addDurability = 1f)
    {
        Debug.Log($"itemType {itemType}");
        if (itemType.Equals("Equipment"))
        {

            for (int i = 0; i < addQuantity; i++)
            {
                string uniqueId = System.Guid.NewGuid().ToString();

                DBItem newEquip = new DBItem()
                {
                    itemDocId = uniqueId,
                    itemName = itemName,
                    quantity = 1,
                    durability = addDurability
                };
                list.Add(newEquip);
            }
        }
        else
        {

            DBItem existing = list.Find(i => i.itemName == itemName);
            if (existing != null)
            {

                existing.quantity += addQuantity;
            }
            else
            {

                string uniqueId = System.Guid.NewGuid().ToString();


                DBItem newItem = new DBItem()
                {
                    itemDocId = uniqueId,
                    itemName = itemName,
                    itemType = itemType,
                    quantity = addQuantity,
                    durability = addDurability
                };
                Debug.Log($"AddItem : {newItem.itemName}, {newItem.durability}");
                list.Add(newItem);
            }
        }
    }
    #endregion

    // EquippedItemDB
    #region EquippedItem
    public void EquippedItemDBtoCash(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
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

                DBItem item = new DBItem();
                item.itemDocId = doc.Id;
                item.itemType = dict["itemType"].ToString();
                item.itemName = dict["itemName"].ToString();
                item.quantity = System.Convert.ToInt32(dict["quantity"]);
                item.durability = float.Parse(dict["durability"].ToString());

                InventoryController.Instance.LoadEquippedItem(item);
            }

        });
    }

    public void EquippedItemCashtoDB(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
        if(InventoryController.Instance.weaponPanel.GetWeaponItemIcon() != null)
        {
            Item item = InventoryController.Instance.weaponPanel.GetWeaponItemIcon().item;
            AddItemToVillageManagerCashList(equippedItemList, item.itemData.name, item.itemData.itemType.ToString(), item.quantity, item.durability);
        }
   

        //현재는 장착 아이템은 하나
        //DBItem equippedItem = equippedItemList[0];

        CollectionReference equippedRef = db.Collection("Users")
                                             .Document(user.UserId)
                                             .Collection("Equipped");

        equippedRef.GetSnapshotAsync().ContinueWithOnMainThread(loadTask =>
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
                DBItem localItem = equippedItemList.Find(i => i.itemDocId == docId);

                if (localItem == null)
                {
                    equippedRef.Document(docId).DeleteAsync();
                }
                else
                {
                    processedDocIds.Add(docId);
                    if (localItem.quantity <= 0)
                    {
                        equippedRef.Document(docId).DeleteAsync();
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

                        equippedRef.Document(docId)
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


            foreach (DBItem localItem in equippedItemList)
            {
                if (!processedDocIds.Contains(localItem.itemDocId))
                {
                    DocumentReference newDocRef = equippedRef.Document(localItem.itemDocId);
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

        });

    }
    #endregion

    // WareHouseDB Sync
    #region WarehouseDB
    public void WareHouseCashtoDB(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
        //InventoryController.Instance.warehouse -> VillageManager.warehouseList
        foreach (Item item in InventoryController.Instance.wareHouse)
        {
            AddItemToVillageManagerCashList(wareHouseList, item.itemData.name, item.itemData.itemType.ToString(), item.quantity, item.durability);
        }

        CollectionReference inventoryRef = db.Collection("Users")
            .Document(user.UserId)
            .Collection("WareHouse");


        inventoryRef.GetSnapshotAsync().ContinueWithOnMainThread(loadTask =>
        {
            if (loadTask.IsFaulted)
            {
                Debug.LogError("Failed to load DB WareHouse: " + loadTask.Exception);
                return;
            }

            QuerySnapshot snapshot = loadTask.Result;
            HashSet<string> processedDocIds = new HashSet<string>();


            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                string docId = doc.Id;
                DBItem localItem = wareHouseList.Find(i => i.itemDocId == docId);

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


            foreach (DBItem localItem in wareHouseList)
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


    public void WareHouseDBtoCash(FirebaseAuth auth, FirebaseFirestore db, FirebaseUser user)
    {
        CollectionReference inventoryRef = db.Collection("Users")
                                             .Document(user.UserId)
                                             .Collection("WareHouse");

        inventoryRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to load WareHouse: " + task.Exception);
                return;
            }
            var snapshot = task.Result;
            wareHouseList.Clear();

            foreach (var doc in snapshot.Documents)
            {
                Dictionary<string, object> dict = doc.ToDictionary();

                VillageManager.DBItem item = new VillageManager.DBItem();
                item.itemDocId = doc.Id;
                item.itemType = dict["itemType"].ToString();
                item.itemName = dict["itemName"].ToString();
                item.quantity = System.Convert.ToInt32(dict["quantity"]);
                item.durability = float.Parse(dict["durability"].ToString());

                InventoryController.Instance.LoadWareHouseItem(item);
            }

        });
        Debug.Log("Village 창고 동기화 완료");
    }
    #endregion

}
