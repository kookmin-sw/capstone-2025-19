using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : Singleton<SceneController>
{
    Scene currentScene;

    List<GameObject> removeGameObjectList = new List<GameObject>();
    public bool isMultiplay = false;

    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        currentScene = SceneManager.GetActiveScene();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    public string GetCurrentSceneName()
    {
        currentScene = SceneManager.GetActiveScene();
        return currentScene.name;
    }

    public bool IsMultiplay() { return isMultiplay; }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        currentScene = SceneManager.GetActiveScene();
    }

    public void SubscribeGo(GameObject go)
    {
        removeGameObjectList.Add(go);
    }
    public void UnsubscribeGo(GameObject go)
    {
        removeGameObjectList.Remove(go);
    }

    private void RemoveGo()
    {
        foreach(GameObject go in removeGameObjectList)
        {
            Destroy(go);
        }
    }
}
