using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        if (instance != null && this.gameObject != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as T;
        }
        if (!gameObject.transform.parent)
        {
            DontDestroyOnLoad(gameObject);
        }
        else if (gameObject.transform.parent.name == "Managers")
        {
            DontDestroyOnLoad(gameObject.transform.parent);
        }
    }
}
