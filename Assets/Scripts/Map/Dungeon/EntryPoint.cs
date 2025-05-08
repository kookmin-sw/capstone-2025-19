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
    [Header("���� �Ǿ�� �ϴ� �� Ÿ�� (��� ������� None)")]
    public NeedRoomType needRoomType;
    [Header("�Ա�")]
    [SerializeField] public GameObject entrance;
    [Header("���� �� ���� ���� ��� ��ü �� ��")]
    [SerializeField] public GameObject wallObject;
    [SerializeField] public SerializableArray<NeedRoomType, float> needRoomArray;
    [SerializeField] public bool dontSetRoom2Entry = false;

    //occupied  �������
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
    [Header("�Ҵ�� �Ա� Ȯ�ο�")]
    public GameObject testCube;
}
