using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CreateRoomSettingPanel : MonoBehaviour
{
    [SerializeField] public TMP_InputField titleInputField;
    [SerializeField] public Toggle pwdToggle;
    [SerializeField] public TMP_InputField pwdInputField;


    // Start is called before the first frame update
    void Start()
    {
        pwdToggle.isOn = false;
        pwdInputField.enabled = false;
    }


    public void CreateRoomButton()
    {
        if(titleInputField.text == "") { Debug.LogError("Need room name!!"); return; }
        if (NetworkController.Instance.CheckSameRoomName(titleInputField.text)) { Debug.LogError("Write other Title Name!!"); return; }
        NetworkController.Instance.CreateRoom(titleInputField.text, pwdToggle.isOn, pwdInputField.text);
        Debug.Log($"title : {titleInputField.text} Lock : {pwdToggle.isOn} pwd : {pwdInputField.text}");
        
    }

    public void CancelButton()
    {
        gameObject.SetActive(false);
    }

    public void EnablePwdInputField(bool value)
    {
        pwdInputField.enabled = value;
    }

    
}
