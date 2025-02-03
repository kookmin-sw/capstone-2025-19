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

    //public static FirebaseApp firebaseApp;
    public static FirebaseAuth firebaseAuth;
    public static FirebaseFirestore firebaseFirestore;

    public static FirebaseUser User;

    // Start is called before the first frame update
    void Start()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;
        firebaseFirestore = FirebaseFirestore.DefaultInstance;
    }

    public void SignUp()
    {
        firebaseAuth.CreateUserWithEmailAndPasswordAsync(email.text, password.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception.ToString());
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("task is canceled");
            }
            else
            {
                User = task.Result.User;
                Debug.Log($"회원가입 성공! UID: {User.UserId}, Email: {User.Email}");

                DocumentReference userDocRef = firebaseFirestore
                .Collection("Users")
                .Document(User.UserId);

                Dictionary<string, object> userData = new Dictionary<string, object>()
                {
                    {"email", email.text},
                    {"nickname", nickname.text},
                    {"level", 1},
                    {"exp", 0},
                };

                userDocRef.SetAsync(userData).ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError($"Firestore 저장 실패: {task.Exception}");
                    }
                    else if (task.IsCanceled)
                    {
                        Debug.LogError("Firestore 저장 취소됨");
                    }
                    else
                    {
                        Debug.Log("Firestore에 유저 정보 저장 완료!");

                        SceneManager.LoadScene("Login");
                    }
                });
            }
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
