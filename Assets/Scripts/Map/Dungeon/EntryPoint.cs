using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    public enum NeedRoomType
    {
        None,
        Stair,
        Hallway,
        BossRoom,
        TrapRoom,

    }

    public NeedRoomType needRoomType;
    //occupied  �������
    private bool isOccupied = false;

    //public void SetOccupied(bool value = true) => isOccupied = value;
    public void SetOccupied(bool value = true)
    {
        isOccupied = value;
        testCube.SetActive(!value);
    }
    public bool IsOccupied() => isOccupied;
    [Header("�Ҵ�� �Ա� Ȯ�ο�")]
    public GameObject testCube;
}
