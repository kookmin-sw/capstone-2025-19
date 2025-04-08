using System;
using UnityEngine;

public class PatrolPath : MonoBehaviour {
    public Transform paths;
    public float sphereRadius = 0.5f;

    private void OnDrawGizmos() {
        for(int i=0; i<transform.childCount; i++)
        {
            int nextIndex = GetNextIndex(i);
            Gizmos.DrawSphere(GetChildPosition(i), sphereRadius);
            Gizmos.DrawLine(GetChildPosition(i), GetChildPosition(nextIndex));
        }
    }

    public Vector3 GetChildPosition(int i)
    {
        return transform.GetChild(i).position;
    }

    public int GetNextIndex(int index)
    {
        return (index+1==transform.childCount) ? 0 : index+1;
    }
}
