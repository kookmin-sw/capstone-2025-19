using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Firebase.Extensions;

public class SignInManager : MonoBehaviour
{

    public bool IsFirebaseReady { get; private set; }
    public bool IsSignInOnProgress { get; private set; }


    public InputField email;
    public InputField password;
    public Button loginButton;
    public Button signUpButton;

    public static FirebaseApp firebaseApp;
    public static FirebaseAuth firebaseAuth;

    public static FirebaseUser User;

    void Start()
    {
        loginButton.interactable = false;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var result = task.Result;
            if (result != DependencyStatus.Available)
            {
                Debug.LogError(result.ToString());
                IsFirebaseReady = false;
            }
            else
            {
                Debug.Log("Firebase is ready to use!");
                IsFirebaseReady = true;

                firebaseApp = FirebaseApp.DefaultInstance;
                firebaseAuth = FirebaseAuth.DefaultInstance;

                loginButton.interactable = true;
            }

            loginButton.interactable = IsFirebaseReady;
        });
    }

    public void goToSignUp()
    {
        SceneManager.LoadScene("SignUp");
    }

    public void SignIn()
    {
        if (!IsFirebaseReady || IsSignInOnProgress || User != null)
        {
            return;
        }

        IsSignInOnProgress = true;
        loginButton.interactable = false;

        firebaseAuth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWithOnMainThread(task =>
        {
            Debug.Log($"Sign in status : {task.Status}");

            IsSignInOnProgress = false;
            loginButton.interactable = true;

            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception);
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("Sign-in canceled");
            }
            else
            {
                User = task.Result.User;
                Debug.Log(User.Email);
                SceneManager.LoadScene("Login");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
