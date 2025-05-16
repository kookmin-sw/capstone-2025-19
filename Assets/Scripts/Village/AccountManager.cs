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
using TMPro;

public class AccountManager : MonoBehaviour
{

    [SerializeField] InputField email;
    [SerializeField] InputField password;
    [SerializeField] InputField nickname;
    [SerializeField] Button signInButton;
    [SerializeField] Button createAccountButton;

    [SerializeField] GameObject errorPanel;
    [SerializeField] Text errorMessageText;

    private bool isLoginProcess;

    [SerializeField] GameObject VillageManager;

    // Start is called before the first frame update
    void Start()
    {
        if (FirebaseManager.Instance.isLoginComplete)
        {
            return;
        }
        InvokeRepeating(nameof(CheckFirebaseReady), 0.5f, 0.5f);
        isLoginProcess = true;
        nickname.gameObject.SetActive(false);
        errorPanel.SetActive(false);
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

        if (isLoginProcess)
        {
            Debug.Log("���� ���� ��ư ó�� ����");
            nickname.gameObject.SetActive(true);
            signInButton.gameObject.SetActive(false);
            isLoginProcess = false;
            return;
        }

        if (nickname.text.Trim().Length < 2)
        {
            ShowError("�г����� 2���� �̻��̾�� �մϴ�.");
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
                    foreach (var e in task.Exception.Flatten().InnerExceptions)
                    {
                        if (e is FirebaseException fe)
                        {
                            switch ((AuthError)fe.ErrorCode)
                            {
                                case AuthError.EmailAlreadyInUse:
                                    ShowError("�̹� ��� ���� �̸����Դϴ�.");
                                    break;
                                case AuthError.InvalidEmail:
                                    ShowError("��ȿ���� ���� �̸��� �����Դϴ�.");
                                    break;
                                case AuthError.WeakPassword:
                                    ShowError("��й�ȣ�� 6�� �̻��̾�� �մϴ�.");
                                    break;
                                default:
                                    ShowError("ȸ������ �� ������ �߻��߽��ϴ�.");
                                    break;
                            }
                        }
                        else
                        {
                            ShowError("ȸ������ �� ������ �߻��߽��ϴ�.");
                        }
                    }
                    return;
                }

                FirebaseUser newUser = task.Result.User;
                Debug.Log($"ȸ������ ����! UID: {newUser.UserId}");
                FirebaseManager.Instance.isLoginComplete = true;
                

                // Firestore�� ���� ���� ����
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
                        Debug.Log("Firestore�� ���� ���� ���� �Ϸ�!");
                        VillageManager.GetComponent<VillageManager>().SpawnPlayer();
                        VillageManager.GetComponent<VillageManager>().SynchronizeDBtoCash();
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
                    foreach (var e in task.Exception.Flatten().InnerExceptions)
                    {
                        if (e is FirebaseException fe)
                        {
                            switch ((AuthError)fe.ErrorCode)
                            {
                                case AuthError.UserNotFound:
                                    ShowError("�������� �ʴ� �����Դϴ�.");
                                    break;
                                case AuthError.WrongPassword:
                                    ShowError("��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
                                    break;
                                case AuthError.InvalidEmail:
                                    ShowError("��ȿ���� ���� �̸��� �����Դϴ�.");
                                    break;
                                default:
                                    ShowError("�α��� �� ������ �߻��߽��ϴ�.");
                                    break;
                            }
                        }
                        else
                        {
                            ShowError("�α��� �� ������ �߻��߽��ϴ�.");
                        }
                    }
                }
                else
                {
                    var user = task.Result.User;
                    Debug.Log($"Sign in success! UID: {user.UserId}");
                    FirebaseManager.Instance.isLoginComplete = true;
                    VillageManager.GetComponent<VillageManager>().SpawnPlayer();
                    VillageManager.GetComponent<VillageManager>().SynchronizeDBtoCash();
                }
            });     
    }

    void ShowError(string message, float duration = 2f)
    {
        StartCoroutine(ShowErrorCoroutine(message, duration));
    }

    IEnumerator ShowErrorCoroutine(string message, float duration)
    {
        errorMessageText.text = message;
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(duration);
        errorPanel.SetActive(false);
    }
}
