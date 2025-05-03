using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirebaseManager : Singleton<FirebaseManager>
{
    
    // Firebase ����
    public FirebaseAuth Auth { get; private set; }
    public FirebaseFirestore Db { get; private set; }

    // �ʱ�ȭ ����
    public bool IsReady { get; private set; }

    public bool isLoginComplete = false;

    protected override void Awake()
    {
        base.Awake();
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        Debug.Log("Checking Firebase dependencies...");

        // Firebase ���Ӽ� üũ
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var result = task.Result;
            if (result != DependencyStatus.Available)
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {result}");
                IsReady = false;
                return;
            }

            // ���Ӽ� OK �� Firebase �ʱ�ȭ
            Debug.Log("Firebase dependencies are available. Initializing...");
            var app = FirebaseApp.DefaultInstance;

            Auth = FirebaseAuth.DefaultInstance;
            Db = FirebaseFirestore.DefaultInstance;

            IsReady = true;
            Debug.Log("Firebase Manager initialization complete!");
        });
    }
}
