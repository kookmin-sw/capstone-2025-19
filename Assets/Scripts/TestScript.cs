using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public int publicInt;
    public float publicFloat;
    public string publicString;
    public bool publicBool;
    public Vector2 publicVector2;
    public Vector3 publicVector3;
    public Quaternion publicQuaternion;
    public Color publicColor;
    public Rect publicRect;
    public RectTransform publicRectTransform;
    public List<GameObject> publicGameObjectList;
    public testEnum publicEnum;

    public enum testEnum
    {
        test1,
        test2,
        test3,

    }
}
