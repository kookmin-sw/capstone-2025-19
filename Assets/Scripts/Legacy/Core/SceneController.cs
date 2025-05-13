using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : Singleton<SceneController>
{
    [SerializeField] private float waitToLoadTime = 5f;
    Scene currentScene;

    List<GameObject> removeGameObjectList = new List<GameObject>();
    public bool isMultiplay = false;

    string nextSceneName;

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
        nextSceneName = sceneName;
        UIFadeController.Instance.FadeToBlack(LoadSceneRoutine);
        //StartCoroutine(LoadSceneRoutine(sceneName));
    }

    /*private IEnumerator LoadSceneRoutine(string sceneName)
    {
        while(waitToLoadTime >= 0)
        {
            waitToLoadTime -= Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
        currentScene = SceneManager.GetActiveScene();
        UIFadeController.Instance.FadeToClear();
    }*/

    private void LoadSceneRoutine()
    {
        SceneManager.LoadScene(nextSceneName);
        currentScene = SceneManager.GetActiveScene();
        UIFadeController.Instance.FadeToClear();
    }

    

    /*public void FadeOutScene()
    {
        UIFadeController.Instance.FadeToBlack(TestFunction);
    }*/

    public void FadeInScene()
    {
        UIFadeController.Instance.FadeToClear();
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
