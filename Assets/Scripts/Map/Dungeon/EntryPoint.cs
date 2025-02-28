using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    //occupied  사용중인
    private bool isOccupied = false;

    public void SetOccupied(bool value = true) => isOccupied = value;
    public bool IsOccupied() => isOccupied;
}
