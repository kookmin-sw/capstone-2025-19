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

public class SignUpManger : MonoBehaviour
{
    public InputField email;
    public InputField password;
    public InputField nickname;
    public Button signUpButton;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(CheckFirebaseReady), 0.5f, 0.5f);
    }

    void CheckFirebaseReady()
    {
        if (FirebaseManager.Instance != null && FirebaseManager.Instance.IsReady)
        {
            CancelInvoke(nameof(CheckFirebaseReady));
        }
    }

    public void SignUp()
    {
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

                // Firestore에 유저 정보 생성
                DocumentReference docRef = db.Collection("Users").Document(newUser.UserId);
                Dictionary<string, object> userData = new Dictionary<string, object>()
                {
                    { "email", email.text },
                    { "nickname", nickname.text },
                    { "level", 1 },
                    { "exp", 0 },
                    { "money",0 },
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
                        SceneManager.LoadScene("Village");
                    }
                });
            });

    }

    // Update is called once per frame
    void Update()
    {
        if (nickname.Equals(""))
        {
            signUpButton.interactable = false;
        }
        else
        {
            signUpButton.interactable = true;
        }
    }
}
