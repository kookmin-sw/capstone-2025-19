using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System;

public class AccountManager : MonoBehaviour
{

    [SerializeField] InputField email;
    [SerializeField] InputField password;
    [SerializeField] InputField nickname;
    [SerializeField] Button signInButton;
    [SerializeField] Button createAccountButton;

    private bool isLoginProcess;

    [SerializeField] GameObject VillageManager;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(CheckFirebaseReady), 0.5f, 0.5f);
        isLoginProcess = true;
        nickname.gameObject.SetActive(false);
    }

    void CheckFirebaseReady()
    {
        if (FirebaseManager.Instance != null && FirebaseManager.Instance.IsReady)
        {
            CancelInvoke(nameof(CheckFirebaseReady));
        }
    }

    void Update()
    {
        if (!isLoginProcess)
        {
            checkNickname();
        }
    }

    public void SignUp()
    {
        if (isLoginProcess)
        {
            Debug.Log("계정 생성 버튼 처음 눌림");
            nickname.gameObject.SetActive(true);
            signInButton.gameObject.SetActive(false);
            isLoginProcess = false;
            return;
        }

        if (!FirebaseManager.Instance.IsReady)
        {
            Debug.LogError("Firebase not ready!");
            return;
        }
        var auth = FirebaseManager.Instance.Auth;
        var db = FirebaseManager.Instance.Db;

        auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError(task.Exception);
                    return;
                }

                FirebaseUser newUser = task.Result.User;
                Debug.Log($"회원가입 성공! UID: {newUser.UserId}");
                FirebaseManager.Instance.isLoginComplete = true;
                

                // Firestore에 유저 정보 생성
                DocumentReference docRef = db.Collection("Users").Document(newUser.UserId);
                Dictionary<string, object> userData = new Dictionary<string, object>()
                {
                    { "email", email.text },
                    { "nickname", nickname.text },
                };

                docRef.SetAsync(userData).ContinueWithOnMainThread(writeTask =>
                {
                    if (writeTask.IsFaulted)
                    {
                        Debug.LogError("Firestore save failed: " + writeTask.Exception);
                    }
                    else
                    {
                        Debug.Log("Firestore에 유저 정보 저장 완료!");
                        VillageManager.GetComponent<VillageManager>().SpawnPlayer();
                    }
                });
            });
        
    }

    public void SignIn()
    {
        if (!FirebaseManager.Instance.IsReady)
        {
            Debug.LogError("Firebase is not ready yet!");
            return;
        }

        var auth = FirebaseManager.Instance.Auth;

        auth.SignInWithEmailAndPasswordAsync(email.text, password.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Sign in failed: " + task.Exception);
                }
                else
                {
                    var user = task.Result.User;
                    Debug.Log($"Sign in success! UID: {user.UserId}");
                    FirebaseManager.Instance.isLoginComplete = true;
                    VillageManager.GetComponent<VillageManager>().SpawnPlayer();
                }
            });     
    }

    

    private void checkNickname()
    {
        if (nickname.Equals(""))
        {
            createAccountButton.interactable = false;
        }
        else
        {
            createAccountButton.interactable = true;
        }
    }
}
