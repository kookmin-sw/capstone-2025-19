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
    //occupied  사용중인
    private bool isOccupied = false;

    //public void SetOccupied(bool value = true) => isOccupied = value;
    public void SetOccupied(bool value = true)
    {
        isOccupied = value;
        testCube.SetActive(!value);
    }
    public bool IsOccupied() => isOccupied;
    [Header("할당된 입구 확인용")]
    public GameObject testCube;
}
