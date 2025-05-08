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
        SmallRoom,

    }
    [Header("연결 되어야 하는 방 타입 (상관 없을경우 None)")]
    public NeedRoomType needRoomType;
    [Header("입구")]
    [SerializeField] public GameObject entrance;
    [Header("연결 된 방이 없을 경우 대체 할 벽")]
    [SerializeField] public GameObject wallObject;
    [SerializeField] public SerializableArray<NeedRoomType, float> needRoomArray;
    [SerializeField] public bool dontSetRoom2Entry = false;

    //occupied  사용중인
    private bool isOccupied = false;

    //public void SetOccupied(bool value = true) => isOccupied = value;
    public void SetOccupied(bool value = true)
    {
        isOccupied = value;
        //testCube.SetActive(!value);
    }
    public void SetEmptyEntrance()
    {
        Vector3 entrancePosition = entrance.transform.position;
        Destroy(entrance);
        Instantiate(wallObject).transform.position = entrancePosition;
    }

    public bool IsOccupied() => isOccupied;
    [Header("할당된 입구 확인용")]
    public GameObject testCube;
}
