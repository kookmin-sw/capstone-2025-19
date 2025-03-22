using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using Firebase.Firestore;

public class SignInManager : MonoBehaviour
{
    public bool IsSignInOnProgress { get; private set; }


    public InputField email;
    public InputField password;
    public Button loginButton;
    public Button signUpButton;


    void Start()
    {
        loginButton.interactable = false;

        // 일정 주기로 Firebase가 준비되었는지 검사
        InvokeRepeating(nameof(CheckFirebaseReady), 0.5f, 0.5f);
    }

    void CheckFirebaseReady()
    {
        // FirebaseManager 싱글턴이 있는지 + IsReady인지 체크
        if (FirebaseManager.Instance != null && FirebaseManager.Instance.IsReady)
        {
            loginButton.interactable = true;
            CancelInvoke(nameof(CheckFirebaseReady));
        }
    }

    public void goToSignUp()
    {
        SceneManager.LoadScene("SignUp");
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
                    SceneManager.LoadScene("Village");
                }
            });


    }


}
