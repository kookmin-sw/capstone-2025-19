using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ReadyPlayerPanel : MonoBehaviour
{
    [SerializeField] Image playerProfile;
    [SerializeField] TextMeshProUGUI playerNickName;


    public void SetPlayer(string name, int index)
    {
        playerNickName.text = name;
        playerProfile.sprite = Resources.Load<Sprite>($"Sprites/PlayerProfile/PlayerProfile{index}");
    }
}
