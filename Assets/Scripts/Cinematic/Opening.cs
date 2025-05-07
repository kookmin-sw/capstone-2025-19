using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using KoreanTyper;

public class Opening : MonoBehaviour
{

    [System.Serializable]
    public class SplashPage
    {
        public Sprite image;
        public string[] scripts;
    }

    public Image splashImage;
    public TextMeshProUGUI[] texts;
    public SplashPage[] pages;

    private int currentIndex = 0;

    void Start()
    {
        ShowPage(currentIndex);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            currentIndex++;
            if (currentIndex < pages.Length)
            {
                ShowPage(currentIndex);
            }
            else
            {
                LoadNextScene(); // 씬전환 or 오브젝트 삭제
            }
        }
    }

    void ShowPage(int index)
    {
        splashImage.sprite = pages[index].image;
        print(index);
        StartCoroutine(TypingText(index));
    }

    void LoadNextScene()
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator TypingText(int index) {
        while (true) {
            //=======================================================================================================
            // Initializing | 초기화
            //=======================================================================================================
            foreach (TextMeshProUGUI t in texts)
                t.text = "";
            //=======================================================================================================


            //=======================================================================================================
            //  Typing effect | 타이핑 효과
            //=======================================================================================================
            for (int t = 0; t < texts.Length && t < pages[index].scripts.Length; t++) {
                int strTypingLength = pages[index].scripts[t].GetTypingLength();

                for (int i = 0; i <= strTypingLength; i++) {
                    texts[t].text = pages[index].scripts[t].Typing(i);
                    yield return new WaitForSeconds(0.03f);
                }
                // Wait 1 second per 1 sentence | 한 문장마다 1초씩 대기
                yield return new WaitForSeconds(1f);
            }
            // Wait 1 second at the end | 마지막에 1초 추가 대기함
            yield return new WaitForSeconds(1f);
        }
    }
}
