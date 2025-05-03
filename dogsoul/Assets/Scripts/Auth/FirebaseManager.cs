using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirebaseManager : Singleton<FirebaseManager>
{
    
    // Firebase 참조
    public FirebaseAuth Auth { get; private set; }
    public FirebaseFirestore Db { get; private set; }

    // 초기화 여부
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

        // Firebase 종속성 체크
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var result = task.Result;
            if (result != DependencyStatus.Available)
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {result}");
                IsReady = false;
                return;
            }

            // 종속성 OK → Firebase 초기화
            Debug.Log("Firebase dependencies are available. Initializing...");
            var app = FirebaseApp.DefaultInstance;

            Auth = FirebaseAuth.DefaultInstance;
            Db = FirebaseFirestore.DefaultInstance;

            IsReady = true;
            Debug.Log("Firebase Manager initialization complete!");
        });
    }
}
