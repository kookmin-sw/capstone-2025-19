using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    public enum NeedRoomType
    {
        None,
        Room,
        Stair,
        Hallway,
        BossRoom,
        TrapRoom,

    }
    [Header("연결되어야 하는 방 타입 (상관 없을경우 None)")]
    public NeedRoomType needRoomType;
    //occupied  사용중인
    private bool isOccupied = false;

    //public void SetOccupied(bool value = true) => isOccupied = value;
    public void SetOccupied(bool value = true)
    {
        isOccupied = value;
        //testCube.SetActive(!value);
    }
    public bool IsOccupied() => isOccupied;
    [Header("할당된 입구 확인용")]
    public GameObject testCube;
}
