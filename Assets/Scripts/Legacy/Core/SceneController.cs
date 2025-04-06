using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : Singleton<SceneController>
{
    Scene currentScene;

    List<GameObject> removeGameObjectList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string GetCurrentSceneName()
    {
        return currentScene.name;
    }

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
